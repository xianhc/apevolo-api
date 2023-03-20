using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Permission.Role;
using ApeVolo.IBusiness.Dto.Permission.Department;
using ApeVolo.IBusiness.Interface.Permission.Department;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Permission.Department;
using AutoMapper;
using Castle.Core.Internal;
using SqlSugar;

namespace ApeVolo.Business.Permission.Department;

public class DepartmentService : BaseServices<Entity.Permission.Department>, IDepartmentService
{
    #region 构造函数

    public DepartmentService(IMapper mapper, IDepartmentRepository departmentRepository, ICurrentUser currentUser)
    {
        BaseDal = departmentRepository;
        Mapper = mapper;
        CurrentUser = currentUser;
    }

    #endregion

    #region 基础方法

    [UseTran]
    public async Task<bool> CreateAsync(CreateUpdateDepartmentDto createUpdateDepartmentDto)
    {
        if (await IsExistAsync(d => d.Name == createUpdateDepartmentDto.Name))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Department"),
                createUpdateDepartmentDto.Name));
        }

        Entity.Permission.Department dept = Mapper.Map<Entity.Permission.Department>(createUpdateDepartmentDto);
        await AddEntityAsync(dept);

        //重新计算子节点个数
        if (!dept.PId.IsNullOrEmpty())
        {
            var department = await BaseDal.QueryFirstAsync(x => x.Id == dept.PId);
            if (department.IsNotNull())
            {
                var departmentList =
                    await BaseDal.QueryListAsync(x => x.PId == department.Id);
                department.SubCount = departmentList.Count;
                await UpdateEntityAsync(department);
            }
        }

        return true;
    }

    [UseTran]
    public async Task<bool> UpdateAsync(CreateUpdateDepartmentDto createUpdateDepartmentDto)
    {
        var oldUseDepartment =
            await QueryFirstAsync(x => x.Id == createUpdateDepartmentDto.Id);
        if (oldUseDepartment.IsNull())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        if (oldUseDepartment.Name != createUpdateDepartmentDto.Name && await IsExistAsync(x =>
                x.Name == createUpdateDepartmentDto.Name))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Department"),
                createUpdateDepartmentDto.Name));
        }

        Entity.Permission.Department dept = Mapper.Map<Entity.Permission.Department>(createUpdateDepartmentDto);
        dept.SubCount = oldUseDepartment.SubCount;
        await UpdateEntityAsync(dept);

        //重新计算子节点个数
        //判断修改前父部门是否与修改后相同  如果相同说明并没有修改上下级部门信息
        if (oldUseDepartment.PId != dept.PId)
        {
            if (!dept.PId.IsNullOrEmpty())
            {
                var department = await BaseDal.QueryFirstAsync(x => x.Id == dept.PId);
                if (department.IsNotNull())
                {
                    var departmentList =
                        await BaseDal.QueryListAsync(x => x.PId == department.Id);
                    department.SubCount = departmentList.Count;
                    await UpdateEntityAsync(department);
                }
            }

            if (!oldUseDepartment.PId.IsNullOrEmpty())
            {
                var department =
                    await BaseDal.QueryFirstAsync(x => x.Id == oldUseDepartment.PId);
                if (department.IsNotNull())
                {
                    var departmentList =
                        await BaseDal.QueryListAsync(x => x.PId == department.Id);
                    department.SubCount = departmentList.Count;
                    await UpdateEntityAsync(department);
                }
            }
        }

        return true;
    }

    [UseTran]
    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        List<long> idList = new List<long>();
        foreach (var id in ids)
        {
            if (!idList.Contains(id))
            {
                idList.Add(id);
            }

            var departments = await BaseDal.QueryListAsync(m => m.PId == id);
            await FindChildIds(departments, idList);
        }

        var departmentList = await QueryByIdsAsync(idList);
        await DeleteEntityListAsync(departmentList);

        HashSet<long> uPIds = new HashSet<long>();

        departmentList.ForEach(d =>
        {
            if (d.PId.IsNotNull())
            {
                uPIds.Add(Convert.ToInt64(d.PId));
            }
        });

        foreach (var pid in uPIds)
        {
            var department = await BaseDal.QueryFirstAsync(x => x.Id == pid);
            if (!department.IsNotNull()) continue;

            var depts =
                await BaseDal.QueryListAsync(x => x.PId == department.Id);
            department.SubCount = depts.Count;
            await UpdateEntityAsync(department);
        }

        return true;
    }


    public async Task<List<DepartmentDto>> QueryAsync(DeptQueryCriteria deptQueryCriteria,
        Pagination pagination)
    {
        Expression<Func<Entity.Permission.Department, bool>> whereExpression = x => true;
        whereExpression = deptQueryCriteria.PId.IsNotNull()
            ? whereExpression.AndAlso(x => x.PId == deptQueryCriteria.PId)
            : whereExpression.AndAlso(x => x.PId == null);
        if (!deptQueryCriteria.DeptName.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Name.Contains(deptQueryCriteria.DeptName));
        }

        if (!deptQueryCriteria.Enabled.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Enabled == deptQueryCriteria.Enabled);
        }

        var deptList = await BaseDal.QueryPageListAsync(whereExpression, pagination);
        var deptDatalist = Mapper.Map<List<DepartmentDto>>(deptList);

        pagination.TotalElements = deptDatalist.Count;
        return deptDatalist;
    }

    public async Task<List<ExportRowModel>> DownloadAsync(DeptQueryCriteria deptQueryCriteria)
    {
        var deptList = await QueryAsync(deptQueryCriteria, new Pagination { PageSize = 9999 });
        List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
        List<ExportColumnModel> exportColumnModels;
        int point = 0;
        deptList.ForEach(dept =>
        {
            point = 0;
            exportColumnModels = new List<ExportColumnModel>();
            exportColumnModels.Add(
                new ExportColumnModel { Key = "ID", Value = dept.Id.ToString(), Point = point++ });
            exportColumnModels.Add(new ExportColumnModel { Key = "部门名称", Value = dept.Name, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "父级ID", Value = dept.PId.ToString(), Point = point++ });
            exportColumnModels.Add(
                new ExportColumnModel { Key = "排序", Value = dept.Sort.ToString(), Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "状态", Value = dept.Enabled ? "正常" : "停用", Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "子部门数量", Value = dept.SubCount.ToString(), Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "创建时间", Value = dept.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++ });
            exportRowModels.Add(new ExportRowModel { exportColumnModels = exportColumnModels });
        });
        return exportRowModels;
    }

    #endregion

    #region 扩展方法

    public async Task<List<DepartmentDto>> QuerySuperiorDeptAsync(HashSet<long> ids)
    {
        var departmentList = new List<DepartmentDto>();
        foreach (var id in ids)
        {
            var dept = await QueryFirstAsync(x => x.Id == id);
            var deptDto = Mapper.Map<DepartmentDto>(dept);
            var departmentDtoList = await FindSuperiorAsync(deptDto, new List<DepartmentDto>());
            departmentList.AddRange(departmentDtoList);
        }

        departmentList = TreeHelper<DepartmentDto>.ListToTrees(departmentList, "Id", "PId", null);

        return departmentList;
    }

    public async Task<List<DepartmentDto>> QueryByPIdAsync(long id)
    {
        return Mapper.Map<List<DepartmentDto>>(await BaseDal.QueryListAsync(x =>
            x.PId == id && x.Enabled));
    }

    public async Task<DepartmentSmallDto> QueryByIdAsync(long id)
    {
        return Mapper.Map<DepartmentSmallDto>(await BaseDal.QueryFirstAsync(x =>
            x.Id == id && x.Enabled));
    }


    public async Task<List<DepartmentDto>> QueryByRoleIdAsync(long roleId)
    {
        var departments =
            await BaseDal.QueryMuchAsync<Entity.Permission.Department, RolesDepartments, Entity.Permission.Department>(
                (d, rd) => new object[]
                {
                    JoinType.Left, d.Id == rd.DeptId
                }, (d, rd) => d,
                (d, rd) => roleId == rd.RoleId
            );
        departments = TreeHelper<Entity.Permission.Department>.SetLeafProperty(departments, "Id", "PId", null);
        return Mapper.Map<List<DepartmentDto>>(departments);
    }

    public async Task<List<long>> FindChildIds(List<long> deptIds, List<DepartmentDto> departmentDtos)
    {
        foreach (var dept in departmentDtos)
        {
            if (!dept.Enabled) continue;
            if (!deptIds.Contains(dept.Id))
            {
                deptIds.Add(dept.Id);
            }

            List<DepartmentDto> deptLists = await QueryByPIdAsync(dept.Id);
            if (deptLists != null && deptLists.Count > 0)
            {
                await FindChildIds(deptIds, deptLists);
            }
        }

        return await Task.FromResult(deptIds);
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 获取顶级部门
    /// </summary>
    /// <returns></returns>
    private async Task<List<DepartmentDto>> FindByPIdIsNullAsync()
    {
        return Mapper.Map<List<DepartmentDto>>(await BaseDal.QueryListAsync(x => x.PId == null && x.Enabled));
    }

    /// <summary>
    /// 查找同级和所有上级部门
    /// </summary>
    /// <param name="departmentDto"></param>
    /// <param name="departmentDtoList"></param>
    /// <returns></returns>
    private async Task<List<DepartmentDto>> FindSuperiorAsync(DepartmentDto departmentDto,
        List<DepartmentDto> departmentDtoList)
    {
        while (true)
        {
            if (departmentDto.PId.IsNull())
            {
                departmentDtoList.AddRange(await FindByPIdIsNullAsync());
                return departmentDtoList;
            }

            departmentDtoList.AddRange(await QueryByPIdAsync(Convert.ToInt64(departmentDto.PId)));
            departmentDto =
                Mapper.Map<DepartmentDto>(await QueryFirstAsync(x => x.Id == departmentDto.PId));
        }
    }

    /// <summary>
    /// 查找所有下级部门
    /// </summary>
    /// <param name="departmentList"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    private async Task FindChildIds(List<Entity.Permission.Department> departmentList, List<long> ids)
    {
        if (departmentList is { Count: > 0 })
        {
            foreach (var department in departmentList)
            {
                if (!ids.Contains(department.Id))
                {
                    ids.Add(department.Id);
                }

                List<Entity.Permission.Department> departments =
                    await BaseDal.QueryListAsync(m => m.PId == department.Id);
                if (departments is { Count: > 0 })
                {
                    await FindChildIds(departments, ids);
                }
            }
        }

        await Task.FromResult(ids);
    }

    #endregion
}