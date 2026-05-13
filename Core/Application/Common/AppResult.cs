namespace Dictionary.Application.Common;

/// <summary>
/// Tüm handler'ların döndürdüğü standart sonuç tipi.
/// Başarı/hata durumunu ve mesajı taşır.
/// </summary>
public class AppResult
{
    public bool IsSuccess { get; private set; }
    public string? Message { get; private set; }
    public List<string>? Errors { get; private set; }

    protected AppResult(bool isSuccess, string? message, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors;
    }

    public static AppResult Success(string? message = null)
        => new(true, message);

    public static AppResult Failure(string message)
        => new(false, message);

    public static AppResult Failure(List<string> errors)
        => new(false, "Doğrulama Hatası", errors);
}

/// <summary>
/// Veri döndüren handler'lar için generic AppResult.
/// command işlemleri için elzem query işlemleri için genelde gereksiz
/// </summary>
public class AppResult<T> : AppResult
{
    public T? Data { get; private set; }

    private AppResult(bool isSuccess, T? data, string? message, List<string>? errors = null)
        : base(isSuccess, message, errors)
    {
        Data = data;
    }

    public static AppResult<T> Success(T data, string? message = null)
        => new(true, data, message);

    /// <summary>
    /// Hata durumunda tek bir hata mesajı döndürmek için kullanılır.
    /// Base sınıftaki Failure metodunu gizler (new anahtar kelimesi) ve string parametresi alır.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static new AppResult<T> Failure(string message)
        => new(false, default, message);

    /// <summary>
    ///  Hata durumunda birden fazla hata mesajı döndürmek için kullanılır. 
    ///  Genellikle model doğrulama hataları için uygundur.
    ///  Base sınıftaki Failure metodunu gizler (new anahtar kelimesi) ve List<string> parametresi alır.
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static new AppResult<T> Failure(List<string> errors)
        => new(false, default, "Validation failed", errors);
}