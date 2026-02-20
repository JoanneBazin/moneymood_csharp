using MoneyMood.Dtos.Budget;
using MoneyMood.Dtos.Entry;

namespace MoneyMood.Services.FixedUserEntry;

public interface IFixedEntryService
{
    Task<IEnumerable<EntryResponse>> GetFixedEntriesAsync(Guid userId, EntryType type);
    Task<IEnumerable<EntryResponse>> CreateFixedEntriesAsync(Guid userId, EntryType type, ICollection<EntryRequest> request);
    Task<EntryResponse> UpdateFixedEntryAsync(Guid userId, Guid entryId, EntryRequest request);
    Task<DeletedEntryResponse> DeleteFixedEntryAsync(Guid userId, Guid entryId);
}