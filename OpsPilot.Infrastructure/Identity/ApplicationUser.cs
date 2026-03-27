using Microsoft.AspNetCore.Identity;

namespace OpsPilot.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }
}