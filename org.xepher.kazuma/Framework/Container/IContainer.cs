using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Container
{
    public interface IContainer
    {
        void Register<TClass>();
        void Register<TService, TClass>() where TClass : TService;
        void RegisterInstance<TService>(TService instance);

        TService Resolve<TService>();
        object Resolve(Type type);
    }
}
