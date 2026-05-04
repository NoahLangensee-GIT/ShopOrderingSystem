using Autofac;
using SOSApp.ViewModels;
using SOSApp.Views;

namespace SOSApp;

// ReSharper disable once InconsistentNaming
public class UIModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MainWindow>().SingleInstance();
        builder.RegisterType<MainWindowViewModel>().AsSelf();
    }
}