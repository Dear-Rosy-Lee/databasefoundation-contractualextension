using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YuLinTu.Library.Business
{
    public static class AutofacRegistration
    {
        public static void TryResolveNamed<TService>(this IComponentContext context, string serviceName, string pluginName, Action<TService> action)
        {
            var name = ResolveServiceName(serviceName, pluginName);
            if (context.IsRegisteredWithName<TService>(name))
                action.Invoke(context.ResolveNamed<TService>(name));
        }

        public static TService ResolveNamed<TService>(this IComponentContext context, string serviceName, string pluginName)
        {
            var name = ResolveServiceName(serviceName, pluginName);
            if (context.IsRegisteredWithName<TService>(name))
                return context.ResolveNamed<TService>(name);

            return context.Resolve<TService>();
        }

        public static TService ResolveNamed<TService>(this IComponentContext context, string serviceName, string pluginName, IEnumerable<Parameter> parameters)
        {
            var name = ResolveServiceName(serviceName, pluginName);
            if (context.IsRegisteredWithName<TService>(name))
                return context.ResolveNamed<TService>(name, parameters);

            return context.Resolve<TService>(parameters);
        }

        public static TService ResolveNamed<TService>(this IComponentContext context, string serviceName, string pluginName, params Parameter[] parameters)
        {
            var name = ResolveServiceName(serviceName, pluginName);
            if (context.IsRegisteredWithName<TService>(name))
                return context.ResolveNamed<TService>(name, parameters);

            return context.Resolve<TService>(parameters);
        }

        private static string ResolveServiceName(string serviceName, string pluginName)
        {
            return pluginName + serviceName;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assembly"></param>
        public static void Register(this ContainerBuilder builder, Assembly assembly)
        {
            var types = assembly
                    .GetTypes()
                    .Where(type => type != null && type.IsClass && !type.IsAbstract);

            foreach (var type in types)
            {
                // TODO: 优化服务注册
                var interfaces = type.GetInterfaces();
                // 名称匹配
                var service = interfaces.Where(x => x.Name.EndsWith(type.Name)).FirstOrDefault();
                // 生命周期
                var lifeTime = GetServiceLifetimeFromClassHierarchy(type);

                var namedAtt = type.GetAttribute<NamedAttribute>();
                if (service is null && lifeTime is null && namedAtt is null)
                {
                    continue;
                }

                if (service == null)
                    service = interfaces[0];

                if (type.IsGenericTypeDefinition)
                {
                    builder
                        .RegisterGeneric(type)
                        .Named(namedAtt, service)
                        .As(service)
                        .ConfigureLifecycle(lifeTime)
                        .RegisterInterfaceInterceptors(service);
                }
                else
                {
                    builder
                        .RegisterType(type)
                        .Named(namedAtt, service)
                        .As(service)
                        .ConfigureLifecycle(lifeTime)
                        .RegisterInterfaceInterceptors(service);
                }
            }
        }

        private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> Named<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
            NamedAttribute namedAttribute, Type serviceType)
        {
            if (namedAttribute is null || namedAttribute.Name.IsNullOrBlank())
                return registrationBuilder;

            return registrationBuilder.Named(namedAttribute.Name, serviceType);
        }

        /// <summary>
        /// 配置服务生命周期
        /// </summary>
        /// <typeparam name="TActivatorData"></typeparam>
        /// <typeparam name="TRegistrationStyle"></typeparam>
        /// <param name="registrationBuilder"></param>
        /// <param name="lifecycleKind"></param>
        /// <returns></returns>
        private static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> ConfigureLifecycle<TActivatorData, TRegistrationStyle>(
           this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder,
           ServiceLifetime? lifecycleKind)
        {
            if (!lifecycleKind.HasValue)
            {
                registrationBuilder.InstancePerDependency();
                return registrationBuilder;
            }

            switch (lifecycleKind.Value)
            {
                case ServiceLifetime.Singleton:
                    registrationBuilder.SingleInstance();
                    break;

                case ServiceLifetime.Scoped:
                    registrationBuilder.InstancePerLifetimeScope();
                    break;

                case ServiceLifetime.Transient:
                    registrationBuilder.InstancePerDependency();
                    break;
            }

            return registrationBuilder;
        }

        /// <summary>
        /// 获取服务生命周期
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ServiceLifetime? GetServiceLifetimeFromClassHierarchy(Type type)
        {
            if (typeof(ITransientDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Transient;
            }

            if (typeof(ISingletonDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Singleton;
            }

            if (typeof(IScopedDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Scoped;
            }

            return null;
        }
    }
}