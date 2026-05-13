using Dictionary.Application.Common;
using Dictionary.Application.Common.Interfaces;
using Dictionary.Application.Features.Entries.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Dictionary.Application.Features.Entries.Queries.GetEntryById
{
    public record GetEntryByIdQuery(int Id, bool IncludeDeleted = false) : IRequest<AppResult<EntryFormDto>>;

    public class GetEntryByIdQueryHandler : IRequestHandler<GetEntryByIdQuery, AppResult<EntryFormDto>>
    {
        private readonly IAppDbContext _context;
        public GetEntryByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<AppResult<EntryFormDto>> Handle(GetEntryByIdQuery request, CancellationToken cancellationToken)
        {
            //silinmiş verileri de isteyebilsin
            var query = request.IncludeDeleted
               ? _context.Entries.IgnoreQueryFilters().AsNoTracking()
               : _context.Entries.AsNoTracking();

            var entry = await query
                       .Where(e => e.Id == request.Id)
                       .Select(e => new EntryFormDto
                       {
                            Id = e.Id,
                            Headword = e.Headword,
                            LanguageId = e.LanguageId,
                            ScriptId = e.ScriptId,
                            IsDeleted = e.IsDeleted
                       })
                       .FirstOrDefaultAsync(cancellationToken);
            if (entry == null)
                return AppResult<EntryFormDto>.Failure("Kelime bulunamadı.");

            return AppResult<EntryFormDto>.Success(entry);
        }
    }

}
