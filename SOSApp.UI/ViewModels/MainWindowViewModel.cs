using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SOSApp.Contract;

namespace SOSApp.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private ObservableCollection<CategoryGroupViewModel> _categoryGroups = new();

    public ObservableCollection<CategoryGroupViewModel> CategoryGroups
    {
        get => _categoryGroups;
        set
        {
            _categoryGroups = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel()
    {
        LoadProducts();
    }

    private void LoadProducts()
    {
        var allProducts = new List<ProductViewModel>
        {
            new ProductViewModel("Laptop X", 999.00m, ProductCategory.Elektronik),
            new ProductViewModel("Smartphone Y", 599.50m, ProductCategory.Elektronik),
            new ProductViewModel("T-Shirt Basic", 15.99m, ProductCategory.Kleidung),
            new ProductViewModel("Jeans Slim", 49.95m, ProductCategory.Kleidung),
            new ProductViewModel("Winterjacke", 120.00m, ProductCategory.Kleidung),
            new ProductViewModel("Apfel", 0.99m, ProductCategory.Lebensmittel),
            new ProductViewModel("Brot", 2.49m, ProductCategory.Lebensmittel)
        };

        var grouped = allProducts
            .GroupBy(p => p.Category)
            .Select(g => new CategoryGroupViewModel
            {
                CategoryName = g.Key.ToString(),
                Products = g.ToList()
            });

        CategoryGroups = new ObservableCollection<CategoryGroupViewModel>(grouped);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}