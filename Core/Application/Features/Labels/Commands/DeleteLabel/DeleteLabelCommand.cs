using Dictionary.Application.Common;
using Dictionary.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Dictionary.Application.Features.Labels.Commands.DeleteLabel;

public record DeleteLabelCommand(int Id) : IRequest<AppResult>;

public class DeleteLabelCommandHandler : IRequestHandler<DeleteLabelCommand, AppResult>
{
    private readonly IAppDbContext _context;

    public DeleteLabelCommandHandler(IAppDbContext context)
        => _context = context;

    public async Task<AppResult> Handle(DeleteLabelCommand request, CancellationToken ct)
    {
        var label = await _context.Labels
            .Include(l => l.SenseLabels)
            .FirstOrDefaultAsync(l => l.Id == request.Id, ct);

        if (label is null)
            return AppResult.Failure("Etiket bulunamadı.");

        if (label.SenseLabels.Count > 0)
            return AppResult.Failure(
                $"Bu etiket {label.SenseLabels.Count} anlama bağlı, silinemez.");

        _context.Labels.Remove(label);
        await _context.SaveChangesAsync(ct);

        return AppResult.Success("Etiket başarıyla silindi.");
    }
}
