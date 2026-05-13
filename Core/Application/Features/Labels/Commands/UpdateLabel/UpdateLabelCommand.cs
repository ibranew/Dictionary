

using Dictionary.Application.Common;
using Dictionary.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Application.Features.Labels.Commands.UpdateLabel;

public record UpdateLabelCommand(int Id, string Name, string Type) : IRequest<AppResult>;

public class UpdateLabelCommandHandler : IRequestHandler<UpdateLabelCommand, AppResult>
{
    private readonly IAppDbContext _context;

    public UpdateLabelCommandHandler(IAppDbContext context)
        => _context = context;

    public async Task<AppResult> Handle(UpdateLabelCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}