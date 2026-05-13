namespace Dictionary.Application.Common.Interfaces.Text;

/// <summary>
/// Metinleri arama ve karşılaştırma işlemleri için standart formata dönüştüren servistir.
/// 
/// Bu arayüzün amacı:
/// - Kullanıcıdan gelen metinleri normalize etmek
/// - Farklı yazım varyasyonlarını (büyük/küçük harf, boşluk vb.) tek form haline getirmek
/// - Arama ve karşılaştırma işlemlerinde tutarlılık sağlamaktır
/// 
/// Not:
/// Bu işlem dil bağımsız bir soyutlama gibi görünse de, gerçek implementasyonlar
/// dil kurallarına göre değişebilir (örn: Türkçe, Korece vb.)
/// </summary>
public interface ITextNormalizer
{
    string Normalize(string input);
}