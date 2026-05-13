

using Dictionary.Application.Common;
using Dictionary.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Dictionary.Application.Features.Entries.Commands.RestoreEntry;

/// <summary>
/// Silinmiş bir kelimeyi geri yüklemek için kullanılan komut. 
/// Verilen Id'ye sahip kelime bulunur ve silinme durumu kaldırılarak geri yüklenir.
/// </summary>
/// <param name="Id"></param>
public record RestoreEntryCommand(int Id) : IRequest<AppResult>;


public class RestoreEntryCommandHandler : IRequestHandler<RestoreEntryCommand, AppResult>
{
    private readonly IAppDbContext _context;
    public RestoreEntryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<AppResult> Handle(RestoreEntryCommand request, CancellationToken cancellationToken)
    {

        var entry = await _context.Entries
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        
        if (entry == null)
            return AppResult.Failure($"Id'si '{request.Id}' olan kelime bulunamadı.");

        if(!entry.IsDeleted)
            return AppResult.Failure($"Id'si '{request.Id}' olan kelime zaten silinmemiş durumda.");

        //Soft restore: 
        entry.IsDeleted = false;
        entry.DeletedAt = null;
        _context.Entries.Update(entry);
        await _context.SaveChangesAsync(cancellationToken);
        return AppResult.Success();
    }
}

