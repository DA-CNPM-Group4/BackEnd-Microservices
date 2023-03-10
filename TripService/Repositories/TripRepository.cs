namespace TripService.Repositories
{
    public class TripRepository : BaseRepository
    {
        public TripRepository(TripDbContext context) : base(context)
        {
        }
    }
}
