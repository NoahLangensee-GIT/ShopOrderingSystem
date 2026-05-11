using Autofac;
using SOSApp.Contract;

namespace SOSApp.Business;

public class BusinessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    { 
        builder.RegisterType<HandleSessionDataBase>().As<IHandleSessionDataBase>();
    }

    public IHandleSessionDataBase CreateSessionDataHandler()
    {
        return new HandleSessionDataBase();
    }
}