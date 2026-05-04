using Autofac;
using Contract.Contract;

namespace Business.Business;

public class BusinessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    { 
        builder.RegisterType<HandleSessionDataBase>().As<IHandleSessionDataBase>();
    }
}