using Apartment_Marketplace_Task.dto_s;
using Apartment_Marketplace_Task.models;

namespace Apartment_Marketplace_Task.services.interfaces;

public interface IApartmentService
{
    Task<Apartment> GetApartmentByIdAsync(string id);
    Task<List<Apartment>> GetApartmentsByFilterAsync(string priceSort, int? rooms);
    Task<Apartment> AddApartmentAsync(Apartment apartment);
    Task<bool> DeleteApartmentAsync(string id);
    Task<Apartment> UpdateApartmentAsync(string id, ApartmentDto updatedApartment);
}