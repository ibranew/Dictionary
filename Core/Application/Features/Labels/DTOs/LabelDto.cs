namespace Dictionary.Application.Features.Labels.DTOs;

public class LabelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string LabelType { get; set; } = null!;

    /// <summary> Bu etikete bağlı toplam sense sayısı. </summary>
    public int SenseCount { get; set; }
}
