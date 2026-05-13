using Dictionary.Application.Common;
using Dictionary.Application.Common.Interfaces;
using Dictionary.Application.Features.Labels.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Application.Features.Labels.Queries.GetAllLabels;

/// <summary>
///  Tüm etiketleri listeleyen sorgu. Etiketlerin bağlı olduğu sense sayısı da döndürülür.
/// </summary>
public record GetAllLabelsQuery : IRequest<AppResult<List<LabelDto>>>;
//Handler
public class GetAllLabelsQueryHandler
    : IRequestHandler<GetAllLabelsQuery, AppResult<List<LabelDto>>>
{
    private readonly IAppDbContext _context;

    public GetAllLabelsQueryHandler(IAppDbContext context)
        => _context = context;

    public async Task<AppResult<List<LabelDto>>> Handle(
        GetAllLabelsQuery request, CancellationToken ct)
    {
       throw new NotImplementedException();
    }
}
