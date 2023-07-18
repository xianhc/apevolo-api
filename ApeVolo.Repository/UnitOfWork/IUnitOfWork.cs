using SqlSugar;

namespace ApeVolo.Repository.UnitOfWork;

public interface IUnitOfWork
{
    SqlSugarScope GetDbClient();
    void BeginTran();
    void CommitTran();
    void RollbackTran();
}