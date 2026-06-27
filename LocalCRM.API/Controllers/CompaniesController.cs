using LocalCRM.Application.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly CompanyService _companyService;

    public CompaniesController(CompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CompanyDto>>> GetAll([FromQuery] bool includeDeleted = false)
    {
        return await _companyService.GetAllAsync(includeDeleted);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetById(int id)
    {
        var company = await _companyService.GetByIdAsync(id);
        if (company == null) return NotFound();
        return company;
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateCompanyCommand command)
    {
        return await _companyService.CreateAsync(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateCompanyCommand command)
    {
        if (id != command.CompanyId) return BadRequest();
        await _companyService.UpdateAsync(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _companyService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("{id}/restore")]
    public async Task<ActionResult> Restore(int id)
    {
        await _companyService.RestoreAsync(id);
        return NoContent();
    }
}
