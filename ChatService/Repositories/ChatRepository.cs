namespace ChatService.Repositories
{
    public class ChatRepository : BaseRepository
    {
        public ChatRepository(ChatDbContext context) : base(context)
        {

        }

        public async Task<int> ClearTable()
        {
            context.RemoveRange(context.Chat);
            return await context.SaveChangesAsync();
        }
    }
}
