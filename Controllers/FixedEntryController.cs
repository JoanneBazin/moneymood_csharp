using Microsoft.AspNetCore.Mvc;
using MoneyMood.Dtos.Budget;
using MoneyMood.Dtos.Entry;
using MoneyMood.Services.FixedUserEntry;

namespace MoneyMood.Controllers;

[ApiController]
[Route("api/fixed-incomes")]
public class FixedIncomesController(IFixedEntryService service) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EntryResponse>>> GetFixedIncomes()
    {
        return Ok(await service.GetFixedEntriesAsync(GetUserId(), EntryType.Income));
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<EntryResponse>>> CreateFixedIncomes(ICollection<EntryRequest> request)
    {
        return Ok(await service.CreateFixedEntriesAsync(GetUserId(), EntryType.Income, request));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EntryResponse>> UpdateFixedIncomes(Guid id, EntryRequest request)
    {
        return Ok(await service.UpdateFixedEntryAsync(GetUserId(), id, request));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedEntryResponse>> DeleteFixedIncomes(Guid id)
    {
        return Ok(await service.DeleteFixedEntryAsync(GetUserId(), id));
    }
}


[ApiController]
[Route("api/fixed-charges")]
public class FixedChargesController(IFixedEntryService service) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EntryResponse>>> GetFixedCharges()
    {
        return Ok(await service.GetFixedEntriesAsync(GetUserId(), EntryType.Charge));
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<EntryResponse>>> CreateFixedCharges(ICollection<EntryRequest> request)
    {
        return Ok(await service.CreateFixedEntriesAsync(GetUserId(), EntryType.Charge, request));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EntryResponse>> UpdateFixedCharges(Guid id, EntryRequest request)
    {
        return Ok(await service.UpdateFixedEntryAsync(GetUserId(), id, request));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedEntryResponse>> DeleteFixedCharges(Guid id)
    {
        return Ok(await service.DeleteFixedEntryAsync(GetUserId(), id));
    }
}