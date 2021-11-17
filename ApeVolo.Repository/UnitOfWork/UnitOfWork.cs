using ApeVolo.Common.DI;
using ApeVolo.IRepository.UnitOfWork;
using SqlSugar;

namespace ApeVolo.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDependencyRepository
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public UnitOfWork(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        /// <summary>
        /// 获取DB，保证唯一性
        /// </summary>
        /// <returns></returns>
        public SqlSugarClient GetDbClient()
        {
            return _sqlSugarClient as SqlSugarClient;
        }

        public void BeginTran()
        {
            GetDbClient().BeginTran();
        }

        public void CommitTran()
        {
            try
            {
                GetDbClient().CommitTran();
            }
            catch
            {
                GetDbClient().RollbackTran();
                throw;
            }
        }

        public void RollbackTran()
        {
            GetDbClient().RollbackTran();
        }
    }
}