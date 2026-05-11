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
    private bool _isInCart;
    private int _cartQuantity;

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
            OnPropertyChanged(nameof(IsCartUpdateNeeded));
            OnPropertyChanged(nameof(CartButtonBackground));
        }
    }
    
    public bool IsInCart
    {
        get => _isInCart;
        set
        {
            _isInCart = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CartButtonText));
            OnPropertyChanged(nameof(IsCartUpdateNeeded));
            OnPropertyChanged(nameof(CartButtonBackground));
        }
    }
    
    public int CartQuantity
    {
        get => _cartQuantity;
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            _cartQuantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsCartUpdateNeeded));
            OnPropertyChanged(nameof(CartButtonBackground));
        }
    }
    
    public bool IsCartUpdateNeeded => IsInCart && Quantity != CartQuantity;
    
    public string CartButtonText => IsInCart
        ? "Warenkorb aktualisieren"
        : "In Warenkorb einfügen";

    public string CartButtonBackground => IsCartUpdateNeeded || !IsInCart
        ? "#44BD32"
        : "#718093";
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}