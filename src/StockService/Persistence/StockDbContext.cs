using CoreLib.DataAccess.Concrete;
using Domain.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence;
public class StockDbContext : DbContextBase
{
    public StockDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Stock> Stocks { get; set; }
}

public class StockDbContextDesignTimeFactory : IDesignTimeDbContextFactory<StockDbContext>
{
    string connectionString = "Data Source=c_sqlserver;Initial Catalog=SagaStockDb;Persist Security Info=True;User ID=sa;Password=Test123!_";
    public StockDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<StockDbContext>();
        builder.UseSqlServer(connectionString);
        return new StockDbContext(builder.Options);
    }
}