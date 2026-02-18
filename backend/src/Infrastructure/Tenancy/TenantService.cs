using Application.Features.Tenancy;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Contexts;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tenancy;

public class TenantService : ITenantService
{
    private readonly IMultiTenantStore<ABCSchoolTenantInfo> _tenantStore;
    private readonly ApplicationDbSeeder _dbSeeder;
    private readonly IServiceProvider _serviceProvider;

    public TenantService(
        IMultiTenantStore<ABCSchoolTenantInfo> tenantStore,
        ApplicationDbSeeder dbSeeder,
        IServiceProvider serviceProvider)
    {
        _tenantStore = tenantStore;
        _dbSeeder = dbSeeder;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> ActivateAsync(string id, CancellationToken cancellationToken)
    {
        var tenantInfo = await _tenantStore.GetAsync(id);
        tenantInfo.IsActive = true;

        await _tenantStore.UpdateAsync(tenantInfo);
        return tenantInfo.Identifier;
    }

    public async Task<string> CreateTenantAsync(CreateTenantRequest createTenant, CancellationToken cancellationToken)
    {
        var newTenant = new ABCSchoolTenantInfo
        {
            Id = createTenant.Identifier,
            Identifier = createTenant.Identifier,
            Name = createTenant.Name,
            IsActive = createTenant.IsActive,
            ConnectionString = createTenant.ConnectionString,
            Email = createTenant.Email,
            FirstName = createTenant.FirstName,
            LastName = createTenant.LastName,
            ValidUpTo = createTenant.ValidUpTo
        };

        await _tenantStore.AddAsync(newTenant);

        using var scope = _serviceProvider.CreateScope();

        _serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
            .MultiTenantContext = new MultiTenantContext<ABCSchoolTenantInfo>(newTenant);

        await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>()
            .InitializeDatabaseAsync(cancellationToken);

        return newTenant.Identifier;
    }

    public async Task<string> DeactivateAsync(string id, CancellationToken cancellationToken)
    {
        var tenantInDb = await _tenantStore.GetAsync(id);
        tenantInDb.IsActive = false;

        await _tenantStore.UpdateAsync(tenantInDb);
        return tenantInDb.Identifier;
    }

    public async Task<TenantResponse> GetTenantByIdAsync(string id, CancellationToken cancellationToken)
    {
        var tenantInDb = await _tenantStore.GetAsync(id);

        return tenantInDb.Adapt<TenantResponse>();
    }

    public async Task<IReadOnlyCollection<TenantResponse>> GetTenantsAsync(CancellationToken cancellationToken)
    {
        var tenantsInDb = await _tenantStore.GetAllAsync();
        return tenantsInDb.Adapt<IReadOnlyCollection<TenantResponse>>();
    }

    public async Task<string> UpdateSubscriptionAsync(
        UpdateTenantSubscriptionRequest updateTenantSubscription, 
        CancellationToken cancellationToken)
    {
        var tenantInDb = await _tenantStore.GetAsync(updateTenantSubscription.TenantId);

        tenantInDb.ValidUpTo = updateTenantSubscription.NewExpiryDate;

        await _tenantStore.UpdateAsync(tenantInDb);

        return tenantInDb.Identifier;
    }
}
