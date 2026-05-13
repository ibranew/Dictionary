
using Dictionary.Application.Features.Labels.Commands.CreateLabel;
using FluentValidation;
namespace Dictionary.Application.Features.Labels.Validators;

/// <summary>
/// Label oluşturma işlemleri için doğrulama kurallarını tanımlar. 
/// </summary>
public class CreateLabelValidator : AbstractValidator<CreateLabelCommand>
{
    public CreateLabelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Etiket adı boş olamaz.")
            .MaximumLength(100).WithMessage("Etiket adı en fazla 100 karakter olabilir.");
    }
   
}

/// <summary>
/// Label güncelleme işlemleri için doğrulama kurallarını tanımlar.
/// </summary>
public class UpdateLabelValidator : AbstractValidator<Commands.UpdateLabel.UpdateLabelCommand>
{
    public UpdateLabelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Etiket adı boş olamaz.")
            .MaximumLength(100).WithMessage("Etiket adı en fazla 100 karakter olabilir.");
    }
}
