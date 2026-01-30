using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Finbuckle.MultiTenant.Extensions;
using Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Startup
{
    public static IServiceCollection AddMultitenancyServices(this IServiceCollection services, IConfiguration config)
    {
        return services
            .AddDbContext<TenantDbContext>(options => options
                .UseSqlServer(config.GetConnectionString("DefaultConnection")))
            .AddMultiTenant<ABCSchoolTenantInfo>()
                .WithHeaderStrategy("tenant")
                .WithClaimStrategy("tenant")
                .WithEFCoreStore<TenantDbContext, ABCSchoolTenantInfo>()
                .Services;
    }
}
