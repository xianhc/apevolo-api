using ApeVolo.Entity.Monitor.Logs;
using ApeVolo.IRepository.Base;

namespace ApeVolo.IRepository.Monitor.Exception;

public interface IExceptionLogRepository : ISugarHandler<ExceptionLog>
{
}