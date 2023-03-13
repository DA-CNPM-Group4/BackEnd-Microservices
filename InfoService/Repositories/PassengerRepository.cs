using InfoService.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoService.Repositories
{
    public class PassengerRepository : BaseRepository
    {
        public PassengerRepository(InfoDbContext context) : base(context)
        {
        }
        public async Task<List<Passenger>> GetAllPassengers()
        {
            return await context.Passenger.ToListAsync();
        }
        public async Task<int> AddPassengerInfo(Passenger passenger)
        {
            await context.Passenger.AddAsync(passenger);
            return await context.SaveChangesAsync();
        }

        public async Task<Passenger> GetPassengerById(Guid AccountId)
        {
            return await context.Passenger.FindAsync(AccountId);
        }

        public async Task<int> UpdatePassengerInfo(Passenger passenger)
        {
            Passenger destination = await context.Passenger.FindAsync(passenger.AccountId);

            //destination.Phone = source.Phone;
            //destination.Email = source.Email;

            destination.Name = passenger.Name;
            destination.Gender = passenger.Gender;
            return await context.SaveChangesAsync();
        }
        public async Task<bool> CheckPassengerExist(Guid AccountId)
        {
            return await context.Passenger.AnyAsync(p => p.AccountId == p.AccountId);
        }

    }
}
