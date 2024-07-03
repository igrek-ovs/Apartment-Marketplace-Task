using Microsoft.EntityFrameworkCore;
using Apartment_Marketplace_Task.data;
using Apartment_Marketplace_Task.dto_s;
using Apartment_Marketplace_Task.models;
using Apartment_Marketplace_Task.services.interfaces;

namespace Apartment_Marketplace_Task.Services
{
    public class ApartmentService : IApartmentService
    {
        private readonly ApartmentDbContext _context;

        public ApartmentService(ApartmentDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Apartment> GetApartmentByIdAsync(string id)
        {
            return await _context.Apartments.FindAsync(id);
        }

        public async Task<List<Apartment>> GetApartmentsByFilterAsync(string priceSort, int? rooms)
        {
            var query = _context.Apartments.AsQueryable();

            if (rooms.HasValue)
            {
                query = query.Where(ap => ap.Rooms == rooms.Value);
            }

            if (!string.IsNullOrEmpty(priceSort))
            {
                if (priceSort.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderBy(a => a.Price);
                }
                else if (priceSort.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderByDescending(a => a.Price);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<Apartment> AddApartmentAsync(Apartment apartment)
        {
            apartment.Id = Guid.NewGuid().ToString();
            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();
            return apartment;
        }

        public async Task<bool> DeleteApartmentAsync(string id)
        {
            var apartmentToDelete = await _context.Apartments.FindAsync(id);
            if (apartmentToDelete == null)
                return false;

            _context.Apartments.Remove(apartmentToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Apartment> UpdateApartmentAsync(string id, ApartmentDto updatedApartment)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
                return null;

            apartment.Rooms = updatedApartment.Rooms;
            apartment.Name = updatedApartment.Name;
            apartment.Price = updatedApartment.Price;
            apartment.Description = updatedApartment.Description;

            await _context.SaveChangesAsync();

            return apartment;
        }
    }
}
