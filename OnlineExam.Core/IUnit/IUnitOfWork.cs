using OnlineExam.Core.IRepositories.Generic;

namespace OnlineExam.Core.IUnit
{
    public interface IUnitOfWork<T> : IAsyncDisposable where T : class
    {
        IGenericRepositoryAsync<T> GenericRepository { get; }
        Task<int> CommitAsync(CancellationToken cancellationToken = default);

        // Transaction methods
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        Task ExecuteWithStrategyAsync(Func<Task> action, CancellationToken cancellationToken = default);
    }
}
