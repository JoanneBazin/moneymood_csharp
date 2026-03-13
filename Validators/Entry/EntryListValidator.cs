using FluentValidation;
using MoneyMood.Dtos.Entry;
using MoneyMood.Validators.Shared;

namespace MoneyMood.Validators.Entry;

public class EntryListValidator(IValidator<EntryRequest> validator): CollectionValidator<EntryRequest>(validator)
{
}