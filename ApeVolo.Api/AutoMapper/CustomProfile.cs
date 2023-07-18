using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Global;
using AutoMapper;

namespace ApeVolo.Api.AutoMapper;

public class CustomProfile : Profile
{
    public CustomProfile()
    {
        // CreateMap<User, UserDTO>();
        // CreateMap<UserDTO, User>();
        //CreateMap<User, UserDTO>().ForMember(p => p.UserName, e => e.MapFrom(p => p.UserName + p.NickName));

        //CreateMap<User, UserDTO>().BeforeMap((source, dto) =>
        //{
        //    //可以较为精确的控制输出数据格式
        //    dto.CreateTime = long.Parse("123456");//Convert.ToDateTime(source.CreateTime).ToString("yyyy-MM-dd")
        //});
        InitAutoMapper();
    }

    /// <summary>
    /// 所有含有AutoInject属性的类进行映射
    /// </summary>
    /// <returns></returns>
    private void InitAutoMapper()
    {
        List<(Type sourceType, Type targetType)> maps = new List<(Type sourceType, Type targetType)>();
        var atributes = GlobalData.GetIBusinessAssembly().GetTypes()
            .Where(x => x.GetCustomAttribute<AutoMappingAttribute>() != null)
            .Select(x => x.GetCustomAttribute<AutoMappingAttribute>());

        foreach (var atribute in atributes)
        {
            if (atribute != null)
            {
                maps.Add((atribute.SourceType, atribute.TargetType));
                maps.Add((atribute.TargetType, atribute.SourceType));
            }
        }

        maps.ForEach(aMap => { CreateMap(aMap.sourceType, aMap.targetType); });

        //如果有实体之间有特殊的字段转换 需要使用下面方式实现
        //同时不能在实体加AutoMappingAttribute


        //添加编辑类型DTO
        //CreateMap<Department, CreateUpdateDepartmentDto>().ForMember(x => x.CreateTime, s => s.MapFrom(s => 1591109078000)).ForMember(x => x.UpdateTime, s => s.MapFrom(s => 1591109078000));
        //CreateMap<CreateUpdateDepartmentDto, Department>().ForMember(x => x.CreateTime, s => s.MapFrom(s => TicksToDate(s.CreateTime))).ForMember(x => x.UpdateTime, s => s.MapFrom(s => TicksToDate(s.CreateTime)));

        //前端使用时间戳类型 后端是DateTime 这里做下特殊处理 
        //需要使用时间戳转DateTime的类 不要使用AutoInjectAttribute
        // CreateMap<CreateUpdateDepartmentDto, Department>()
        //    .ForMember(x => x.CreateTime, s => s.MapFrom(s => TicksToDateTime(s.CreateTime)))
        //    .ForMember(x => x.UpdateTime, s => s.MapFrom(s => TicksToDateTime(s.UpdateTime)));
        // CreateMap<CreateUpdateUserDto, User>()
        //     .ForMember(x => x.CreateTime, s => s.MapFrom(s => TicksToDateTime(s.CreateTime)))
        //    .ForMember(x => x.UpdateTime, s => s.MapFrom(s => TicksToDateTime(s.UpdateTime)))
        //    .ForMember(x => x.PasswordReSetTime, s => s.MapFrom(s => TicksToDateTime(s.PasswordReSetTime)));
        // CreateMap<CreateUpdateRoleDto, Role>()
    }

    //时间戳(毫秒值)String转换为DateTime类型转换
    // private DateTime TicksToDateTime(long time)
    // {
    //     var dateTime = new DateTime((Convert.ToInt64(time) * 10000) + 621355968000000000);
    //     return dateTime.AddHours(8);
    // }
}
