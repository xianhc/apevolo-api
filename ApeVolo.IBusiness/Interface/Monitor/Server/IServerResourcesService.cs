using System.Threading.Tasks;
using ApeVolo.IBusiness.Vo.ServerResources;

namespace ApeVolo.IBusiness.Interface.Monitor.Server;

public interface IServerResourcesService
{
    Task<ServerResourcesInfo> Query();
}