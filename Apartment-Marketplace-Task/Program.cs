using Apartment_Marketplace_Task.data;
using Apartment_Marketplace_Task.dto_s;
using Apartment_Marketplace_Task.models;
using Apartment_Marketplace_Task.services.interfaces;
using Apartment_Marketplace_Task.validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Apartment_Marketplace_Task.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IApartmentService, ApartmentService>();
builder.Services.AddTransient<IValidator<ApartmentDto>, ApartmentDtoValidator>();
builder.Services.AddDbContext<ApartmentDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});

app.UseHttpsRedirection();

app.MapGet("/apartments", async (IApartmentService service, string priceSort, int? rooms) =>
{
    var apartments = await service.GetApartmentsByFilterAsync(priceSort, rooms);
    return Results.Ok(apartments);
})
.WithName("GetApartments")
.Produces<List<Apartment>>(StatusCodes.Status200OK);

app.MapGet("/apartments/{id}", async (IApartmentService service, string id) =>
{
    var apartment = await service.GetApartmentByIdAsync(id);
    if (apartment == null)
        return Results.NotFound();

    return Results.Ok(apartment);
})
.WithName("GetApartmentById")
.Produces<Apartment>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("/apartments", async (IApartmentService service, ApartmentDto apartmentDto, IValidator<ApartmentDto> validator) =>
{
    var validationResult = await validator.ValidateAsync(apartmentDto);
    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors.Select(e => e.ErrorMessage);
        return Results.BadRequest(errors);
    }

    var apartment = new Apartment
    {
        Id = Guid.NewGuid().ToString(),
        Rooms = apartmentDto.Rooms,
        Name = apartmentDto.Name,
        Price = apartmentDto.Price,
        Description = apartmentDto.Description
    };

    var createdApartment = await service.AddApartmentAsync(apartment);
    return Results.Created($"/apartments/{createdApartment.Id}", createdApartment);
})
.WithName("AddApartment")
.Produces<Apartment>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.WithOpenApi();

app.MapDelete("/apartments/{id}", async (IApartmentService service, string id) =>
{
    var result = await service.DeleteApartmentAsync(id);
    if (!result)
        return Results.NotFound();

    return Results.NoContent();
})
.WithName("DeleteApartment")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.MapPut("/apartments/{id}", async (IApartmentService service, string id, ApartmentDto apartmentDto, IValidator<ApartmentDto> validator) =>
{
    var validationResult = await validator.ValidateAsync(apartmentDto);
    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors.Select(e => e.ErrorMessage);
        return Results.BadRequest(errors);
    }
    
    var result = await service.UpdateApartmentAsync(id, apartmentDto);
    if (result == null)
        return Results.NotFound();

    return Results.Ok(result);
})
.WithName("UpdateApartment")
.Produces<Apartment>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi();

app.Run();
