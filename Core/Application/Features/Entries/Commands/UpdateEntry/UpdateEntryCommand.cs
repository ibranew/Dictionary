using Dictionary.Application.Common;
using Dictionary.Application.Common.Interfaces;
using Dictionary.Application.Common.Interfaces.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Application.Features.Entries.Commands.UpdateEntry;

/// <summary>
/// Entry güncellemek için gerekli bilgileri içeren komut sınıfı.
/// </summary>
public class UpdateEntryCommand : IRequest<AppResult>
{
    public int Id { get; set; }
    public string Headword { get; set; } = null!;
    public int LanguageId { get; set; }
    public int ScriptId { get; set; }
}

public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand, AppResult>
{
    readonly IAppDbContext _context;
    readonly ITextNormalizer _textNormalizer;

    public UpdateEntryCommandHandler(IAppDbContext context, ITextNormalizer textNormalizer)
    {
        _context=context;
        _textNormalizer=textNormalizer;
    }

    public async Task<AppResult> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
    {
        // Headword normalize
        var normalizedHeadword = _textNormalizer.Normalize(request.Headword.Trim());

        // Dil var mı 
        var languageExists = await _context.Languages
            .AnyAsync(x => x.Id == request.LanguageId, cancellationToken);

        if (!languageExists)
            return AppResult.Failure($"Seçilen dil bulunamadı. (Id: {request.LanguageId})");

        // Script var mı
        var scriptExists = await _context.Scripts
            .AnyAsync(x => x.Id == request.ScriptId, cancellationToken);

        if (!scriptExists)
            return AppResult.Failure($"Seçilen yazı sistemi bulunamadı. (Id: {request.ScriptId})");

        // Entry var mı
        var entry = await _context.Entries
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        
        if (entry == null)
            return AppResult.Failure($"Güncellenmek istenen entry bulunamadı. (Id: {request.Id})");

        // Aynı headword ve dilde başka bir entry var mı
        var duplicateEntryExists = await _context.Entries
            .AnyAsync(x => x.Id != request.Id &&
                           x.HeadwordNormalized == normalizedHeadword &&
                           x.LanguageId == request.LanguageId, cancellationToken);

        if (duplicateEntryExists)
            return AppResult.Failure($"Aynı headword ve dilde başka bir entry zaten mevcut. (Headword: {request.Headword}, LanguageId: {request.LanguageId})");

        // Entry güncelle
        entry.Headword = request.Headword.Trim();
        entry.HeadwordNormalized = normalizedHeadword;
        entry.ScriptId = request.ScriptId;
        entry.LanguageId = request.LanguageId;

        await _context.SaveChangesAsync(cancellationToken);

        return AppResult.Success("Kelime başarıyla güncellendi.");

        throw new NotImplementedException();
    }
}