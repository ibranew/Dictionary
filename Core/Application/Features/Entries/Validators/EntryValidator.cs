using Dictionary.Application.Features.Entries.Commands.CreateEntry;
using Dictionary.Application.Features.Entries.Commands.UpdateEntry;
using FluentValidation;

namespace Dictionary.Application.Features.Entries.Validators;
public class CreateEntryValidator : AbstractValidator<CreateEntryCommand>
{
    public CreateEntryValidator()
    {
        RuleFor(x => x.Headword)
            .NotEmpty().WithMessage("Headword boş olamaz.")
            .MaximumLength(200).WithMessage("Headword en fazla 200 karakter olabilir.");
        RuleFor(x => x.LanguageId)
            .GreaterThan(0).WithMessage("Geçerli bir Dil girilmelidir.");
        RuleFor(x => x.ScriptId)
            .GreaterThan(0).WithMessage("Geçerli bir Yazı tipi girilmelidir.");
    }
}
public class UpdateEntryValidator : AbstractValidator<UpdateEntryCommand>
{
    public UpdateEntryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir Id girilmelidir.");
        RuleFor(x => x.Headword)
            .NotEmpty().WithMessage("Headword boş olamaz.")
            .MaximumLength(200).WithMessage("Headword en fazla 200 karakter olabilir.");
        RuleFor(x => x.LanguageId)
            .GreaterThan(0).WithMessage("Geçerli bir Dil girilmelidir.");
        RuleFor(x => x.ScriptId)
            .GreaterThan(0).WithMessage("Geçerli bir Yazı tipi girilmelidir.");
    }
}