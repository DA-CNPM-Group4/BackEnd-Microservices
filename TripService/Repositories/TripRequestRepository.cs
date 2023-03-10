namespace TripService.Repositories
{
    public class TripRequestRepository : BaseRepository
    {
        public TripRequestRepository(TripDbContext context) : base(context)
        {
        }
    }
}
