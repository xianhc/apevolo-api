using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ApeVolo.Api.Aop;
using ApeVolo.Business.Base;
using ApeVolo.Common.ConfigOptions;
using ApeVolo.Common.DI;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.IBusiness.Base;
using ApeVolo.Repository.SugarHandler;
using ApeVolo.Repository.UnitOfWork;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Module = Autofac.Module;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// autofac注册
/// </summary>
public class AutofacRegister : Module
{
    private readonly IConfiguration _configuration;

    public AutofacRegister(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        var configs = _configuration.Get<Configs>();

        //事务 缓存 AOP
        var registerType = new List<Type>();
        if (configs.Aop.Tran.Enabled)
        {
            builder.RegisterType<TransactionAop>();
            registerType.Add(typeof(TransactionAop));
        }

        if (configs.Aop.Cache.Enabled)
        {
            builder.RegisterType<RedisAop>();
            registerType.Add(typeof(RedisAop));
        }

        builder.RegisterGeneric(typeof(SugarRepository<>)).As(typeof(ISugarRepository<>))
            .InstancePerDependency();
        builder.RegisterGeneric(typeof(BaseServices<>)).As(typeof(IBaseServices<>)).InstancePerDependency();

        //注册业务层
        builder.RegisterAssemblyTypes(GlobalData.GetBusinessAssembly())
            .AsImplementedInterfaces()
            .InstancePerDependency()
            .PropertiesAutowired()
            .EnableInterfaceInterceptors()
            .InterceptedBy(registerType.ToArray());

        // 注册仓储层
        builder.RegisterAssemblyTypes(GlobalData.GetRepositoryAssembly())
            .AsImplementedInterfaces()
            .PropertiesAutowired()
            .InstancePerDependency();

        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .PropertiesAutowired();


        //注册控制器
        var controllerBaseType = typeof(ControllerBase);
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
            .PropertiesAutowired();

        builder.RegisterType<DisposableContainer>()
            .As<IDisposableContainer>()
            .InstancePerLifetimeScope();
    }
}
