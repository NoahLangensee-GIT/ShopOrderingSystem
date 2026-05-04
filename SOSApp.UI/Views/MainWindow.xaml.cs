using System.Windows;
using Contract.Contract;
using TODOApp.UI.ViewModels;

namespace TODOApp.UI.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
    }
}