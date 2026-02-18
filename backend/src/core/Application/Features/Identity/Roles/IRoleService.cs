namespace Application.Features.Identity.Roles;

public interface IRoleService
{
    Task<string> CreateAsync(CreateRoleRequest request);
    Task<string> UpdateAsync(UpdateRoleRequest request);
    Task<string> DeleteAsync(string id);
    Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request);
    Task<bool> DoesItExistsAsync(string name);
    Task<IReadOnlyCollection<RoleResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<RoleResponse> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<RoleResponse> GetRoleWithPermissionsAsync(string id, CancellationToken cancellationToken);
}
