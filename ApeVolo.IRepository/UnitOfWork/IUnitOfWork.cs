using SqlSugar;

namespace ApeVolo.IRepository.UnitOfWork;

public interface IUnitOfWork
{
    SqlSugarScope GetDbClient();
    void BeginTran();
    void CommitTran();
    void RollbackTran();
}