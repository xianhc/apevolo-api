#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApeVolo.Common.Helper;

/// <summary>
/// 构建树
/// </summary>
/// <typeparam name="T">泛型</typeparam>
public static class TreeHelper<T> where T : new()
{
    /// <summary>
    /// 构建树结构
    /// </summary>
    /// <param name="lists">数据源</param>
    /// <param name="code">ID编码</param>
    /// <param name="parentCode">父级编码</param>
    /// <param name="value">父级编码值</param>
    /// <param name="childNodeName">子节点属性名称</param>
    /// <returns></returns>
    public static List<T> ListToTrees(List<T> lists, string code, string parentCode, long value,
        string childNodeName = "Children")
    {
        if (lists.Count > 0)
        {
            var tempLists = lists
                .Where(m => (long?)m?.GetType().GetProperty(parentCode)?.GetValue(m, null) == value).ToList();
            if (tempLists.Count > 0)
            {
                var treeDatas = new List<T>();
                T t;
                Type? type;
                foreach (var obj in tempLists)
                {
                    t = obj;
                    type = obj?.GetType();

                    var childs = ListToTrees(lists, code, parentCode,
                        (long)(type?.GetProperty(code)?.GetValue(t, null) ?? 0));
                    if (childs.Count > 0)
                    {
                        type?.GetProperty(childNodeName)?.SetValue(t, childs, null);
                        //type.GetProperty("HasChildren").SetValue(t, true, null);
                        //type.GetProperty("Leaf").SetValue(t, false, null);
                    }

                    treeDatas.Add(t);
                }

                return treeDatas;
            }
        }

        return new List<T>();
    }

    public static List<T> SetLeafProperty(List<T> lists, string code, string parentCode, long value,
        string childNodeName = "Children")
    {
        if (lists.Count > 0)
        {
            var temp_lists =
                lists; //.Where(m => m.GetType().GetProperty(parentCode).GetValue(m, null).ToString() == value).ToList();
            if (temp_lists.Count > 0)
            {
                var treeDatas = new List<T>();
                T t;
                Type? type;
                foreach (var obj in temp_lists)
                {
                    t = obj;
                    type = obj?.GetType();

                    //var childs = ListToTrees(lists, code, parentCode, type.GetProperty(code).GetValue(t, null).ToString());
                    var childs = temp_lists.Where(m =>
                        m?.GetType().GetProperty(parentCode)?.GetValue(m, null)?.ToString() ==
                        type?.GetProperty(code)?.GetValue(t, null)?.ToString()).ToList();
                    if (childs.Count > 0)
                    {
                        //type.GetProperty(childNodeName).SetValue(t, null, null);
                        /*    if (type.GetProperty("HasChildren") != null)
                            {
                                type.GetProperty("HasChildren").SetValue(t, true, null);
                            }

                            if (type.GetProperty("Leaf") != null)
                            {
                                type.GetProperty("Leaf").SetValue(t, false, null);
                            }*/
                    }

                    treeDatas.Add(t);
                }

                return treeDatas;
            }
        }

        return new List<T>();
    }
}
