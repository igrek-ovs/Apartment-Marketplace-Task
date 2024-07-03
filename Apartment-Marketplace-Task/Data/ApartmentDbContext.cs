using Apartment_Marketplace_Task.models;
using Microsoft.EntityFrameworkCore;

namespace Apartment_Marketplace_Task.data;

public class ApartmentDbContext : DbContext
{
    public ApartmentDbContext(DbContextOptions<ApartmentDbContext> options) : base(options)
    {
    }

    public DbSet<Apartment> Apartments { get; set; }
}