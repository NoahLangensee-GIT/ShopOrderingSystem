using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SOSApp.Contract;
using SOSApp.UI;

namespace SOSApp.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private const decimal VatRate = 0.081m;
    private readonly IHandleSessionDataBase _dataBaseHandler;
    private ObservableCollection<CartItemViewModel> _cartItems = new();
    private ObservableCollection<CategoryGroupViewModel> _categoryGroups = new();
    private bool _isCartOpen;
    private string _statusMessage = string.Empty;
    private string _userEmail = string.Empty;

    public string UserEmail 
    {
        get => _userEmail;
        set { _userEmail = value; OnPropertyChanged(); }
    }

    public MainWindowViewModel(IHandleSessionDataBase dataBaseHandler)
    {
        _dataBaseHandler = dataBaseHandler;

        // Fix: Alle Commands nutzen jetzt die korrekte Signatur oder Lambda-Ausdrücke
        AddToCartCommand = new RelayCommand(AddToCart);
        OpenCartCommand = new RelayCommand(_ => IsCartOpen = true);
        CloseCartCommand = new RelayCommand(_ => IsCartOpen = false);
        RemoveFromCartCommand = new RelayCommand(RemoveFromCart);
        IncreaseCartItemQuantityCommand = new RelayCommand(IncreaseCartItemQuantity);
        DecreaseCartItemQuantityCommand = new RelayCommand(DecreaseCartItemQuantity);
        IncreaseProductQuantityCommand = new RelayCommand(IncreaseProductQuantity);
        DecreaseProductQuantityCommand = new RelayCommand(DecreaseProductQuantity);
        PlaceOrderCommand = new RelayCommand(PlaceOrder);

        LoadProducts();
    }

    // Properties
    public ObservableCollection<CategoryGroupViewModel> CategoryGroups
    {
        get => _categoryGroups;
        set { _categoryGroups = value; OnPropertyChanged(); }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public ObservableCollection<CartItemViewModel> CartItems
    {
        get => _cartItems;
        set { _cartItems = value; OnPropertyChanged(); RefreshCartTotals(); }
    }

    public bool IsCartOpen
    {
        get => _isCartOpen;
        set { _isCartOpen = value; OnPropertyChanged(); }
    }

    public decimal TotalGrossPrice => CartItems.Sum(item => item.TotalGrossPrice);
    public decimal TotalNetPrice => TotalGrossPrice / (1 + VatRate);
    public decimal TotalVatPrice => TotalGrossPrice - TotalNetPrice;

    // Commands
    public ICommand AddToCartCommand { get; }
    public ICommand OpenCartCommand { get; }
    public ICommand CloseCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand IncreaseCartItemQuantityCommand { get; }
    public ICommand DecreaseCartItemQuantityCommand { get; }
    public ICommand IncreaseProductQuantityCommand { get; }
    public ICommand DecreaseProductQuantityCommand { get; }
    public ICommand PlaceOrderCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    // Methoden mit korrigierter Nullable-Signatur (object? parameter)
    private void IncreaseProductQuantity(object? parameter)
    {
        if (parameter is not ProductViewModel product) return;
        product.Quantity++;
    }

    private void DecreaseProductQuantity(object? parameter)
    {
        if (parameter is not ProductViewModel product) return;
        if (product.Quantity <= 0) return;
        product.Quantity--;
    }

    private void AddToCart(object? parameter)
    {
        if (parameter is not ProductViewModel product) return;
        var existingItem = CartItems.FirstOrDefault(item => item.Product == product);

        if (product.Quantity <= 0)
        {
            if (existingItem is not null)
            {
                CartItems.Remove(existingItem);
                product.IsInCart = false;
                product.CartQuantity = 0;
                StatusMessage = $"{product.Name} wurde entfernt.";
                RefreshCartTotals();
                return;
            }
            MessageBox.Show($"Menge für {product.Name} wählen.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (existingItem is null)
        {
            var cartItem = new CartItemViewModel(product, product.Quantity);
            cartItem.PropertyChanged += (_, _) => RefreshCartTotals();
            CartItems.Add(cartItem);
            product.IsInCart = true;
            product.CartQuantity = product.Quantity;
        }
        else
        {
            existingItem.Quantity = product.Quantity;
            product.CartQuantity = product.Quantity;
        }

        RefreshCartTotals();
        IsCartOpen = true;
    }

    private void RemoveFromCart(object? parameter)
    {
        if (parameter is not CartItemViewModel cartItem) return;
        CartItems.Remove(cartItem);
        cartItem.Product.Quantity = 0;
        cartItem.Product.CartQuantity = 0;
        cartItem.Product.IsInCart = false;
        RefreshCartTotals();
    }

    private void IncreaseCartItemQuantity(object? parameter)
    {
        if (parameter is not CartItemViewModel cartItem) return;
        cartItem.Quantity++;
        cartItem.Product.Quantity = cartItem.Quantity;
        cartItem.Product.CartQuantity = cartItem.Quantity;
        RefreshCartTotals();
    }

    private void DecreaseCartItemQuantity(object? parameter)
    {
        if (parameter is not CartItemViewModel cartItem) return;
        if (cartItem.Quantity > 1)
        {
            cartItem.Quantity--;
            cartItem.Product.Quantity = cartItem.Quantity;
            cartItem.Product.CartQuantity = cartItem.Quantity;
        }
        else
        {
            CartItems.Remove(cartItem);
            cartItem.Product.Quantity = 0;
            cartItem.Product.CartQuantity = 0;
            cartItem.Product.IsInCart = false;
        }
        RefreshCartTotals();
    }

    private void PlaceOrder(object? parameter)
    {
        if (string.IsNullOrWhiteSpace(UserEmail) || !UserEmail.Contains("@"))
        {
            IsCartOpen = false;
            MessageBox.Show("Bitte gültige E-Mail eingeben.", "Eingabe fehlt", MessageBoxButton.OK, MessageBoxImage.Warning);
            IsCartOpen = true;
            return;
        }

        if (CartItems == null || CartItems.Count == 0) return;

        IsCartOpen = false;
        var invoiceWindow = new SOSApp.Views.InvoiceWindow(this);
        if (Application.Current.MainWindow != null) invoiceWindow.Owner = Application.Current.MainWindow;

        if (invoiceWindow.ShowDialog() == true)
        {
            foreach (var group in CategoryGroups)
            {
                foreach (var product in group.Products)
                {
                    product.Quantity = 0;
                    product.CartQuantity = 0;
                    product.IsInCart = false;
                }
            }
            CartItems.Clear();
            RefreshCartTotals(); 
            StatusMessage = "Bestellung erfolgreich abgeschlossen.";
        }
    }

    private void RefreshCartTotals()
    {
        OnPropertyChanged(nameof(TotalGrossPrice));
        OnPropertyChanged(nameof(TotalNetPrice));
        OnPropertyChanged(nameof(TotalVatPrice));
    }

    private void LoadProducts()
    {
        var productDtos = _dataBaseHandler.GetAllProducts();
        var grouped = productDtos
            .Select(dto => new ProductViewModel(dto.Name, dto.Price, dto.Category))
            .GroupBy(p => p.Category)
            .Select(g => new CategoryGroupViewModel { CategoryName = g.Key.ToString(), Products = g.ToList() });

        CategoryGroups = new ObservableCollection<CategoryGroupViewModel>(grouped);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}