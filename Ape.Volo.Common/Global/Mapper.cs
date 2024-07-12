#nullable enable
using Mapster;

namespace Ape.Volo.Common.Global;

/// <summary>
/// 对象映射
/// </summary>
public class Mapper : IMapper
{
    public TDestination MapTo<TDestination>(object source)
    {
        return source.Adapt<TDestination>();
    }

    public TDestination MapTo<TSource, TDestination>(TSource source)
    {
        return source.Adapt<TSource, TDestination>();
    }
}

public interface IMapper
{
    TDestination MapTo<TDestination>(object source);

    TDestination MapTo<TSource, TDestination>(TSource source);
}
