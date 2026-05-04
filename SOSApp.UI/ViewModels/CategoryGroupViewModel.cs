namespace SOSApp.ViewModels;

public class CategoryGroupViewModel
{
    public string CategoryName { get; set; } = string.Empty;
    public List<ProductViewModel> Products { get; set; } = [];
}