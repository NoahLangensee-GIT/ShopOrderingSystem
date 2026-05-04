using System.Windows;
using Autofac;
using Business.Business;
using TODOApp.UI;
using TODOApp.UI.Views;

namespace TODO_App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<BusinessModule>();
        builder.RegisterModule<UIModule>();
        var container = builder.Build();
        container.Resolve<MainWindow>().Show();
        base.OnStartup(e);
    }
}