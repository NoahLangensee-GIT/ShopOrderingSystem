using NHibernate;
using SOSApp.Contract;

namespace SOSApp.Business;

internal class HandleSessionDataBase : IHandleSessionDataBase
{
    private readonly ISession _currentSession;

    public HandleSessionDataBase()
    {
        var sessionFactory1 = new SessionFactory();
        _currentSession = sessionFactory1.OpenSession();
    }

    public List<ProductDto> GetAllProducts()
    {
        return _currentSession.Query<ProductEntity>()
            .Select(entity => new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                Category = entity.Category
            }).ToList();
    }
}