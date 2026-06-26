using System.Reflection;
using FluentValidation;
using LocalCRM.Application.AuditLogs;
using LocalCRM.Application.Auth;
using LocalCRM.Application.Companies;
using LocalCRM.Application.Contacts;
using LocalCRM.Application.Engagements;
using LocalCRM.Application.Interactions;
using LocalCRM.Application.Notes;
using LocalCRM.Application.Documents;
using LocalCRM.Application.Tags;
using LocalCRM.Application.Settings;
using LocalCRM.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace LocalCRM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IInteractionService, InteractionService>();
        services.AddScoped<IEngagementService, EngagementService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
