using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        #region Read
        public Task<Subject?> GetByIdAsync(int id, CancellationToken cancellationToken);
        #endregion
    }
}
