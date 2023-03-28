namespace ChatService.Repositories
{
    public class ChatMessageRepository : BaseRepository
    {
        public ChatMessageRepository(ChatDbContext context) : base(context)
        {
        }
    }
}
