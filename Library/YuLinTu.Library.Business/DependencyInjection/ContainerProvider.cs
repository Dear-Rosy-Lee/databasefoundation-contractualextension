using Autofac;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 提供容器
    /// </summary>
    public class ContainerProvider
    {
        /// <summary>
        /// The container
        /// </summary>
        private static IContainer _Container;

        private static ContainerBuilder _Builder;

        public static ContainerBuilder Builder { get => _Builder; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public static IContainer Get()
        {
            return _Container;
        }

        public static IContainer Build()
        {
            return _Container = _Builder.Build();
        }

        /// <summary>
        /// Loads the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        public static void Load(IContainer container)
        {
            _Container = container;
        }

        public static void Load(ContainerBuilder builder)
        {
            _Builder = builder;
        }
    }
}