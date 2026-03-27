namespace OpsPilot.Domain.Constants;

public static class SystemRoles
{
    public const string Employee = "Employee";
    public const string Manager = "Manager";
    public const string Hr = "HR";
    public const string ItSupport = "IT Support";
    public const string Finance = "Finance";
    public const string Admin = "Admin";
    public const string SuperAdmin = "SuperAdmin";

    public static readonly string[] All =
    [
        Employee,
        Manager,
        Hr,
        ItSupport,
        Finance,
        Admin,
        SuperAdmin
    ];

    public static readonly string[] Elevated =
    [
        Manager,
        Hr,
        ItSupport,
        Finance,
        Admin,
        SuperAdmin
    ];
}