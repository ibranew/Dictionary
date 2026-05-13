using Dictionary.Application.Common.Interfaces;
using Dictionary.Application.Common.Interfaces.Text;
using Dictionary.Application.Features.Entries.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Application.Features.Entries.Queries.GetEntries
{
    public class GetEntriesResult
    {
        public List<EntryRowDto> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
    /// <summary>
    /// Entryleri Döner sadece listeleme için
    /// </summary>
    /// <param name="Search"></param>
    /// <param name="LanguageId"></param>
    /// <param name="Page"></param>
    public record GetEntriesQuery
        (string? Search,
        int? LanguageId, 
        int Page = 1,
        int PageSize = 10,
        bool IncludeDeleted = false) : IRequest<GetEntriesResult>;
    public class GetEntriesQueryHandler : IRequestHandler<GetEntriesQuery, GetEntriesResult>
    {
        private readonly IAppDbContext _context;
        private readonly ITextNormalizer _textNormalizer;
        public GetEntriesQueryHandler(IAppDbContext context, ITextNormalizer textNormalizer)
        {
            _context=context;
            _textNormalizer=textNormalizer;
        }
        public async Task<GetEntriesResult> Handle(GetEntriesQuery request, CancellationToken cancellationToken)
        {
            //Query
            var query = request.IncludeDeleted
                ? _context.Entries.IgnoreQueryFilters().AsNoTracking()
                : _context.Entries.AsNoTracking();

            //Arama metni
            if (!string.IsNullOrEmpty(request.Search))
            {
                string norm = _textNormalizer.Normalize(request.Search);
                query = query.Where(x => x.HeadwordNormalized.Contains(norm));
            }

            //Dil
            if (request.LanguageId.HasValue)
                query = query.Where(x => x.LanguageId == request.LanguageId);

            //Toplam
            var totalCount = await query.CountAsync(cancellationToken);

            //Veri
            var items = await query
                .OrderBy(x => x.Headword)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new EntryRowDto
                {
                    Id = x.Id,
                    Headword = x.Headword,
                    LanguageName = x.Language.Name,
                    SenseCount = x.Senses.Count,
                    IsDeleted = x.IsDeleted
                })
                .ToListAsync(cancellationToken);

            return new GetEntriesResult
            {
                Items = items,
                TotalCount = totalCount
            };
        }
    }


}
