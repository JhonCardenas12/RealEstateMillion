using System;
using System.Threading.Tasks;
namespace RealEstate.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IOwnerRepository OwnerRepository { get; }
        IPropertyRepository PropertyRepository { get; }
        IPropertyImageRepository PropertyImageRepository { get; }
        IPropertyTraceRepository PropertyTraceRepository { get; }
        IUserRepository UserRepository { get; }
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
