
namespace Dictionary.Application.Features.Dashboard.DTOs;
public sealed class DashboardStatsDto
{
    public int TotalEntries { get; set; }

    public int TotalSenses { get; set; }

    public int TotalSenseTranslations { get; set; }

    public int TotalExamples { get; set; }

    public int SoftDeletedCount { get; set; }
}
