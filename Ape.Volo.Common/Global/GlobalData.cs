using System;
using System.IO;
using System.Reflection;

namespace Ape.Volo.Common.Global;

/// <summary>
/// DI全局
/// </summary>
public static class GlobalData
{
    public static Assembly GetEntityAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.Entity.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.Entity.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    public static Assembly GetIBusinessAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.IBusiness.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.IBusiness.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    public static Assembly GetBusinessAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.Business.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.Business.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    public static Assembly GetRepositoryAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.Repository.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.Repository.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    public static Assembly GetApiAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.Api.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.Api.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }
}
