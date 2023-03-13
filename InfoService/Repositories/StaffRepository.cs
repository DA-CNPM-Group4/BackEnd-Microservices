using InfoService.Models;

namespace InfoService.Repositories
{
    public class StaffRepository : BaseRepository
    {
        public StaffRepository(InfoDbContext context) : base(context)
        {
        }

        public async Task<int> AddStaffInfo(Staff staff)
        {
            await context.Staff.AddAsync(staff);
            return await context.SaveChangesAsync();
        }
    }
}
