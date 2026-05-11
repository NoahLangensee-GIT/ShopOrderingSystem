using SOSApp.Contract;

namespace SOSApp.Business;

internal class ProductEntity
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }
    public virtual decimal Price { get; set; }
    public virtual ProductCategory Category { get; set; }
}

