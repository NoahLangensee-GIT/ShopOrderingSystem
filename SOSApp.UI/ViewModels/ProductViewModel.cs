using System.ComponentModel;
using System.Runtime.CompilerServices;
using SOSApp.Contract;

namespace SOSApp.ViewModels;

public class ProductViewModel : INotifyPropertyChanged
{
    private ProductCategory _category;
    private string _name = null!;
    private decimal _price;
    private int _quantity;

    public ProductViewModel(string name, decimal price, ProductCategory category)
    {
        Name = name;
        Price = price;
        Category = category;
    }


    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public decimal Price
    {
        get => _price;
        set { _price = value; OnPropertyChanged(); }
    }

    public ProductCategory Category
    {
        get => _category;
        set { _category = value; OnPropertyChanged(); }
    }
    
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            _quantity = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}