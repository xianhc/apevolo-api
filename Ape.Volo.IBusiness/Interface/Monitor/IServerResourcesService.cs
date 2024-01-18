using System.Threading.Tasks;
using Ape.Volo.IBusiness.Vo.ServerResources;

namespace Ape.Volo.IBusiness.Interface.Monitor;

public interface IServerResourcesService
{
    Task<ServerResourcesInfo> Query();
}
