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

        public async Task<int> UpdateDriverInfo(Driver driver)
        {
            Driver destination = await context.Driver.FindAsync(driver.AccountId);

            //destination.Phone = source.Phone;
            //destination.Email = source.Email;
            destination.IdentityNumber = driver.IdentityNumber;
            destination.Name = driver.Name;
            destination.Gender = driver.Gender;
            destination.Address = driver.Address;
            return await context.SaveChangesAsync();
        }
        public async Task<bool> CheckDriverExist(Guid AccountId)
        {
            return await context.Driver.AnyAsync(p => p.AccountId == p.AccountId);
        }

        public async Task<List<Driver>> GetAllDrivers()
        {
            return await context.Driver.ToListAsync();
        }

        public async Task<Driver> GetDriverById(Guid AccountId)
        {
            return await context.Driver.FindAsync(AccountId);
        }

        public async Task<int> MarkAlreadyRegisVehicle(Guid driverId)
        {
            Driver driver = await context.Driver.FindAsync(driverId);
            if(driver != null)
            {
                driver.HaveVehicleRegistered = true;
                return await context.SaveChangesAsync();
            }
            return -1;
        }

    }
}
