using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApeVolo.Common.Global
{
    /// <summary>
    /// DI全局
    /// </summary>
    public static class GlobalData
    {
        static readonly List<string> FxAssemblies = new List<string> { "ApeVolo.Repository", "ApeVolo.Business", "ApeVolo.IBusiness", "ApeVolo.Entity" };
        static readonly List<string> EntityAssemblys = new List<string> { "ApeVolo.Entity" };
        static GlobalData()
        {
            var assemblys = FxAssemblies.Select(x => Assembly.Load(x)).ToList();
            List<Type> allTypes = new List<Type>();
            assemblys.ForEach(aAssembly =>
            {
                allTypes.AddRange(aAssembly.GetTypes());
            });
            FxAllTypes = allTypes;


            //实体
            var entityAssemblys = EntityAssemblys.Select(x => Assembly.Load(x)).ToList();
            List<Type> entityTypes = new List<Type>();
            entityAssemblys.ForEach(entityAssembly =>
            {
                entityTypes.AddRange(entityAssembly.GetTypes());
            });
            EntityTypes = entityTypes;
        }

        /// <summary>
        /// 框架所有自定义类
        /// </summary>
        public static readonly List<Type> FxAllTypes;

        /// <summary>
        /// 所有实体类
        /// </summary>
        public static readonly List<Type> EntityTypes;
    }
}
