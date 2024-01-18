using SqlSugar;

namespace Ape.Volo.Repository.UnitOfWork;

public interface IUnitOfWork
{
    SqlSugarScope GetDbClient();
    void BeginTran();
    void CommitTran();
    void RollbackTran();
}
