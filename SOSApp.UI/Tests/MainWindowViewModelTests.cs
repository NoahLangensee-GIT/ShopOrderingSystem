using System.Collections.Generic;
using System.Linq;
using Moq;
using SOSApp.Contract;
using SOSApp.ViewModels;
using Xunit;

namespace SOSApp.Tests;

public class MainWindowViewModelTests
{
    private readonly Mock<IHandleSessionDataBase> _dbHandlerMock;
    private readonly List<ProductDto> _testProducts;

    public MainWindowViewModelTests()
    {
        _dbHandlerMock = new Mock<IHandleSessionDataBase>();

        _testProducts = new List<ProductDto>
        {
            new() { Id = 1, Name = "Laptop Dell XPS", Price = 1299.99m, Category = ProductCategory.Elektronik },
            new() { Id = 2, Name = "Wireless Maus Logitech", Price = 45.99m, Category = ProductCategory.Elektronik },
            new() { Id = 6, Name = "T-Shirt weiß", Price = 19.99m, Category = ProductCategory.Kleidung },
            new() { Id = 11, Name = "Kaffee Arabica 500g", Price = 8.99m, Category = ProductCategory.Lebensmittel }
        };

        _dbHandlerMock.Setup(db => db.GetAllProducts()).Returns(_testProducts);
    }

    private MainWindowViewModel CreateViewModel()
    {
        return new MainWindowViewModel(_dbHandlerMock.Object);
    }

    [Fact]
    public void Constructor_ShouldLoadAndGroupProductsCorrectly()
    {
        // Arrange & Act
        var vm = CreateViewModel();

        // Assert
        Assert.NotNull(vm.CategoryGroups);
        Assert.Equal(3, vm.CategoryGroups.Count); 
        
        var elektronikGruppe = vm.CategoryGroups.FirstOrDefault(g => g.CategoryName == ProductCategory.Elektronik.ToString());
        Assert.NotNull(elektronikGruppe);
        Assert.Equal(2, elektronikGruppe.Products.Count);
    }

    [Fact]
    public void IncreaseProductQuantityCommand_ShouldIncreaseQuantity()
    {
        // Arrange
        var vm = CreateViewModel();
        var product = new ProductViewModel("Laptop Dell XPS", 1299.99m, ProductCategory.Elektronik) { Quantity = 0 };

        // Act
        vm.IncreaseProductQuantityCommand.Execute(product);

        // Assert
        Assert.Equal(1, product.Quantity);
    }

    [Fact]
    public void DecreaseProductQuantityCommand_ShouldNotGoBelowZero()
    {
        // Arrange
        var vm = CreateViewModel();
        var product = new ProductViewModel("T-Shirt weiß", 19.99m, ProductCategory.Kleidung) { Quantity = 0 };

        // Act
        vm.DecreaseProductQuantityCommand.Execute(product);

        // Assert
        Assert.Equal(0, product.Quantity);
    }

    [Fact]
    public void AddToCartCommand_ShouldAddProduct_WhenQuantityIsPositive()
    {
        // Arrange
        var vm = CreateViewModel();
        var product = new ProductViewModel("Kaffee Arabica 500g", 8.99m, ProductCategory.Lebensmittel) { Quantity = 2 };

        // Act
        vm.AddToCartCommand.Execute(product);

        // Assert
        Assert.Single(vm.CartItems);
        var cartItem = vm.CartItems.First();
        Assert.Equal("Kaffee Arabica 500g", cartItem.Product.Name);
        Assert.Equal(2, cartItem.Quantity);
        Assert.True(vm.IsCartOpen);
    }

    [Fact]
    public void CartTotals_ShouldCalculateGrossNetAndVatCorrectly()
    {
        // Arrange
        var vm = CreateViewModel();
        var product = new ProductViewModel("Wireless Maus Logitech", 108.10m, ProductCategory.Elektronik) { Quantity = 1 };

        // Act
        vm.AddToCartCommand.Execute(product);

        // Assert
        Assert.Equal(108.10m, vm.TotalGrossPrice);
        Assert.Equal(100.00m, vm.TotalNetPrice);
        Assert.Equal(8.10m, vm.TotalVatPrice);
    }

    [Fact]
    public void RemoveFromCartCommand_ShouldClearCartItemAndResetProduct()
    {
        // Arrange
        var vm = CreateViewModel();
        var product = new ProductViewModel("T-Shirt weiß", 19.99m, ProductCategory.Kleidung) { Quantity = 1 };
        vm.AddToCartCommand.Execute(product);
        var cartItem = vm.CartItems.First();

        // Act
        vm.RemoveFromCartCommand.Execute(cartItem);

        // Assert
        Assert.Empty(vm.CartItems);
        Assert.Equal(0, product.Quantity);
        Assert.False(product.IsInCart);
    }
}
