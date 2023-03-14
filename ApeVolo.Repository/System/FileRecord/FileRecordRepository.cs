using ApeVolo.IRepository.System.FileRecord;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.System.FileRecord;

public class FileRecordRepository : SugarHandler<Entity.Do.Core.FileRecord>, IFileRecordRepository
{
    public FileRecordRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}