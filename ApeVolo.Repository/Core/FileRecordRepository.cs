using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Core;

public class FileRecordRepository : SugarHandler<FileRecord>, IFileRecordRepository
{
    public FileRecordRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}