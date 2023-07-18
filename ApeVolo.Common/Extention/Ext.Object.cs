using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ApeVolo.Common.ClassLibrary;
using Newtonsoft.Json;

namespace ApeVolo.Common.Extention;

public static partial class ExtObject
{
    private static BindingFlags BindingFlags =>
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;


    /// <summary>
    /// 判断是否为Null或者空
    /// </summary>
    /// <param name="obj">对象</param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this object obj)
    {
        if (obj == null)
            return true;
        string objStr = obj.ToString();
        return string.IsNullOrEmpty(objStr);
    }

    /// <summary>
    /// 不等于NULL？
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNotNull(this object obj)
    {
        return obj != null;
    }

    /// <summary>
    /// 等于NULL？
    /// </summary>
    /// <param name="obj">对象</param>
    /// <returns></returns>
    public static bool IsNull(this object obj)
    {
        return obj == null;
    }

    /// <summary>
    /// 将对象序列化成Json字符串
    /// </summary>
    /// <param name="obj">需要序列化的对象</param>
    /// <returns></returns>
    public static string ToJson(this object obj)
    {
        var serializerSettings = new JsonSerializerSettings
        {
            // 设置为驼峰命名
            //ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ContractResolver = new CustomContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        //serializerSettings.Converters.Add(new UnixDateTimeConvertor());

        return JsonConvert.SerializeObject(obj, Formatting.None, serializerSettings);
    }

    /// <summary>
    /// 将对象序列化成Json字符串
    /// </summary>
    /// <param name="obj">需要序列化的对象</param>
    /// <returns></returns>
    public static string ToRedisJson(this object obj)
    {
        var serializerSettings = new JsonSerializerSettings
        {
            // 设置为驼峰命名
            //ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ContractResolver = new CustomContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        return JsonConvert.SerializeObject(obj, Formatting.None, serializerSettings);
    }

    /// <summary>
    /// 将对象序列化成Json字符串,同时忽略null字段
    /// </summary>
    /// <param name="obj">需要序列化的对象</param>
    /// <returns></returns>
    public static string ToJsonByIgnore(this object obj)
    {
        var serializerSettings = new JsonSerializerSettings
        {
            // 设置为驼峰命名
            //ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ContractResolver = new CustomContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        return JsonConvert.SerializeObject(obj, Formatting.None, serializerSettings);
    }

    /// <summary>
    /// 实体类转json数据，速度快
    /// </summary>
    /// <param name="t">实体类</param>
    /// <returns></returns>
    public static string EntityToJson(this object t)
    {
        if (t == null)
            return null;
        string jsonStr = "";
        jsonStr += "{";
        PropertyInfo[] infos = t.GetType().GetProperties();
        for (int i = 0; i < infos.Length; i++)
        {
            jsonStr = jsonStr + "\"" + infos[i].Name + "\":\"" + infos[i].GetValue(t) + "\"";
            if (i != infos.Length - 1)
                jsonStr += ",";
        }

        jsonStr += "}";
        return jsonStr;
    }

    /// <summary>
    /// 深复制
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="obj">对象</param>
    /// <returns></returns>
    public static T DeepClone<T>(this T obj) where T : class
    {
        if (obj == null)
            return null;

        return obj.ToJson().ToObject<T>();
    }

    /// <summary>
    /// 将对象序列化为XML字符串
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="obj">对象</param>
    /// <returns></returns>
    public static string ToXmlStr<T>(this T obj)
    {
        var jsonStr = obj.ToJson();
        var xmlDoc = JsonConvert.DeserializeXmlNode(jsonStr);
        if (xmlDoc != null)
        {
            string xmlDocStr = xmlDoc.InnerXml;

            return xmlDocStr;
        }

        return null;
    }

    /// <summary>
    /// 将对象序列化为XML字符串
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="obj">对象</param>
    /// <param name="rootNodeName">根节点名(建议设为xml)</param>
    /// <returns></returns>
    public static string ToXmlStr<T>(this T obj, string rootNodeName)
    {
        var jsonStr = obj.ToJson();
        var xmlDoc = JsonConvert.DeserializeXmlNode(jsonStr, rootNodeName);
        if (xmlDoc != null)
        {
            string xmlDocStr = xmlDoc.InnerXml;

            return xmlDocStr;
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static int ToInt(this object thisValue)
    {
        int reval = 0;
        if (thisValue == null) return 0;
        if (thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return reval;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static int ToInt(this object thisValue, int errorValue)
    {
        int reval;
        if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return errorValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static double ToMoney(this object thisValue)
    {
        double reval;
        if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static double ToMoney(this object thisValue, double errorValue)
    {
        double reval;
        if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return errorValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static string ToString(this object thisValue)
    {
        if (thisValue != null) return thisValue.ToString()?.Trim();
        return "";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static string ToString(this object thisValue, string errorValue)
    {
        if (thisValue != null) return thisValue.ToString()?.Trim();
        return errorValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static decimal ToDecimal(this object thisValue)
    {
        decimal reval;
        if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static decimal ToDecimal(this object thisValue, decimal errorValue)
    {
        decimal reval;
        if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return errorValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static DateTime ToDate(this object thisValue)
    {
        DateTime reval = DateTime.MinValue;
        if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
        {
            reval = Convert.ToDateTime(thisValue);
        }

        return reval;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static DateTime ToDate(this object thisValue, DateTime errorValue)
    {
        DateTime reval;
        if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return errorValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static bool ToBool(this object thisValue)
    {
        bool reval = false;
        if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return reval;
    }

    /// <summary>
    /// 是否拥有某属性
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="propertyName">属性名</param>
    /// <returns></returns>
    public static bool ContainsProperty(this object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName, BindingFlags) != null;
    }

    /// <summary>
    /// 获取某属性值
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="propertyName">属性名</param>
    /// <returns></returns>
    public static object GetPropertyValue(this object obj, string propertyName)
    {
        var pi = obj.GetType().GetProperty(propertyName, BindingFlags);
        if (pi == null) return null;
        var type = pi.PropertyType;
        if (type == typeof(DateTime))
        {
            return Convert.ToDateTime(pi.GetValue(obj)).ToString("yyyy-MM-dd HH:mm:ss");
        }

        if (type.IsEnum)
        {
            return (int)pi.GetValue(obj)!;
        }

        return pi.GetValue(obj);
    }

    /// <summary>
    /// 设置某属性值
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="propertyName">属性名</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    public static void SetPropertyValue(this object obj, string propertyName, object value)
    {
        obj.GetType().GetProperty(propertyName, BindingFlags)?.SetValue(obj, value);
    }

    /// <summary>
    /// 是否拥有某字段
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="fieldName">字段名</param>
    /// <returns></returns>
    public static bool ContainsField(this object obj, string fieldName)
    {
        return obj.GetType().GetField(fieldName, BindingFlags) != null;
    }

    /// <summary>
    /// 获取某字段值
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="fieldName">字段名</param>
    /// <returns></returns>
    public static object GetGetFieldValue(this object obj, string fieldName)
    {
        return obj.GetType().GetField(fieldName, BindingFlags)?.GetValue(obj);
    }

    /// <summary>
    /// 设置某字段值
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="fieldName">字段名</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    public static void SetFieldValue(this object obj, string fieldName, object value)
    {
        obj.GetType().GetField(fieldName, BindingFlags)?.SetValue(obj, value);
    }

    /// <summary>
    /// 改变实体类型
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="targetType">目标类型</param>
    /// <returns></returns>
    public static object ChangeType(this object obj, Type targetType)
    {
        return obj.ToJson().ToObject(targetType);
    }

    /// <summary>
    /// 改变实体类型
    /// </summary>
    /// <typeparam name="T">目标泛型</typeparam>
    /// <param name="obj">对象</param>
    /// <returns></returns>
    public static T ChangeType<T>(this object obj)
    {
        return obj.ToJson().ToObject<T>();
    }

    /// <summary>
    /// 改变类型
    /// </summary>
    /// <param name="obj">原对象</param>
    /// <param name="targetType">目标类型</param>
    /// <returns></returns>
    public static object ChangeType_ByConvert(this object obj, Type targetType)
    {
        object resObj;
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
        {
            NullableConverter newNullableConverter = new NullableConverter(targetType);
            resObj = newNullableConverter.ConvertFrom(obj);
        }
        else
        {
            resObj = Convert.ChangeType(obj, targetType);
        }

        return resObj;
    }

    public static string GetGenericTypeName(this Type type)
    {
        string typeName;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }

    public static string GetGenericTypeName(this object obj)
    {
        return obj.GetType().GetGenericTypeName();
    }
}