using Microsoft.EntityFrameworkCore;
using MoneyMood.Data;
using MoneyMood.Dtos.Budget;
using MoneyMood.Dtos.Entry;
using MoneyMood.Exceptions;
using MoneyMood.Models;

namespace MoneyMood.Services.FixedUserEntry;

public class FixedEntryService(AppDbContext context) : IFixedEntryService
{
    public async Task<IEnumerable<EntryResponse>> GetFixedEntriesAsync(Guid userId, EntryType type)
    {
        return await context.FixedEntries
            .Where(e => e.UserId == userId && e.Type == type)
            .OrderBy(e => e.CreatedAt)
            .Select(e => MapToResponse(e))
            .ToListAsync();
    }

    public async Task<IEnumerable<EntryResponse>> CreateFixedEntriesAsync(Guid userId, EntryType type, ICollection<EntryRequest> request)
    {
        var entries = request.Select(r => new FixedEntry
        {
            Name = r.Name,
            Amount = r.Amount,
            Type = type,
            UserId = userId
        }).ToList();

        context.FixedEntries.AddRange(entries);
        await context.SaveChangesAsync();

        return entries.Select(e => MapToResponse(e));
    }

    public async Task<EntryResponse> UpdateFixedEntryAsync(Guid userId, Guid entryId, EntryRequest request)
    {
        var entry = await context.FixedEntries
            .FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId)
            ?? throw ApiException.NotFound("Entrée non trouvée ou non liée au budget");

        entry.Name = request.Name;
        entry.Amount = request.Amount;
        await context.SaveChangesAsync();

        return  MapToResponse(entry);
    }

    public async Task<DeletedEntryResponse> DeleteFixedEntryAsync(Guid userId, Guid entryId)
    {
       var entry = await context.FixedEntries
            .FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId)
            ?? throw ApiException.NotFound("Entrée non trouvée ou non liée au budget");

        context.FixedEntries.Remove(entry);
        await context.SaveChangesAsync(); 

        return new DeletedEntryResponse
        {
            Id = entry.Id
        };
    }

    private static EntryResponse MapToResponse(FixedEntry entry) => new()
    {
        Id = entry.Id, 
        Name = entry.Name, 
        Amount = entry.Amount
    };
}