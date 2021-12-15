using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.IBusiness.Dto.Core;

namespace ApeVolo.IBusiness.Interface.Core
{
    /// <summary>
    /// 数据权限接口
    /// </summary>
    public interface IDataScopeService
    {
        Task<List<long>> GetDeptIds(UserDto userDto);
    }
}