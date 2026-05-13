
using Dictionary.Application.Common.Interfaces;
using Dictionary.Application.Features.Dashboard.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Application.Features.Dashboard.Queries.GetStats;

public record GetStatsQuery : IRequest<DashboardStatsDto>;

public class GetStatsQueryHandler : IRequestHandler<GetStatsQuery, DashboardStatsDto>
{
    private readonly IAppDbContext _context;
    public GetStatsQueryHandler(IAppDbContext context)
    {
        _context=context;
    }
    public async Task<DashboardStatsDto> Handle(
    GetStatsQuery request,
    CancellationToken cancellationToken)
    {
        return new DashboardStatsDto
        {
            TotalEntries = await _context.Entries
                .CountAsync(cancellationToken),

            TotalSenses = await _context.Senses
                .CountAsync(cancellationToken),

            TotalSenseTranslations = await _context.SenseTranslations
                .CountAsync(cancellationToken),

            TotalExamples = await _context.Examples
                .CountAsync(cancellationToken),

            SoftDeletedCount = await _context.Entries
                .CountAsync(e => e.IsDeleted, cancellationToken)
        };
    }
}

