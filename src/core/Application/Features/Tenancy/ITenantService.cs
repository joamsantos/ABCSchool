namespace Application.Features.Tenancy;

public interface ITenantService
{
    Task<string> CreateTenantAsync(CreateTenantRequest createTenant, CancellationToken cancellationToken);
    Task<string> ActivateAsync(string id, CancellationToken cancellationToken);
    Task<string> DeactivateAsync(string id, CancellationToken cancellationToken);
    Task<string> UpdateSubscriptionAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TenantResponse>> GetTenantsAsync(CancellationToken cancellationToken);
    Task<TenantResponse> GetTenantByIdAsync(string id, CancellationToken cancellationToken);
}
