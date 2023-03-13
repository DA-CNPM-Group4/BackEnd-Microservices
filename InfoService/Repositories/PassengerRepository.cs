using InfoService.Models;

namespace InfoService.Repositories
{
    public class PassengerRepository : BaseRepository
    {
        public PassengerRepository(InfoDbContext context) : base(context)
        {
        }

        public async Task<int> AddPassengerInfo(Passenger passenger)
        {
            await context.Passenger.AddAsync(passenger);
            return await context.SaveChangesAsync();
        }
    }
}
