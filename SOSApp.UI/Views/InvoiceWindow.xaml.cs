using System.Windows;
using SOSApp.ViewModels;

namespace SOSApp.Views;

public partial class InvoiceWindow : Window
{
    public InvoiceWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        // Das Fenster nutzt das ViewModel des MainWindows als Datenquelle
        DataContext = viewModel;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true; // Signalisiert Erfolg
        Close();
    }
}