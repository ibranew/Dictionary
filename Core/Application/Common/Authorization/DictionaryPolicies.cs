
namespace Dictionary.Application.Common.Authorization;

/// <summary>
/// Authorization politikalarını tanımlayan statik bir sınıf. 
/// Bu sınıf, 
/// uygulama genelinde kullanılacak olan yetkilendirme politikalarını 
/// merkezi bir yerde tanımlamak için kullanılır. 
/// Her politika, belirli roller veya gereksinimler doğrultusunda erişim kontrolü sağlar.
/// </summary>
public static class DictionaryPolicies
{
    /// <summary>
    /// Sadece Admin rolüne sahip kullanıcıların erişebileceği bir politika. 
    /// </summary>
    public const string AdminOnly = "AdminOnly";
    /// <summary>
    /// Admin ve Editor rollerine sahip kullanıcıların erişebileceği bir politika.
    /// </summary>
    public const string EditorAccess = "EditorAccess";
}