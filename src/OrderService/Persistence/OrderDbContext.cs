using CoreLib.Constant;
using CoreLib.DataAccess.Concrete;
using Domain.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence;
public class OrderDbContext : DbContextBase
{
    public OrderDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}

public class OrderDbContextDesignTimeFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<OrderDbContext>();
        builder.UseSqlServer(EnvironmentVariableProvider.DbConnectionString);
        return new OrderDbContext(builder.Options);
    }
}
