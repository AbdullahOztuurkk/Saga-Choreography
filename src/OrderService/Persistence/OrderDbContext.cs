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
    string connectionString = "Data Source=c_sqlserver;Initial Catalog=SagaOrderDb;Persist Security Info=True;User ID=sa;Password=Test123!_";
    public OrderDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<OrderDbContext>();
        builder.UseSqlServer(connectionString);
        return new OrderDbContext(builder.Options);
    }
}
