using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Settings;

public interface ISettingService
{
    Task<List<SettingDto>> GetSettingsAsync();
    Task<string?> GetSettingValueAsync(string key);
    Task UpdateSettingAsync(string key, string value);
}

public class SettingService : ISettingService
{
    private readonly ILocalCRMContext _context;
    private readonly IMapper _mapper;

    public SettingService(ILocalCRMContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<SettingDto>> GetSettingsAsync()
    {
        return await _context.Settings
            .ProjectTo<SettingDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<string?> GetSettingValueAsync(string key)
    {
        var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
        return setting?.Value;
    }

    public async Task UpdateSettingAsync(string key, string value)
    {
        var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting == null)
        {
            _context.Settings.Add(new Domain.Entities.Setting { Key = key, Value = value });
        }
        else
        {
            setting.Value = value;
        }

        await _context.SaveChangesAsync();
    }
}
