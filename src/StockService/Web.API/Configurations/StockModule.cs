using Application.Services.Concrete;
using Autofac;
using CoreLib.Constant;
using CoreLib.DataAccess;
using Persistence;
using Web.API.Controllers;

namespace Web.API.Configurations;

public class StockModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(MainApiController).Assembly).Where(t => t.IsSubclassOf(typeof(MainApiController))).PropertiesAutowired();

        builder.RegisterAssemblyTypes(typeof(BaseService).Assembly)
            .Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces()
            .PropertiesAutowired()
            .InstancePerLifetimeScope();

        builder.AddUnitOfWorkContext<StockDbContext>(EnvironmentVariableProvider.DbConnectionString);
    }
}
