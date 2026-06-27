using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LocalCRM.Application.Tags;

public interface ITagService
{
    Task<List<TagDto>> GetAllAsync();
    Task<List<TagDto>> GetByGroupAsync(string group);
    Task<TagDto?> GetByIdAsync(int id);
    Task<TagDto> CreateAsync(CreateTagCommand command);
    Task UpdateAsync(UpdateTagCommand command);
    Task DeleteAsync(int id);
}

public class TagService : ITagService
{
    private readonly ILocalCRMContext _context;
    private readonly IMapper _mapper;

    public TagService(ILocalCRMContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TagDto>> GetAllAsync()
    {
        return await _context.Tags
            .OrderBy(t => t.TagGroup).ThenBy(t => t.TagOrder)
            .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<TagDto>> GetByGroupAsync(string group)
    {
        return await _context.Tags
            .Where(t => t.TagGroup == group)
            .OrderBy(t => t.TagOrder)
            .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<TagDto?> GetByIdAsync(int id)
    {
        return await _context.Tags
            .Where(t => t.TagId == id)
            .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<TagDto> CreateAsync(CreateTagCommand command)
    {
        var entity = _mapper.Map<Tag>(command);
        _context.Tags.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<TagDto>(entity);
    }

    public async Task UpdateAsync(UpdateTagCommand command)
    {
        var entity = await _context.Tags.FindAsync(command.TagId);
        if (entity == null) throw new Exception("Entity not found");

        _mapper.Map(command, entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Tags.FindAsync(id);
        if (entity == null) throw new Exception("Entity not found");

        _context.Tags.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
