using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpsPilot.Domain.Common;
using OpsPilot.Domain.Entities;
using OpsPilot.Infrastructure.Identity;

namespace OpsPilot.Infrastructure.Persistence;

public class OpsPilotDbContext : IdentityDbContext<ApplicationUser>
{
    public OpsPilotDbContext(DbContextOptions<OpsPilotDbContext> options)
        : base(options)
    {
    }

    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<RequestStatusHistory> RequestStatusHistories => Set<RequestStatusHistory>();
    public DbSet<RequestType> RequestTypes => Set<RequestType>();
    public DbSet<ApprovalFlow> ApprovalFlows => Set<ApprovalFlow>();
    public DbSet<ApprovalStepTemplate> ApprovalStepTemplates => Set<ApprovalStepTemplate>();
    public DbSet<ApprovalStep> ApprovalSteps => Set<ApprovalStep>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Reimbursement> Reimbursements => Set<Reimbursement>();
    public DbSet<PolicyDocument> PolicyDocuments => Set<PolicyDocument>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Department>().HasIndex(x => x.Code).IsUnique();
        builder.Entity<RequestType>().HasIndex(x => x.Code).IsUnique();
        builder.Entity<EmployeeProfile>().HasIndex(x => x.UserId).IsUnique();

        builder.Entity<EmployeeProfile>()
            .Navigation(x => x.Department)
            .AutoInclude();

        builder.Entity<EmployeeProfile>()
            .HasOne(x => x.ManagerProfile)
            .WithMany(x => x.DirectReports)
            .HasForeignKey(x => x.ManagerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Request>()
            .HasOne(x => x.RequestType)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.RequestTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Request>()
            .HasOne(x => x.EmployeeProfile)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.EmployeeProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Request>()
            .HasIndex(x => new { x.Status, x.EmployeeProfileId });

        builder.Entity<Request>()
            .Navigation(x => x.RequestType)
            .AutoInclude();

        builder.Entity<Request>()
            .Navigation(x => x.EmployeeProfile)
            .AutoInclude();

        builder.Entity<Request>()
            .HasMany(x => x.StatusHistory)
            .WithOne(x => x.Request)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RequestStatusHistory>()
            .HasIndex(x => new { x.RequestId, x.ActionDate });

        builder.Entity<ApprovalFlow>()
            .HasMany(x => x.StepTemplates)
            .WithOne(x => x.ApprovalFlow)
            .HasForeignKey(x => x.ApprovalFlowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApprovalStep>()
            .HasOne(x => x.Request)
            .WithMany(x => x.ApprovalSteps)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Asset>()
            .HasOne(x => x.Request)
            .WithOne(x => x.Asset)
            .HasForeignKey<Asset>(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Reimbursement>()
            .HasOne(x => x.Request)
            .WithOne(x => x.Reimbursement)
            .HasForeignKey<Reimbursement>(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Reimbursement>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<BaseAuditableEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                if (string.IsNullOrWhiteSpace(entry.Entity.CreatedBy))
                {
                    entry.Entity.CreatedBy = "system";
                }
            }
            else
            {
                entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}