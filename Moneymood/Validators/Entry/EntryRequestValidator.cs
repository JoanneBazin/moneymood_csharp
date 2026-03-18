using MoneyMood.Dtos.Entry;
using MoneyMood.Validators.Shared;

namespace MoneyMood.Validators.Entry;

public class EntryRequestValidator : EntryBaseValidator<EntryRequest>
{
    public EntryRequestValidator()
    {
        SetupEntryNameRules(x => x.Name);
        SetupEntryAmountRules(x => x.Amount);
    }
}