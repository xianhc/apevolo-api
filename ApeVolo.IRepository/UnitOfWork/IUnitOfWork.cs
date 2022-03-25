using SqlSugar;

namespace ApeVolo.IRepository.UnitOfWork;

public interface IUnitOfWork
{
    SqlSugarClient GetDbClient();
    void BeginTran();
    void CommitTran();
    void RollbackTran();
}