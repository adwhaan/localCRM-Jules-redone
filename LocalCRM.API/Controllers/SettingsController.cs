using LocalCRM.Application.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ISettingService _settingService;

    public SettingsController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SettingDto>>> GetSettings()
    {
        return await _settingService.GetSettingsAsync();
    }

    [HttpGet("{key}")]
    public async Task<ActionResult<string?>> GetSetting(string key)
    {
        return await _settingService.GetSettingValueAsync(key);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSetting([FromBody] SettingDto setting)
    {
        await _settingService.UpdateSettingAsync(setting.Key, setting.Value);
        return NoContent();
    }
}
