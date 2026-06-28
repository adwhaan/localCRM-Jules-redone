using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Common.Interfaces;

public interface ILocalCRMContext
{
    DbSet<Company> Companies { get; }
    DbSet<Contact> Contacts { get; }
    DbSet<Interaction> Interactions { get; }
    DbSet<Engagement> Engagements { get; }
    DbSet<Note> Notes { get; }
    DbSet<Document> Documents { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<Tag> Tags { get; }
    DbSet<Setting> Settings { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<CompanyContactLink> CompanyContactLinks { get; }
    DbSet<InteractionLink> InteractionLinks { get; }
    DbSet<CompanyNoteLink> CompanyNoteLinks { get; }
    DbSet<CompanyDocumentLink> CompanyDocumentLinks { get; }
    DbSet<ContactNoteLink> ContactNoteLinks { get; }
    DbSet<InteractionNoteLink> InteractionNoteLinks { get; }
    DbSet<InteractionDocumentLink> InteractionDocumentLinks { get; }
    DbSet<EngagementCompanyLink> EngagementCompanyLinks { get; }
    DbSet<EngagementContactLink> EngagementContactLinks { get; }
    DbSet<EngagementNoteLink> EngagementNoteLinks { get; }
    DbSet<EngagementDocumentLink> EngagementDocumentLinks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
