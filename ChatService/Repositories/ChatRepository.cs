namespace ChatService.Repositories
{
    public class ChatRepository : BaseRepository
    {
        public ChatRepository(ChatDbContext context) : base(context)
        {
        }
    }
}
