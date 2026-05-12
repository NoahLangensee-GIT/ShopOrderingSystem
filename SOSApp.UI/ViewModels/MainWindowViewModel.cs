using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using SOSApp.Contract;
using SOSApp.UI;
using SOSApp.Views;

namespace SOSApp.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private const decimal VatRate = 0.081m;
    private readonly IHandleSessionDataBase _dataBaseHandler;
    private readonly DispatcherTimer _statusMessageTimer = new();
    private ObservableCollection<CartItemViewModel> _cartItems = new();
    private ObservableCollection<CategoryGroupViewModel> _categoryGroups = new();
    private bool _isCartOpen;
    private string _statusMessage = string.Empty;
    private string _userEmail = string.Empty;

    public MainWindowViewModel(IHandleSessionDataBase dataBaseHandler)
    {
        _dataBaseHandler = dataBaseHandler;

        // Configuring timer
        _statusMessageTimer.Interval = TimeSpan.FromSeconds(3);
        _statusMessageTimer.Tick += (_, _) =>
        {
            _statusMessageTimer.Stop();
            StatusMessage = string.Empty;
        };

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

    public string UserEmail
    {
        get => _userEmail;
        set
        {
            _userEmail = value;
            OnPropertyChanged();
        }
    }

    // Properties
    public ObservableCollection<CategoryGroupViewModel> CategoryGroups
    {
        get => _categoryGroups;
        private set
        {
            _categoryGroups = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<CartItemViewModel> CartItems
    {
        get => _cartItems;
        set
        {
            _cartItems = value;
            OnPropertyChanged();
            RefreshCartTotals();
        }
    }

    public bool IsCartOpen
    {
        get => _isCartOpen;
        set
        {
            _isCartOpen = value;
            OnPropertyChanged();
        }
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
    private static void IncreaseProductQuantity(object? parameter)
    {
        if (parameter is not ProductViewModel product) return;
        product.Quantity++;
    }

    private static void DecreaseProductQuantity(object? parameter)
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
                ShowStatusMessage($"{product.Name} wurde entfernt.");
                RefreshCartTotals();
                return;
            }

            MessageBox.Show($"Menge für {product.Name} wählen.", "Info", MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        if (existingItem is null)
        {
            var cartItem = new CartItemViewModel(product, product.Quantity);
            cartItem.PropertyChanged += (_, _) => RefreshCartTotals();
            CartItems.Add(cartItem);
            product.IsInCart = true;
            product.CartQuantity = product.Quantity;
            ShowStatusMessage($"{cartItem.Product.Name} wurde eingefügt.");
        }
        else
        {
            existingItem.Quantity = product.Quantity;
            product.CartQuantity = product.Quantity;
            ShowStatusMessage($"{existingItem.Product.Name} wurde aktualisiert.");
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
        ShowStatusMessage($"{cartItem.Product.Name} wurde entfernt.");
        RefreshCartTotals();
    }

    private void IncreaseCartItemQuantity(object? parameter)
    {
        if (parameter is not CartItemViewModel cartItem) return;
        cartItem.Quantity++;
        cartItem.Product.Quantity = cartItem.Quantity;
        cartItem.Product.CartQuantity = cartItem.Quantity;
        ShowStatusMessage($"{cartItem.Product.Name} wurde aktualisiert.");
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
            ShowStatusMessage($"{cartItem.Product.Name} wurde aktualisiert.");
        }
        else
        {
            CartItems.Remove(cartItem);
            cartItem.Product.Quantity = 0;
            cartItem.Product.CartQuantity = 0;
            cartItem.Product.IsInCart = false;
            ShowStatusMessage($"{cartItem.Product.Name} wurde entfernt.");
        }

        RefreshCartTotals();
    }

    private void PlaceOrder(object? parameter)
    {
        if (CartItems.Count == 0)
        {
            IsCartOpen = false;
            MessageBox.Show("Bitte Produkte in den Warenkorb legen.", "Info", MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        if (string.IsNullOrWhiteSpace(UserEmail) || !IsValidEmail(UserEmail))
        {
            IsCartOpen = false;
            MessageBox.Show("Bitte gültige E-Mail eingeben.", "Eingabe fehlt", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            IsCartOpen = true;
            return;
        }

        IsCartOpen = false;
        var invoiceWindow = new InvoiceWindow(this);
        if (Application.Current.MainWindow != null) invoiceWindow.Owner = Application.Current.MainWindow;

        if (invoiceWindow.ShowDialog() != true) return;

        foreach (var group in CategoryGroups)
        foreach (var product in group.Products)
        {
            product.Quantity = 0;
            product.CartQuantity = 0;
            product.IsInCart = false;
        }

        CartItems.Clear();
        RefreshCartTotals();
        ShowStatusMessage("Bestellung erfolgreich abgeschlossen.");
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var address = new MailAddress(email);

            if (address.Address != email) return false;

            var hostParts = address.Host.Split('.');

            return hostParts.Length >= 2
                   && hostParts.All(part => part.Length > 0)
                   && hostParts[^1].Length >= 2;
        }
        catch
        {
            return false;
        }
    }

    private void RefreshCartTotals()
    {
        OnPropertyChanged(nameof(TotalGrossPrice));
        OnPropertyChanged(nameof(TotalNetPrice));
        OnPropertyChanged(nameof(TotalVatPrice));
    }

    private void ShowStatusMessage(string message)
    {
        StatusMessage = message;
        _statusMessageTimer.Stop();
        _statusMessageTimer.Start();
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

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}