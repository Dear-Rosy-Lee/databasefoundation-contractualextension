using System.Linq;
using System.Reflection;

namespace YuLinTu.Library.Business
{
    public static class UnitOfWorkHelper
    {
        public static bool IsUnitOfWorkType(TypeInfo implementationType)
        {
            //Explicitly defined UnitOfWorkAttribute
            if (HasUnitOfWorkAttribute(implementationType) || AnyMethodHasUnitOfWorkAttribute(implementationType))
            {
                return true;
            }

            //Conventional classes
            if (typeof(IUnitOfWorkEnabled).GetTypeInfo().IsAssignableFrom(implementationType))
            {
                return true;
            }

            return false;
        }

        public static bool IsUnitOfWorkMethod(MethodInfo methodInfo, out UnitOfWorkAttribute unitOfWorkAttribute)
        {
            Check.NotNull(methodInfo, nameof(methodInfo));

            //Method declaration
            var attrs = methodInfo.GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
            if (attrs.Any())
            {
                unitOfWorkAttribute = attrs.First();
                return !unitOfWorkAttribute.IsDisabled;
            }

            if (methodInfo.DeclaringType != null)
            {
                //Class declaration
                attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
                if (attrs.Any())
                {
                    unitOfWorkAttribute = attrs.First();
                    return !unitOfWorkAttribute.IsDisabled;
                }

                //Conventional classes
                if (typeof(IUnitOfWorkEnabled).GetTypeInfo().IsAssignableFrom(methodInfo.DeclaringType))
                {
                    unitOfWorkAttribute = null;
                    return true;
                }
            }

            unitOfWorkAttribute = null;
            return false;
        }

        private static bool AnyMethodHasUnitOfWorkAttribute(TypeInfo implementationType)
        {
            return implementationType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(HasUnitOfWorkAttribute);
        }

        private static bool HasUnitOfWorkAttribute(MemberInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(UnitOfWorkAttribute), true);
        }
    }
}