using FluentValidation;

namespace MoneyMood.Validators.Shared;

public class CollectionValidator<T> : AbstractValidator<List<T>>
{
    public CollectionValidator(IValidator<T> validator)
    {
        RuleForEach(x => x).SetValidator(validator);
    }
}