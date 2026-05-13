namespace Dictionary.Application.Features.Entries.DTOs
{
    public class EntryRowDto
    {
        public int Id { get; set; }
        public string Headword { get; set; } = null!;
        public string LanguageName { get; set; } = null!;
        public int SenseCount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
