/**
 * TransactionPipelineBehavior wraps MediatR commands in a transaction.
 *
 * <p>Ensures atomic operations across both domain and identity contexts.</p>
 */
namespace AuthService.Application.Common.Behaviours;

using MediatR;
using System.Transactions;

using AuthService.Application.Common.ApplicationServices.Persistence;


/// <summary>
/// MediatR pipeline behavior for transaction management.
/// </summary>
public sealed class TransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    /// <summary>
    /// Initializes a new instance of TransactionPipelineBehavior.
    /// </summary>
    public TransactionPipelineBehavior(
        IUnitOfWork unitOfWork,
        IIdentityUnitOfWork identityUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _identityUnitOfWork = identityUnitOfWork;
    }

    /// <summary>
    /// Handles the request by wrapping command execution in a transaction.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Skip transaction for queries
        if (!_IsCommand())
            return await next();

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var response = await next();

        // Save both contexts within the same transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _identityUnitOfWork.SaveChangesAsync(cancellationToken);

        transaction.Complete();

        return response;
    }

    /// <summary>
    /// Checks if the request is a command (not a query).
    /// </summary>
    private bool _IsCommand()
        => typeof(TRequest).Name.EndsWith("Command");
}
