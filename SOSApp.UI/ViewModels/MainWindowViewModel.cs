using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SOSApp.Contract;

namespace SOSApp.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private ObservableCollection<CategoryGroupViewModel> _categoryGroups = new();
    private readonly IHandleSessionDataBase _dataBaseHandler;

    public ObservableCollection<CategoryGroupViewModel> CategoryGroups
    {
        get => _categoryGroups;
        set
        {
            _categoryGroups = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel(IHandleSessionDataBase dataBaseHandler)
    {
        _dataBaseHandler = dataBaseHandler;
        LoadProducts();
    }

    private void LoadProducts()
    {
        var productDtos = _dataBaseHandler.GetAllProducts();
        var allProducts = productDtos
            .Select(dto => new ProductViewModel(dto.Name, dto.Price, dto.Category))
            .ToList();

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