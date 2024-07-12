using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.Global;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Dto.Permission;
using Mapster;

namespace Ape.Volo.Api.Mapster;

public class MapsterCustomMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        List<(Type sourceType, Type targetType)> maps = new List<(Type sourceType, Type targetType)>();
        var attributes = GlobalType.BusinessInterfaceTypes
            .Where(x => x.GetCustomAttribute<AutoMappingAttribute>() != null)
            .Select(x => x.GetCustomAttribute<AutoMappingAttribute>());

        foreach (var attribute in attributes)
        {
            if (attribute == null) continue;
            maps.Add((attribute.SourceType, attribute.TargetType));
            maps.Add((attribute.TargetType, attribute.SourceType));
        }

        //根据AutoMappingAttribute特性自动映射
        maps.ForEach(aMap => { config.NewConfig(aMap.sourceType, aMap.targetType); });


        //自定义映射 会覆盖存在的
        // config.NewConfig<User, UserDto>() .Ignore(dest => dest.Password)
        //     .Map(dest => dest.Dept123, src => src.Dept.Adapt<DepartmentSmallDto>());
    }
}
