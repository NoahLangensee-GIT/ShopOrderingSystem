using Autofac;
using TODOApp.UI.ViewModels;
using TODOApp.UI.Views;

namespace TODOApp.UI;

// ReSharper disable once InconsistentNaming
public class UIModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MainWindow>().SingleInstance();
        builder.RegisterType<MainWindowViewModel>().AsSelf();
    }
}