using System.Data;
using System.Threading.Tasks;
using RealEstate.Application.Interfaces;

namespace RealEstate.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _db;
        private readonly IOwnerRepository _ownerRepo;
        private readonly IPropertyRepository _propRepo;
        private readonly IPropertyImageRepository _imgRepo;
        private readonly IPropertyTraceRepository _traceRepo;
        private readonly IUserRepository _userRepo;
        private IDbTransaction _transaction;

        public UnitOfWork(IDbConnection db, IOwnerRepository ownerRepo, IPropertyRepository propRepo, IPropertyImageRepository imgRepo, IPropertyTraceRepository traceRepo, IUserRepository userRepo)
        {
            _db = db;
            _ownerRepo = ownerRepo;
            _propRepo = propRepo;
            _imgRepo = imgRepo;
            _traceRepo = traceRepo;
            _userRepo = userRepo;
        }

        public IOwnerRepository OwnerRepository => _ownerRepo;
        public IPropertyRepository PropertyRepository => _propRepo;
        public IPropertyImageRepository PropertyImageRepository => _imgRepo;
        public IPropertyTraceRepository PropertyTraceRepository => _traceRepo;
        public IUserRepository UserRepository => _userRepo;

        public async Task BeginTransactionAsync()
        {
            if (_db.State != ConnectionState.Open) _db.Open();
            _transaction = _db.BeginTransaction();
            await Task.CompletedTask;
        }

        public async Task CommitAsync()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            _transaction = null;
            await Task.CompletedTask;
        }

        public async Task RollbackAsync()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            if (_db?.State == ConnectionState.Open) _db.Close();
        }
    }
}
