using FluentNHibernate.Mapping;

namespace SOSApp.Business;

internal class ProductEntityMap : ClassMap<ProductEntity>
{
    public ProductEntityMap()
    {
        Table("Product");
        Id(product => product.Id).Column("ProductId");
        Map(product => product.Name).Column("Name");
        Map(product => product.Price).Column("Price");
        Map(product => product.Category).Column("Category");
    }
}

