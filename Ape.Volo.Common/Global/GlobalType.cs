using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ape.Volo.Common.Global;

/// <summary>
/// 程序集类型
/// </summary>
public static class GlobalType
{
    //public static readonly List<Type> AllType;
    public static readonly List<Type> ApiTypes;
    public static readonly List<Type> CommonTypes;
    public static readonly List<Type> BusinessInterfaceTypes;
    public static readonly List<Type> BusinessTypes;
    public static readonly List<Type> RepositoryTypes;
    public static readonly List<Type> QuartzNetServiceTypes;
    public static readonly List<Type> EntityTypes;
    public static readonly List<Type> EventBusTypes;


    static GlobalType()
    {
        ApiTypes = GetApiAssembly().GetTypes().Where(u => u.IsPublic).ToList();
        CommonTypes = GetCommonAssembly().GetTypes().Where(u => u.IsPublic).ToList();
        BusinessInterfaceTypes = GetIBusinessAssembly().GetTypes().Where(u => u.IsPublic).ToList();
        BusinessTypes = GetBusinessAssembly().GetTypes().Where(u => u.IsPublic).ToList();
        RepositoryTypes = GetRepositoryAssembly().GetTypes().Where(u => u.IsPublic).ToList();
        QuartzNetServiceTypes = GetQuartzNetServiceAssembly().GetTypes().Where(u => u.IsPublic).ToList();
        EntityTypes = GetEntityAssembly().GetTypes().Where(u => u.IsPublic).ToList();
        EventBusTypes = GetEventBusAssembly().GetTypes().Where(u => u.IsPublic).ToList();
    }


    static Assembly GetEntityAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.Entity.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.Entity.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    static Assembly GetIBusinessAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.IBusiness.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.IBusiness.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    static Assembly GetBusinessAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.Business.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.Business.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    static Assembly GetRepositoryAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.Repository.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.Repository.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    static Assembly GetApiAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.Api.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.Api.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    static Assembly GetCommonAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.Common.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.Common.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }

    static Assembly GetQuartzNetServiceAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.QuartzNetService.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.QuartzNetService.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }


    static Assembly GetEventBusAssembly()
    {
        var basePath = AppContext.BaseDirectory;
        var dllFile = Path.Combine(basePath, "Ape.Volo.EventBus.dll");
        if (!File.Exists(dllFile))
        {
            throw new System.Exception("Ape.Volo.EventBus.dll文件未生成,编译项目成功后重试！");
        }

        return Assembly.LoadFrom(dllFile);
    }
}
