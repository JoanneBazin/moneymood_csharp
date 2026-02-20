using MoneyMood.Dtos.Budget;
using MoneyMood.Dtos.Entry;

namespace MoneyMood.Services.Budget;

public interface IMonthlyEntryService
{
    Task<MonthlyEntryOperationResponse<IEnumerable<EntryResponse>>> CreateMonthlyEntriesAsync(Guid userId, Guid budgetId, EntryType type, ICollection<EntryRequest> request);
    Task<MonthlyEntryOperationResponse<EntryResponse>> UpdateMonthlyEntryAsync(Guid userId, Guid budgetId, Guid entryId, EntryRequest request);
    Task<MonthlyEntryOperationResponse<DeletedEntryResponse>> DeleteMonthlyEntryAsync(Guid userId, Guid budgetId, Guid entryId);
}