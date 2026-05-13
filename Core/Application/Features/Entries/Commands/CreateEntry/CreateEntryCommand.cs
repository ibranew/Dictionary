using Dictionary.Application.Common;
using Dictionary.Application.Common.Interfaces;
using Dictionary.Application.Common.Interfaces.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Dictionary.Application.Features.Entries.Commands.CreateEntry;
/// <summary>
/// En basit haliyle, yeni bir Entry (kelime) oluşturmak için gerekli bilgileri içeren komut sınıfı.
/// Geriye AppResult.Data içerisinde yeni oluşturulan Entry'nin Id'sini döndürür.
/// </summary>
public class CreateEntryCommand : IRequest<AppResult<int>>
{
    public string Headword { get; set; } = null!;
    public int LanguageId { get; set; }
    public int ScriptId { get; set; }
}
public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, AppResult<int>>
{
    private readonly IAppDbContext _context;
    private readonly ITextNormalizer _textNormalizer;

    public CreateEntryCommandHandler(IAppDbContext context, ITextNormalizer textNormalizer)
    {
        _context = context;
        _textNormalizer = textNormalizer;
    }

    public async Task<AppResult<int>> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
    {
        // Headword normalize
        var normalizedHeadword = _textNormalizer.Normalize(request.Headword.Trim());

        // Dil var mı 
        var languageExists = await _context.Languages
            .AnyAsync(x => x.Id == request.LanguageId, cancellationToken);

        if (!languageExists)
            return AppResult<int>.Failure($"Seçilen dil bulunamadı. (Id: {request.LanguageId})");

        // Script var mı
        var scriptExists = await _context.Scripts
            .AnyAsync(x => x.Id == request.ScriptId, cancellationToken);

        if (!scriptExists)
            return AppResult<int>.Failure($"Seçilen yazı sistemi bulunamadı. (Id: {request.ScriptId})");

        // Aynı kelime var mı 
        var exists = await _context.Entries
        .AnyAsync(e =>
           e.HeadwordNormalized == normalizedHeadword &&
           e.LanguageId == request.LanguageId,
           cancellationToken);

        if (exists)
            return AppResult<int>.Failure($"'{request.Headword}' kelimesi sözlükte zaten mevcut.");

        // Entity oluştur
        var entry = new Domain.Entities.Entry
        {
            Headword = request.Headword.Trim(),
            HeadwordNormalized = normalizedHeadword,
            LanguageId = request.LanguageId,
            ScriptId = request.ScriptId
        };

        // Veritabanına ekle
        _context.Entries.Add(entry);
        
        // Kaydet
        await _context.SaveChangesAsync(cancellationToken);
        
        return AppResult<int>.Success(entry.Id, "Kelime başarıyla oluşturuldu.");
    }
}