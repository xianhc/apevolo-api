using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.IBusiness.Dto.Permission.User;

namespace ApeVolo.IBusiness.Interface.Permission.Role;

/// <summary>
/// 数据权限接口
/// </summary>
public interface IDataScopeService
{
    Task<List<long>> GetDeptIds(UserDto userDto);
}