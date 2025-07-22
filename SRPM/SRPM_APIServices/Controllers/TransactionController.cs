using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController: Controller
{
    //=================================[ Declares ]================================
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    //=================================[ Endpoints ]================================
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] RQ_Transaction inputData)
    {
        var result = await _transactionService.NewTransaction(inputData);
        return result.result ? Created(nameof(Add), result.transactionId) : BadRequest("Create Failed!");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var transaction = await _transactionService.GetTransactionById(id);
        return transaction != null ? Ok(transaction) : NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RQ_Transaction inputData)
    {
        var result = await _transactionService.UpdateTransaction(id, inputData);
        return result ? Ok("Update Successfully!") : BadRequest("Update Failed!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _transactionService.DeleteTransaction(id);
        return result ? Ok("Deleted Successfully!") : NotFound("Delete Failed!");
    }
}