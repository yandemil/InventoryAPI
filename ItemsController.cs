namespace Inventory.Extensions;

using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CustomError = Inventory.Extensions.Error;

[ApiController]
[Microsoft.AspNetCore.Components.Route("Inventory/ItemsController")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ItemsController (AppDbContext db) => _db = db;

    // Récupère toutes les tâches
    /*
    app.MapGet("/items", async (AppDbContext db) =>
        await db.Items.ToListAsync());
        */
    
    [HttpGet]
    public async Task<IActionResult> GetAllItem()
    {
        return Ok(await _db.Items.ToListAsync());
    }

    /*
    // Tâches par ID
    app.MapGet("/items/{id}", async (int id, AppDbContext db) =>
    {
    var item = await db.Items.FindAsync(id);

        return item is not null
            ? Results.Ok(item)
            : Result.Fail(CustomError.NotFound("ITEM_404", "Item not in the database")
                    .WithMetadata("Field", "id")).ToProblematic();
    });
    */    
    [HttpGet]
    public async Task<IActionResult> SearchItem(int id)
    {
        var item = await _db.Items.FindAsync(id);

        // Build the Result object
        Result result = CustomError.NotFound("ITEM_404", "Item not in the database")
                    .WithMetadata("Field", "id");

        // Convert to ProblemDetails
        var problem = result.ToProblematicAction();

        if (problem is null)
            return StatusCode(500, new ProblemDetails
            {
                Title = "Unhandled Error!",
                Status = 500
            });

        return item is not null
            ? Ok(item)
            : StatusCode(problem.Status ?? 400, problem);
    }

    // POST - Créer une item
    /*
    app.MapPost("/items", async (Item item, AppDbContext db) =>
    {
        Result validationResult = ItemValidator.Validate(item);
        if (validationResult.IsFailed)
            return validationResult.ToProblematic();

        var newItem = new Item
        {
            Name = item.Name,
            Price = item.Price,
            Quantity = item.Quantity
        };

        db.Items.Add(newItem);
        await db.SaveChangesAsync();

        return Results.Created($"/items/{item.Id}", item);
    });
    */
    [HttpPost]
    public async Task<IActionResult> PostItem(Item item)
    {
        Result validationResult = ItemValidator.Validate(item);
        if (validationResult.IsFailed)
            return (IActionResult)validationResult.ToProblematic();

        var newItem = new Item
        {
            Name = item.Name,
            Price = item.Price,
            Quantity = item.Quantity
        };

        _db.Items.Add(newItem);
        await _db.SaveChangesAsync();

        return Created($"/items/{newItem.Id}", item);
    }

    // PUT - Modifier
    /*
    app.MapPut("/items/{id}", async (int id, Item inputItem, AppDbContext db) =>
    {
        var item = await db.Items.FindAsync(id);
        if (item is null) return Results.NotFound();

        item.Name = inputItem.Name;
        item.Quantity = inputItem.Quantity;
        item.Price = inputItem.Price;

        await db.SaveChangesAsync();

        return Results.NoContent();
    });
    */
    [HttpPut]
    public async Task<IActionResult> PutItem(int id, Item inputItem)
    {
        var item = await _db.Items.FindAsync(id);
        if (item is null) return NotFound();

        item.Name = inputItem.Name;
        item.Quantity = inputItem.Quantity;
        item.Price = inputItem.Price;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE - Supprimer
    /*
    app.MapDelete("/items/{id}", async (int id, AppDbContext db) =>
    {
        if (await db.Items.FindAsync(id) is Item item)
        {
            db.Items.Remove(item);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        // return Results.NotFound();
        return Result.Fail(CustomError.NotFound("ITEM_404", "Item not in the database")
                    .WithMetadata("Field", "id"))
                    .ToProblematic();
    });
    */
    [HttpDelete]
    public async Task<IActionResult> DeleteItem(int id)
    {
        if (await _db.Items.FindAsync(id) is Item item)
        {
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Build the Result object
        Result result = CustomError.NotFound("ITEM_404", "Item not in the database")
                    .WithMetadata("Field", "id");

        // Convert to ProblemDetails
        var problem = result.ToProblematicAction();

        if (problem is null)
            return StatusCode(500, new ProblemDetails
            {
                Title = "Unhandled Error!",
                Status = 500
            });

        return StatusCode(problem.Status ?? 400, problem);
    }

} 
