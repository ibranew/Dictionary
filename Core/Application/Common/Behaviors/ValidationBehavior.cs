using FluentValidation;
using MediatR;

namespace Dictionary.Application.Common.Behaviors;

/// Validasyon hatalarını AppResult veya AppResult<T> formatında döner.
/// Validasyon için Yazılan Validator'lar TRequest türünde olmalıdır !.
/// Command 'larda kullanılması önerilir.
/// Örnek: CreateEntryCommand, 
/// için CreateEntryValidator 
/// : AbstractValidator<CreateEntryCommand> yazılmalıdır. 
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => f.ErrorMessage)
            .ToList();

        if (failures.Any())
        {
            var responseType = typeof(TResponse);

            // AppResult<T> ise
            if (responseType.IsGenericType &&
                responseType.GetGenericTypeDefinition() == typeof(AppResult<>))
            {
                var genericArg = responseType.GetGenericArguments()[0];

                var failureMethod = typeof(AppResult<>)
                    .MakeGenericType(genericArg)
                    .GetMethod(nameof(AppResult<object>.Failure), new[] { typeof(List<string>) });

                return (TResponse)failureMethod!.Invoke(null, new object[] { failures })!;
            }

            // AppResult ise
            if (responseType == typeof(AppResult))
            {
                return (TResponse)(object)AppResult.Failure(failures);
            }
            // Diğer durumlarda, Hata mesajı dön
            throw new InvalidOperationException("TResponse must be AppResult or AppResult<T>");
        }
        return await next();
    }
}