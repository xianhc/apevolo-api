using System.Threading.Tasks;
using ApeVolo.IBusiness.Vo.ServerResources;

namespace ApeVolo.IBusiness.Interface.Monitor;

public interface IServerResourcesService
{
    Task<ServerResourcesInfo> Query();
}
