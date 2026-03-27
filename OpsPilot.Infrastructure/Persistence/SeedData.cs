using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpsPilot.Domain.Constants;
using OpsPilot.Domain.Entities;
using OpsPilot.Infrastructure.Identity;

namespace OpsPilot.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(
        IServiceProvider serviceProvider,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        var context = serviceProvider.GetRequiredService<OpsPilotDbContext>();
        await context.Database.MigrateAsync();

        foreach (var role in SystemRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = await EnsureUserAsync(userManager, "admin@opspilot.local", "Admin@123", "System Admin");
        var manager = await EnsureUserAsync(userManager, "manager@opspilot.local", "Manager@123", "Ops Manager");
        var employee = await EnsureUserAsync(userManager, "employee@opspilot.local", "Employee@123", "Team Employee");
        var itSupport = await EnsureUserAsync(userManager, "itsupport@opspilot.local", "Support@123", "IT Support Agent");
        var finance = await EnsureUserAsync(userManager, "finance@opspilot.local", "Finance@123", "Finance Officer");
        var hr = await EnsureUserAsync(userManager, "hr@opspilot.local", "Hrteam@123", "HR Officer");

        await EnsureRoleAsync(userManager, admin, SystemRoles.Admin);
        await EnsureRoleAsync(userManager, admin, SystemRoles.SuperAdmin);
        await EnsureRoleAsync(userManager, manager, SystemRoles.Manager);
        await EnsureRoleAsync(userManager, employee, SystemRoles.Employee);
        await EnsureRoleAsync(userManager, itSupport, SystemRoles.ItSupport);
        await EnsureRoleAsync(userManager, finance, SystemRoles.Finance);
        await EnsureRoleAsync(userManager, hr, SystemRoles.Hr);

        if (!await context.Departments.AnyAsync())
        {
            context.Departments.AddRange(
                new Department { Name = "Operations", Code = "OPS", ManagerUserId = manager.Id, CreatedBy = admin.Id },
                new Department { Name = "Information Technology", Code = "IT", ManagerUserId = manager.Id, CreatedBy = admin.Id },
                new Department { Name = "Human Resources", Code = "HR", ManagerUserId = hr.Id, CreatedBy = admin.Id },
                new Department { Name = "Finance", Code = "FIN", ManagerUserId = finance.Id, CreatedBy = admin.Id });
            await context.SaveChangesAsync();
        }

        if (!await context.EmployeeProfiles.AnyAsync())
        {
            var opsDepartment = await context.Departments.FirstAsync(x => x.Code == "OPS");
            var managerProfile = new EmployeeProfile
            {
                UserId = manager.Id,
                EmployeeCode = "EMP1001",
                FullName = manager.FullName,
                JobTitle = "Operations Manager",
                DepartmentId = opsDepartment.Id,
                DateOfJoining = DateTime.UtcNow.AddYears(-4),
                CreatedBy = admin.Id
            };

            context.EmployeeProfiles.Add(managerProfile);
            await context.SaveChangesAsync();

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                UserId = employee.Id,
                EmployeeCode = "EMP1002",
                FullName = employee.FullName,
                JobTitle = "Operations Analyst",
                DepartmentId = opsDepartment.Id,
                ManagerProfileId = managerProfile.Id,
                DateOfJoining = DateTime.UtcNow.AddYears(-1),
                CreatedBy = admin.Id
            });
            await context.SaveChangesAsync();
        }

        if (!await context.RequestTypes.AnyAsync())
        {
            context.RequestTypes.AddRange(
                new RequestType { Name = "Leave", Code = "LEAVE", Description = "Annual and casual leave", CreatedBy = admin.Id },
                new RequestType { Name = "Asset", Code = "ASSET", Description = "Laptop/mobile/accessory request", CreatedBy = admin.Id },
                new RequestType { Name = "Reimbursement", Code = "REIMBURSE", Description = "Expense reimbursement", CreatedBy = admin.Id });
            await context.SaveChangesAsync();
        }

        if (!await context.ApprovalFlows.AnyAsync())
        {
            var leave = await context.RequestTypes.FirstAsync(x => x.Code == "LEAVE");
            var asset = await context.RequestTypes.FirstAsync(x => x.Code == "ASSET");
            var reimburse = await context.RequestTypes.FirstAsync(x => x.Code == "REIMBURSE");

            var leaveFlow = new ApprovalFlow
            {
                RequestTypeId = leave.Id,
                Name = "Leave Workflow",
                CreatedBy = admin.Id,
                StepTemplates =
                [
                    new ApprovalStepTemplate { Sequence = 1, RoleName = SystemRoles.Manager, IsDepartmentManager = true, CreatedBy = admin.Id },
                    new ApprovalStepTemplate { Sequence = 2, RoleName = SystemRoles.Hr, IsDepartmentManager = false, CreatedBy = admin.Id }
                ]
            };

            var assetFlow = new ApprovalFlow
            {
                RequestTypeId = asset.Id,
                Name = "Asset Workflow",
                CreatedBy = admin.Id,
                StepTemplates =
                [
                    new ApprovalStepTemplate { Sequence = 1, RoleName = SystemRoles.Manager, IsDepartmentManager = true, CreatedBy = admin.Id },
                    new ApprovalStepTemplate { Sequence = 2, RoleName = SystemRoles.ItSupport, IsDepartmentManager = false, CreatedBy = admin.Id }
                ]
            };

            var reimburseFlow = new ApprovalFlow
            {
                RequestTypeId = reimburse.Id,
                Name = "Reimbursement Workflow",
                CreatedBy = admin.Id,
                StepTemplates =
                [
                    new ApprovalStepTemplate { Sequence = 1, RoleName = SystemRoles.Manager, IsDepartmentManager = true, CreatedBy = admin.Id },
                    new ApprovalStepTemplate { Sequence = 2, RoleName = SystemRoles.Finance, IsDepartmentManager = false, CreatedBy = admin.Id }
                ]
            };

            context.ApprovalFlows.AddRange(leaveFlow, assetFlow, reimburseFlow);
            await context.SaveChangesAsync();
        }
    }

    private static async Task<ApplicationUser> EnsureUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string fullName)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            return user;
        }

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }

        return user;
    }

    private static async Task EnsureRoleAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, string role)
    {
        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}