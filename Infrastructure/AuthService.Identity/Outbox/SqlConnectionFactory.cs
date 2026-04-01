/**
 * SqlConnectionFactory creates SQL connections for Identity bounded context.
 *
 * <p>Used by ProcessOutboxMessagesJob for raw SQL operations via Dapper.</p>
 */
namespace AuthService.Identity.Outbox;

using Microsoft.Data.SqlClient;
using System.Data;

using AuthService.Application.Common.ApplicationServices.Persistence;


internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();

        return connection;
    }
}
