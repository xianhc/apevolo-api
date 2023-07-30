using System;
using System.IO;
using System.Reflection;

namespace ApeVolo.Common.Global;

/// <summary>
/// DI全局
/// </summary>
public static class GlobalData
{
    public static Assembly GetEntityAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "ApeVolo.Entity.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("ApeVolo.Entity.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    public static Assembly GetIBusinessAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "ApeVolo.IBusiness.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("ApeVolo.IBusiness.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    public static Assembly GetBusinessAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "ApeVolo.Business.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("ApeVolo.Business.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    public static Assembly GetRepositoryAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "ApeVolo.Repository.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("ApeVolo.Repository.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }
}
