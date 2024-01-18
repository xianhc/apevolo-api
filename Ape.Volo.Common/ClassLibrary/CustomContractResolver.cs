using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ape.Volo.Common.ClassLibrary;

public class CustomContractResolver : CamelCasePropertyNamesContractResolver
{
    ///// <summary>
    ///// 实现首字母小写
    ///// </summary>
    ///// <param name="propertyName"></param>
    ///// <returns></returns>
    protected override string ResolvePropertyName(string propertyName)
    {
        return propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
    }

    /// <summary>
    /// 对长整型做处理
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns></returns>
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        if (objectType == typeof(long))
        {
            return new JsonConverterLong();
        }

        return base.ResolveContractConverter(objectType);
    }

    ///// <summary>
    /////   Creates a Newtonsoft.Json.Serialization.JsonProperty for the given System.Reflection.MemberInfo.
    ///// </summary>
    ///// <param name="type"></param>
    ///// <param name="memberSerialization"></param>
    ///// <returns></returns>
    //protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    //{
    //    return type.GetProperties()
    //            .Select(p =>
    //            {
    //                var jp = base.CreateProperty(p, memberSerialization);
    //                jp.ValueProvider = new NullToEmptyStringValueProvider(p);
    //                return jp;
    //            }).ToList();
    //}
}
