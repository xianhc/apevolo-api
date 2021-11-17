using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Core;
using AutoMapper;
using Castle.Core.Internal;
using SqlSugar;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.Business.Impl.Core
{
    public class DepartmentService : BaseServices<Department>, IDepartmentService
    {
        #region 构造函数

        public DepartmentService(IMapper mapper, IDepartmentRepository departmentRepository)
        {
            _baseDal = departmentRepository;
            _mapper = mapper;
        }

        #endregion

        #region 基础方法

        [UseTran]
        public async Task<bool> CreateAsync(CreateUpdateDepartmentDto createUpdateDepartmentDto)
        {
            if (await IsExistAsync(d => d.IsDeleted == false
                                        && d.Name == createUpdateDepartmentDto.Name))
            {
                throw new BadRequestException($"部门=》{createUpdateDepartmentDto.Name}=》已存在!");
            }

            Department dept = _mapper.Map<Department>(createUpdateDepartmentDto);
            await AddEntityAsync(dept);

            //重新计算子节点个数
            if (!dept.PId.IsNullOrEmpty())
            {
                var department = await _baseDal.QueryFirstAsync(x => x.IsDeleted == false && x.Id == dept.PId);
                if (department.IsNotNull())
                {
                    var departmentList =
                        await _baseDal.QueryListAsync(x => x.IsDeleted == false && x.PId == department.Id);
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
                await QueryFirstAsync(x => x.IsDeleted == false && x.Id == createUpdateDepartmentDto.Id);
            if (oldUseDepartment.IsNull())
            {
                throw new BadRequestException("更新失败=》待更新数据不存在！");
            }

            if (oldUseDepartment.Name != createUpdateDepartmentDto.Name && await IsExistAsync(x =>
                x.IsDeleted == false
                && x.Name == createUpdateDepartmentDto.Name))
            {
                throw new BadRequestException($"部门名称=>{createUpdateDepartmentDto.Name}=>已存在！");
            }

            Department dept = _mapper.Map<Department>(createUpdateDepartmentDto);

            await UpdateEntityAsync(dept);

            //重新计算子节点个数
            //判断修改前父部门是否与修改后相同  如果相同说明并没有修改上下级部门信息
            if (oldUseDepartment.PId != dept.PId)
            {
                if (!dept.PId.IsNullOrEmpty())
                {
                    var department = await _baseDal.QueryFirstAsync(x => x.IsDeleted == false && x.Id == dept.PId);
                    if (department.IsNotNull())
                    {
                        var departmentList =
                            await _baseDal.QueryListAsync(x => x.IsDeleted == false && x.PId == department.Id);
                        department.SubCount = departmentList.Count;
                        await UpdateEntityAsync(department);
                    }
                }

                if (!oldUseDepartment.PId.IsNullOrEmpty())
                {
                    var department =
                        await _baseDal.QueryFirstAsync(x => x.IsDeleted == false && x.Id == oldUseDepartment.PId);
                    if (department.IsNotNull())
                    {
                        var departmentList =
                            await _baseDal.QueryListAsync(x => x.IsDeleted == false && x.PId == department.Id);
                        department.SubCount = departmentList.Count;
                        await UpdateEntityAsync(department);
                    }
                }
            }

            return true;
        }

        [UseTran]
        public async Task<bool> DeleteAsync(HashSet<string> ids)
        {
            List<string> idList = new List<string>();
            ids.ForEach(async id =>
            {
                if (!idList.Contains(id))
                {
                    idList.Add(id);
                }

                var departments = await _baseDal.QueryListAsync(m => m.IsDeleted == false && m.PId == id);
                await FindChildIds(departments, idList);
            });
            var departmentList = await QueryByIdsAsync(idList);
            await DeleteEntityListAsync(departmentList);

            List<string> uPIds = new List<string>();

            departmentList.ForEach(d =>
            {
                if (!uPIds.Contains(d.PId) && !d.PId.IsNullOrEmpty())
                {
                    uPIds.Add(d.PId);
                }
            });

            uPIds.ForEach(async pid =>
            {
                var department = await _baseDal.QueryFirstAsync(x => x.IsDeleted == false && x.Id == pid);
                if (department.IsNotNull())
                {
                    var depts =
                        await _baseDal.QueryListAsync(x => x.IsDeleted == false && x.PId == department.Id);
                    department.SubCount = depts.Count;
                    await UpdateEntityAsync(department);
                }
            });

            return true;
        }


        public async Task<List<DepartmentDto>> QueryAsync(DeptQueryCriteria deptQueryCriteria,
            Pagination pagination)
        {
            Expression<Func<Department, bool>> whereExpression = x => (x.IsDeleted == false);
            if (!deptQueryCriteria.DeptName.IsNullOrEmpty())
            {
                whereExpression = whereExpression.And(x => x.Name.Contains(deptQueryCriteria.DeptName));
            }

            if (!deptQueryCriteria.PId.IsNullOrEmpty())
            {
                whereExpression = whereExpression.And(x => x.PId == deptQueryCriteria.PId);
            }

            if (!deptQueryCriteria.Enabled.IsNullOrEmpty())
            {
                whereExpression = whereExpression.And(x => x.Enabled == deptQueryCriteria.Enabled);
            }

            var deptList = await _baseDal.QueryPageListAsync(whereExpression, pagination);
            var deptDatalist = _mapper.Map<List<DepartmentDto>>(deptList);
            if (deptQueryCriteria.PId.IsNullOrEmpty())
            {
                deptDatalist = deptDatalist.Where(x => x.PId == null).ToList();
            }

            pagination.TotalElements = deptDatalist.Count;
            return deptDatalist;
        }

        public async Task<List<ExportRowModel>> DownloadAsync(DeptQueryCriteria deptQueryCriteria)
        {
            var deptList = await QueryAsync(deptQueryCriteria, new Pagination() {PageSize = 9999});
            List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
            List<ExportColumnModel> exportColumnModels;
            int point = 0;
            deptList.ForEach(dept =>
            {
                point = 0;
                exportColumnModels = new List<ExportColumnModel>();
                exportColumnModels.Add(new ExportColumnModel {Key = "ID", Value = dept.Id, Point = point++});
                exportColumnModels.Add(new ExportColumnModel {Key = "部门名称", Value = dept.Name, Point = point++});
                exportColumnModels.Add(new ExportColumnModel {Key = "父级ID", Value = dept.PId, Point = point++});
                exportColumnModels.Add(
                    new ExportColumnModel {Key = "排序", Value = dept.Sort.ToString(), Point = point++});
                exportColumnModels.Add(new ExportColumnModel
                    {Key = "状态", Value = dept.Enabled ? "正常" : "停用", Point = point++});
                exportColumnModels.Add(new ExportColumnModel
                    {Key = "子部门数量", Value = dept.SubCount.ToString(), Point = point++});
                exportColumnModels.Add(new ExportColumnModel
                    {Key = "创建时间", Value = dept.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++});
                exportRowModels.Add(new ExportRowModel() {exportColumnModels = exportColumnModels});
            });
            return exportRowModels;
        }

        #endregion

        #region 扩展方法

        public async Task<List<DepartmentDto>> QuerySuperiorDeptAsync(List<string> ids)
        {
            DepartmentDto departmentDto = null;
            List<DepartmentDto> departmentDtos = new List<DepartmentDto>();
            var dtos = departmentDtos;
            ids.ForEach(async (s, index) =>
            {
                departmentDto =
                    _mapper.Map<DepartmentDto>(await QueryFirstAsync(x => x.IsDeleted == false && x.Id == s));
                List<DepartmentDto> depts = await FindSuperiorAsync(departmentDto, new List<DepartmentDto>());
                dtos.AddRange(depts);
            });
            departmentDtos = TreeHelper<DepartmentDto>.ListToTrees(departmentDtos, "Id", "PId", null);

            return await Task.FromResult(departmentDtos);
        }

        public async Task<List<DepartmentDto>> QueryByPIdAsync(string id)
        {
            return _mapper.Map<List<DepartmentDto>>(await _baseDal.QueryListAsync(x =>
                x.IsDeleted == false && x.PId == id && x.Enabled == true));
        }

        public async Task<DepartmentSmallDto> QueryByIdAsync(string id)
        {
            return _mapper.Map<DepartmentSmallDto>(await _baseDal.QueryFirstAsync(x =>
                x.IsDeleted == false && x.Id == id && x.Enabled));
        }


        public async Task<List<DepartmentDto>> QueryByRoleIdAsync(string roleId)
        {
            var departments = await _baseDal.QueryMuchAsync<Department, RolesDepartments, Department>(
                (d, rd) => new object[]
                {
                    JoinType.Left, d.Id == rd.DeptId
                }, (d, rd) => d,
                (d, rd) => d.IsDeleted == false && roleId == rd.RoleId
            );
            departments = TreeHelper<Department>.SetLeafProperty(departments, "Id", "PId", "0");
            return _mapper.Map<List<DepartmentDto>>(departments);
        }

        public async Task<List<string>> FindChildIds(List<string> deptIds, List<DepartmentDto> departmentDtos)
        {
            departmentDtos.ForEach(async dept =>
            {
                if (dept.Enabled)
                {
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
            });
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
            return _mapper.Map<List<DepartmentDto>>(await _baseDal.QueryListAsync(x =>
                x.IsDeleted == false && x.PId == null && x.Enabled));
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
                if (departmentDto.PId.IsNullOrEmpty())
                {
                    departmentDtoList.AddRange(await FindByPIdIsNullAsync());
                    return departmentDtoList;
                }

                departmentDtoList.AddRange(await QueryByPIdAsync(departmentDto.PId));
                departmentDto =
                    _mapper.Map<DepartmentDto>(await QueryFirstAsync(x =>
                        x.IsDeleted == false && x.Id == departmentDto.PId));
            }
        }

        /// <summary>
        /// 查找所有下级部门
        /// </summary>
        /// <param name="departmentList"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task FindChildIds(List<Department> departmentList, List<string> ids)
        {
            departmentList.ForEach(async ml =>
            {
                if (!ids.Contains(ml.Id))
                {
                    ids.Add(ml.Id);
                }

                List<Department> departments =
                    await _baseDal.QueryListAsync(m => m.IsDeleted == false && m.PId == ml.Id);
                if (departments is {Count: > 0})
                {
                    await FindChildIds(departments, ids);
                }
            });

            await Task.FromResult(ids);
        }

        #endregion
    }
}