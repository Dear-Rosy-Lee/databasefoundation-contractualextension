using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 工作单元注册器
    /// </summary>
    public static class UnitOfWorkInterceptorRegistrar
    {
        public static IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> RegisterInterfaceInterceptors<TLimit, TActivatorData, TSingleRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> registration, Type service,
            ProxyGenerationOptions options = null)
        {
            if (ShouldIntercept(service))
            {
                return registration.EnableInterfaceInterceptors(options).InterceptedBy(typeof(UnitOfWorkInterceptor));
            }
            else
            {
                return registration;
            }
        }

        private static bool ShouldIntercept(Type type)
        {
            return !DynamicProxyIgnoreTypes.Contains(type) && UnitOfWorkHelper.IsUnitOfWorkType(type.GetTypeInfo());
        }
    }
}