using InfoService.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoService.Repositories
{
    public class StaffRepository : BaseRepository
    {
        public StaffRepository(InfoDbContext context) : base(context)
        {
        }

        public async Task<List<Staff>> GetAllStaffs()
        {
            return await context.Staff.ToListAsync();
        }

        public async Task<int> AddStaffInfo(Staff staff)
        {
            await context.Staff.AddAsync(staff);
            return await context.SaveChangesAsync();
        }

        public async Task<Staff> GetStaffById(Guid AccountId)
        {
            return await context.Staff.FindAsync(AccountId);
        }

        public async Task<int> UpdateStaffInfo(Staff staff)
        {
            Staff destination = await context.Staff.FindAsync(staff.AccountId);

            //destination.Phone = source.Phone;
            //destination.Email = source.Email;

            destination.IdentityNumber = staff.IdentityNumber;
            destination.Name = staff.Name;
            destination.Gender = staff.Gender;
            destination.Address = staff.Address;
            return await context.SaveChangesAsync();
        }

        public async Task<bool> CheckStaffExist(Guid AccountId)
        {
            return await context.Staff.AnyAsync(p => p.AccountId == p.AccountId);
        }
    }
}
