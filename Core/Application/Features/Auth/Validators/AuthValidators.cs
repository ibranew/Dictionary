using Dictionary.Application.Features.Auth.Commands.Login;
using Dictionary.Application.Features.Auth.Commands.Register;
using Dictionary.Application.Features.Auth.DTOs;


using FluentValidation;
namespace Dictionary.Application.Features.Auth.Validators;
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.RegisterDto)
            .SetValidator(new RegisterDtoValidator());
    }
}
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.LoginDto)
            .SetValidator(new LoginDtoValidator());
    }
}
public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("EmailOrUserName boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
    }
}
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.EmailOrUserName)
            .NotEmpty().WithMessage("E-posta veya kullanıcı adı boş olamaz.");
           
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
    }
}