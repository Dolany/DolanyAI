using Autofac;

namespace Dolany.Ai.Common
{
    public class AutofacSvc
    {
        public static IContainer Container;

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
