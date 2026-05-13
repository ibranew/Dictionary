

using Dictionary.Application.Common.DTOs;
using Dictionary.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Persistence.Services;

public class LookupService : ILookupService
{
    private readonly IAppDbContext _context;

    public LookupService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LookupDto>> GetLanguagesAsync()
    {
        return await _context.Languages
            .AsNoTracking()
            .Select(x => new LookupDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();
    }

    public async Task<List<LookupDto>> GetLabelsAsync()
    {
        return await _context.Labels
            .AsNoTracking()
            .Select(x => new LookupDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();
    }

    public async Task<List<LookupDto>> GetScriptsAsync()
    {
        return await _context.Scripts
            .AsNoTracking()
            .Select(x => new LookupDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();
    }
}
