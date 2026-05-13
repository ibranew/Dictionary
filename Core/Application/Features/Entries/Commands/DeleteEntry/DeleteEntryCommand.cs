
using Dictionary.Application.Common;
using Dictionary.Application.Common.Interfaces;
using MediatR;

namespace Dictionary.Application.Features.Entries.Commands.DeleteEntry;

public record DeleteEntryCommand(int Id) : IRequest<AppResult>;

public class DeleteEntryCommandHandler : IRequestHandler<DeleteEntryCommand, AppResult>
{
    private readonly IAppDbContext _context;
    public DeleteEntryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<AppResult> Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.Entries.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entry == null)
            return AppResult.Failure($"Id'si '{request.Id}' olan kelime bulunamadı.");
       
        //Soft delete: 
        entry.IsDeleted = true;
        entry.DeletedAt = DateTime.UtcNow;

        _context.Entries.Update(entry);
        await _context.SaveChangesAsync(cancellationToken);
        return AppResult.Success();
    }
}
