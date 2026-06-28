using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Infrastructure.Persistence;

public class LocalCRMContext : DbContext, ILocalCRMContext
{
    public LocalCRMContext(DbContextOptions<LocalCRMContext> options) : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Interaction> Interactions => Set<Interaction>();
    public DbSet<Engagement> Engagements => Set<Engagement>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<CompanyContactLink> CompanyContactLinks => Set<CompanyContactLink>();
    public DbSet<InteractionLink> InteractionLinks => Set<InteractionLink>();
    public DbSet<CompanyNoteLink> CompanyNoteLinks => Set<CompanyNoteLink>();
    public DbSet<CompanyDocumentLink> CompanyDocumentLinks => Set<CompanyDocumentLink>();
    public DbSet<ContactNoteLink> ContactNoteLinks => Set<ContactNoteLink>();
    public DbSet<InteractionNoteLink> InteractionNoteLinks => Set<InteractionNoteLink>();
    public DbSet<InteractionDocumentLink> InteractionDocumentLinks => Set<InteractionDocumentLink>();
    public DbSet<EngagementCompanyLink> EngagementCompanyLinks => Set<EngagementCompanyLink>();
    public DbSet<EngagementContactLink> EngagementContactLinks => Set<EngagementContactLink>();
    public DbSet<EngagementNoteLink> EngagementNoteLinks => Set<EngagementNoteLink>();
    public DbSet<EngagementDocumentLink> EngagementDocumentLinks => Set<EngagementDocumentLink>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity(j => j.ToTable("role_permissions_link"));

        modelBuilder.Entity<CompanyContactLink>()
            .HasKey(c => new { c.CompanyId, c.ContactId, c.StartDate });

        modelBuilder.Entity<InteractionLink>()
            .HasKey(i => i.InteractionId);

        modelBuilder.Entity<InteractionLink>()
            .HasOne(il => il.Interaction)
            .WithOne()
            .HasForeignKey<InteractionLink>(il => il.InteractionId);

        modelBuilder.Entity<CompanyNoteLink>()
            .HasKey(c => new { c.CompanyId, c.NoteId });

        modelBuilder.Entity<CompanyDocumentLink>()
            .HasKey(c => new { c.CompanyId, c.DocumentId });

        modelBuilder.Entity<ContactNoteLink>()
            .HasKey(c => new { c.ContactId, c.NoteId });

        modelBuilder.Entity<InteractionNoteLink>()
            .HasKey(i => new { i.InteractionId, i.NoteId });

        modelBuilder.Entity<InteractionDocumentLink>()
            .HasKey(i => new { i.InteractionId, i.DocumentId });

        modelBuilder.Entity<EngagementCompanyLink>()
            .HasKey(e => new { e.EngagementId, e.CompanyId, e.StartDate });

        modelBuilder.Entity<EngagementContactLink>()
            .HasKey(e => new { e.EngagementId, e.ContactId, e.StartDate });

        modelBuilder.Entity<EngagementNoteLink>()
            .HasKey(e => new { e.EngagementId, e.NoteId });

        modelBuilder.Entity<EngagementDocumentLink>()
            .HasKey(e => new { e.EngagementId, e.DocumentId });

        modelBuilder.Entity<AuditLog>()
            .HasKey(a => a.AuditId);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(LocalCRM.Domain.Common.SoftDeletableEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression ConvertFilterExpression(Type type)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(type, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, "IsDeleted");
        var falseConstant = System.Linq.Expressions.Expression.Constant(false);
        var comparison = System.Linq.Expressions.Expression.Equal(property, falseConstant);
        return System.Linq.Expressions.Expression.Lambda(comparison, parameter);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<LocalCRM.Domain.Common.BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
