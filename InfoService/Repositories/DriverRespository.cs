using InfoService.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoService.Repositories
{
    public class DriverRespository : BaseRepository
    {
        public DriverRespository(InfoDbContext context) : base(context)
        {
        }

        public async Task<int> AddDriverInfo(Driver driver)
        {
            await context.Driver.AddAsync(driver);
            return await context.SaveChangesAsync();
        }

        public async Task<List<Driver>> GetAllDrivers()
        {
            return await context.Driver.ToListAsync();
        }
    }
}
