namespace Dictionary.Application.Features.Entries.DTOs
{
    public class EntryFormDto
    {
        public int Id { get; set; }
        public string Headword { get; set; } = null!;
        public int LanguageId { get; set; }
        public int? ScriptId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
