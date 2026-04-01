/**
 * ProcessOutboxMessagesJob processes outbox messages for Identity bounded context.
 *
 * <p>Scans Identity.OutboxMessages table for pending domain events.
 * Publishes events via MediatR and marks messages as processed.</p>
 */
namespace AuthService.Identity.Outbox;

using Dapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;

using AuthService.Application.Common.ApplicationServices.Persistence;
using AuthService.Domain.Common;
using AuthService.Identity.Settings;


internal sealed class ProcessOutboxMessagesJob
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IPublisher _publisher;
    private readonly OutboxSettings _outboxOptions;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        [FromKeyedServices("Identity")] ISqlConnectionFactory sqlConnectionFactory,
        IPublisher publisher,
        IOptions<OutboxSettings> outboxOptions,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _publisher = publisher;
        _logger = logger;
        _outboxOptions = outboxOptions.Value;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Beginning to process outbox messages");

        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        using IDbTransaction transaction = connection.BeginTransaction();

        IReadOnlyList<OutboxMessageResponse> outboxMessages = await _GetOutboxMessagesAsync(connection, transaction);

        foreach (OutboxMessageResponse outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    JsonSerializerSettings)!;

                await _publisher.Publish(domainEvent);
            }
            catch (Exception caughtException)
            {
                _logger.LogError(
                    caughtException,
                    "Exception while processing outbox message {MessageId}",
                    outboxMessage.Id);

                exception = caughtException;
            }

            await _UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }

        transaction.Commit();

        _logger.LogInformation("Completed processing outbox messages");
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> _GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        string sql = $@"
            SELECT TOP({_outboxOptions.BatchSize}) Id, Content
            FROM [Identity].[OutboxMessages] WITH (UPDLOCK)
            WHERE ProcessedOnUtc IS NULL
            ORDER BY OccurredOnUtc";

        IEnumerable<OutboxMessageResponse> outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(
            sql,
            transaction: transaction);

        return outboxMessages.ToList();
    }

    private async Task _UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        const string sql = @"
            UPDATE [Identity].[OutboxMessages]
            SET ProcessedOnUtc = @ProcessedOnUtc,
                Error = @Error
            WHERE Id = @Id";

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOnUtc = DateTime.UtcNow,
                Error = exception?.ToString()
            },
            transaction: transaction);
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
