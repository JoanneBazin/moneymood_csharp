
using FluentValidation;
using MoneyMood.Dtos.Category;

namespace MoneyMood.Validators.Category;

public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Le nom est requis")
            .MinimumLength(2).WithMessage("Le nom est trop court")
            .MaximumLength(30).WithMessage("Le nom est trop long");
    }
}