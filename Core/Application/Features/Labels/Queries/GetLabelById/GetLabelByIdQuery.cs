using Dictionary.Application.Common;
using Dictionary.Application.Common.Interfaces;
using Dictionary.Application.Features.Labels.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Application.Features.Labels.Queries.GetLabelById;

public record GetLabelByIdQuery(int Id) : IRequest<AppResult<LabelDto>>;

public class GetLabelByIdQueryHandler
    : IRequestHandler<GetLabelByIdQuery, AppResult<LabelDto>>
{
    private readonly IAppDbContext _context;

    public GetLabelByIdQueryHandler(IAppDbContext context)
        => _context = context;

    public async Task<AppResult<LabelDto>> Handle(
        GetLabelByIdQuery request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}