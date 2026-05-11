using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SOSApp.ViewModels;

public class CartItemViewModel : INotifyPropertyChanged
{
    private int _quantity;

    public ProductViewModel Product { get; }

    public string Name => Product.Name;
    public decimal UnitGrossPrice => Product.Price;

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value < 1)
            {
                value = 1;
            }

            _quantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalGrossPrice));
            OnPropertyChanged(nameof(TotalNetPrice));
        }
    }

    public decimal TotalGrossPrice => UnitGrossPrice * Quantity;

    public decimal TotalNetPrice => TotalGrossPrice / 1.081m;

    public CartItemViewModel(ProductViewModel product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}