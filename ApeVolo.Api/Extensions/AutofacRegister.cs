using System;
using System.Collections.Generic;
using System.Linq;
using ApeVolo.Api.Aop;
using ApeVolo.Common.DI;
using ApeVolo.Common.Global;
using Autofac;
using Autofac.Extras.DynamicProxy;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// autofac注册
/// </summary>
public class AutofacRegister : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        //事务 缓存 AOP
        var cacheType = new List<Type>();
        builder.RegisterType<TransactionAop>();
        cacheType.Add(typeof(TransactionAop));
        builder.RegisterType<RedisAop>();
        cacheType.Add(typeof(RedisAop));

        // 获取所有待注入服务类
        var dependencyService = typeof(IDependencyService);
        var dependencyServiceArray = GlobalData.FxAllTypes
            .Where(x => dependencyService.IsAssignableFrom(x) && x != dependencyService).ToArray();
        builder.RegisterTypes(dependencyServiceArray)
            .AsImplementedInterfaces()
            .PropertiesAutowired()
            .InstancePerDependency()
            .EnableInterfaceInterceptors()
            .InterceptedBy(cacheType.ToArray());


        // 获取所有待注入仓储类
        var dependencyRepository = typeof(IDependencyRepository);
        var dependencyRepositoryArray = GlobalData.FxAllTypes
            .Where(x => dependencyRepository.IsAssignableFrom(x) && x != dependencyRepository).ToArray();
        builder.RegisterTypes(dependencyRepositoryArray)
            .AsImplementedInterfaces()
            .InstancePerDependency();

        //控制器
        /*var controllerBaseType = typeof(ControllerBase);
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
            .PropertiesAutowired();*/

        builder.RegisterType<DisposableContainer>()
            .As<IDisposableContainer>()
            .InstancePerLifetimeScope();
    }
}