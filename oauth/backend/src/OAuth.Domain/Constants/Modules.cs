namespace OAuth.Domain.Constants;

public static class OAuthModules
{
    public const string Business = "Business";
    public const string BusinessUnit = "BusinessUnit";
    public const string Users = "Users";

    public static readonly IReadOnlySet<string> All =
        new HashSet<string> { Business, BusinessUnit, Users };
}

public static class OAuthFunctions
{
    public const string InviteUser = "InviteUser";

    public static readonly IReadOnlySet<string> All =
        new HashSet<string> { InviteUser };
}

public static class OAuthRoles
{
    public const string BusinessAdmin = "BusinessAdmin";
    public const string BusinessUnitAdmin = "BusinessUnitAdmin";
    public const string ModuleAdmin = "ModuleAdmin";
    public const string Manager = "Manager";
    public const string Reader = "Reader";
    public const string Writer = "Writer";

    public static readonly IReadOnlySet<string> All =
        new HashSet<string> { BusinessAdmin, BusinessUnitAdmin, ModuleAdmin, Manager, Reader, Writer };
}
