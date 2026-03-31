namespace AuthService.Application.Common.ApplicationServices.Persistence;


public interface IUnitOfWork : IAsyncDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
