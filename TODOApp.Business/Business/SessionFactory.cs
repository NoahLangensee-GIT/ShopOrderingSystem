using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Business.Business;

internal class SessionFactory
{
    public ISession OpenSession()
    {
        var sessionFactory = Fluently.Configure()
            .Database(MySQLConfiguration.Standard
                .ConnectionString("Server=localhost;Port=3307;Database=todo;Uid=root;Pwd=Test;")
                .ShowSql())
            .Mappings(configuration => configuration.FluentMappings
                .AddFromAssemblyOf<TaskEntity>()
                )
            .BuildSessionFactory();
    
        return sessionFactory.OpenSession();
    }
}