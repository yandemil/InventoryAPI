using FluentResults;
using Microsoft.EntityFrameworkCore;
using Inventory.Extensions;
using CustomError = Inventory.Extensions.Error;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));

// Validation automatique
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler(exceptionHandlerApp =>
    exceptionHandlerApp.Run(async context =>
        await Results.Problem().ExecuteAsync(context)));

/* CRUD */

// Récupère toutes les tâches
app.MapGet("/items", async (AppDbContext db) =>
    await db.Items.ToListAsync());

// Tâches par ID
app.MapGet("/items/{id}", async (int id, AppDbContext db) =>
{
    var item = await db.Items.FindAsync(id);

    return item is not null
        ? Results.Ok(item)
        : Result.Fail(CustomError.NotFound("ITEM_404", "Item not in the database")
                .WithMetadata("Field", "id")).ToProblematic();
});

/*
app.MapGet("/items/{id}", async (int id, AppDbContext db) => 
    await db.Items.FindAsync(id) is Item item ? Results.Ok(item) : Results.NotFound());
*/

// POST - Créer une item
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

    return Results.Created($"/items/{newItem.Id}", newItem);
});

// PUT - Modifier
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

// DELETE - Supprimer
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


app.Run();