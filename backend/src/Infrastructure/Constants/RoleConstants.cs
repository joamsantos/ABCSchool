namespace Infrastructure.Constants;

public static class RoleConstants
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);

    public static IReadOnlyList<string> DefaultRoles => [Admin, Basic];

    public static bool IsDefaultRole(string role) => DefaultRoles.Contains(role);
}
