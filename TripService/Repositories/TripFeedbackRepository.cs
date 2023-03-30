namespace TripService.Repositories
{
    public class TripFeedbackRepository : BaseRepository
    {
        public TripFeedbackRepository(TripDbContext context) : base(context)
        {
        }

        public async Task<int> ClearTable()
        {
            context.RemoveRange(context.TripFeedback);
            return await context.SaveChangesAsync();
        }
    }
}
