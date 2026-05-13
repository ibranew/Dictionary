using Dictionary.Application.Common.DTOs;
namespace Dictionary.Application.Common.Interfaces;
/// <summary>
/// Uygulama genelinde kullanılan lookup (dropdown) verilerini sağlar.
/// 
/// Bu servis UI bağımsızdır ve sadece basit Id-Name çiftleri döner.
/// ASP.NET'e özel tipler (SelectListItem gibi) içermez.
/// 
/// Kullanım alanları:
/// - Language dropdown
/// - Label seçimleri
/// - Script listeleri
/// - Diğer sabit referans veriler
/// </summary>
public interface ILookupService
{
    /// <summary>
    /// Tüm dilleri döner.
    /// </summary>
    Task<List<LookupDto>> GetLanguagesAsync();

    /// <summary>
    /// Tüm label (etiket) verilerini döner.
    /// </summary>
    Task<List<LookupDto>> GetLabelsAsync();

    /// <summary>
    /// Yazı sistemi (script) verilerini döner.
    /// Örn: Latin, Hangul vb.
    /// </summary>
    Task<List<LookupDto>> GetScriptsAsync();
}