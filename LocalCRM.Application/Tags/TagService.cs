using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Tags;

public interface ITagService
{
    Task<List<TagDto>> GetTagsAsync();
    Task<List<TagDto>> GetTagsByGroupAsync(string group);
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

    public async Task<List<TagDto>> GetTagsAsync()
    {
        return await _context.Tags
            .OrderBy(t => t.TagGroup).ThenBy(t => t.TagOrder)
            .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<TagDto>> GetTagsByGroupAsync(string group)
    {
        return await _context.Tags
            .Where(t => t.TagGroup == group)
            .OrderBy(t => t.TagOrder)
            .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
