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
                .ConnectionString("Server=localhost;Port=3307;Database=sos;Uid=root;Pwd=Test;")
                .ShowSql())
            .Mappings(configuration => configuration.FluentMappings
                .AddFromAssemblyOf<TaskEntity>()
                )
            .ExposeConfiguration(cfg => cfg.SetProperty(NHibernate.Cfg.Environment.Hbm2ddlAuto, "update"))
            .BuildSessionFactory();
    
        return sessionFactory.OpenSession();
    }
}