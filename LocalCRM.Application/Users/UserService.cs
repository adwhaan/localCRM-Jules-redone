using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Users;

public interface IUserService
{
    Task<List<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto> CreateUserAsync(CreateUserCommand command);
    Task UpdateUserAsync(UpdateUserCommand command);
    Task ResetPasswordAsync(int userId, string newPassword);
}

public class UserService : IUserService
{
    private readonly ILocalCRMContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public UserService(ILocalCRMContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Where(u => u.UserId == id)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<UserDto> CreateUserAsync(CreateUserCommand command)
    {
        var existing = await _context.Users.AnyAsync(u => u.Username == command.Username);
        if (existing) throw new Exception("Username already exists");

        var entity = new User
        {
            Username = command.Username,
            PasswordHash = _identityService.HashPassword(command.Password),
            RoleId = command.RoleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.Username ?? "system",
            MustChangePassword = true // Requirement: admin-created users must change password on first login
        };

        _context.Users.Add(entity);
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "users",
            EntityId = entity.UserId,
            ActionType = "CREATE",
            PerformedBy = entity.CreatedBy,
            Notes = $"Created user: {entity.Username}"
        });

        await _context.SaveChangesAsync();

        return await GetUserByIdAsync(entity.UserId) ?? throw new Exception("Error retrieving created user");
    }

    public async Task UpdateUserAsync(UpdateUserCommand command)
    {
        var entity = await _context.Users.FindAsync(command.UserId);
        if (entity == null) throw new Exception("User not found");

        entity.RoleId = command.RoleId;
        entity.IsActive = command.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUserService.Username ?? "system";

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "users",
            EntityId = entity.UserId,
            ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Updated user: {entity.Username}"
        });

        await _context.SaveChangesAsync();
    }

    public async Task ResetPasswordAsync(int userId, string newPassword)
    {
        var entity = await _context.Users.FindAsync(userId);
        if (entity == null) throw new Exception("User not found");

        entity.PasswordHash = _identityService.HashPassword(newPassword);
        entity.MustChangePassword = true;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUserService.Username ?? "system";

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "users",
            EntityId = entity.UserId,
            ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Reset password for user: {entity.Username}"
        });

        await _context.SaveChangesAsync();
    }
}
