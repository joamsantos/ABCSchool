namespace Application.Features.Identity.Roles;

public class RoleResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public IReadOnlyCollection<string> Permissions { get; set; }
}
