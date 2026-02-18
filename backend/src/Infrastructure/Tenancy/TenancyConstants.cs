namespace Infrastructure.Tenancy;

public class TenancyConstants
{
    public const string TenantIdName = "tenant";
    public const string DefaultPassword = "qwert#12345";
    public const string FirstName = "Joe";
    public const string LastName = "Doe";

    public static class Root
    {
        public const string Id = "root";
        public const string Name = "Root";
        public const string Email = "admin.root@abcschool.com";
    }
}
