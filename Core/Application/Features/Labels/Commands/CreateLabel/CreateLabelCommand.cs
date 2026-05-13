using Dictionary.Application.Common;
using Dictionary.Domain.Entities;
using Dictionary.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Application.Features.Labels.Commands.CreateLabel;

public record CreateLabelCommand(string Name, string Type) : IRequest<AppResult>;

public class CreateLabelCommandHandler : IRequestHandler<CreateLabelCommand, AppResult>
{
    private readonly IAppDbContext _context;

    public CreateLabelCommandHandler(IAppDbContext context)
        => _context = context;

    public async Task<AppResult> Handle(CreateLabelCommand request, CancellationToken ct)
    {
       throw new NotImplementedException();
    }
}