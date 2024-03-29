using CoreLib.Constant;
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
    public StockDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<StockDbContext>();
        builder.UseSqlServer(EnvironmentVariableProvider.DbConnectionString);
        return new StockDbContext(builder.Options);
    }
}