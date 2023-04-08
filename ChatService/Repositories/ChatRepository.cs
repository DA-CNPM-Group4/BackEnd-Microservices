using ChatService.DTOs;
using ChatService.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Repositories
{
    public class ChatRepository : BaseRepository
    {
        private readonly FireStoreService _storeService;
        public ChatRepository(ChatDbContext context) : base(context)
        {
            _storeService = new FireStoreService();
        }

        public async Task<ChatResponseDTO> GetChat(string tripId)
        {
            
            Chat chat = await context.Chat.FindAsync(tripId);
            List<ChatMessage> messages = await context.ChatMessage.Where(m => m.TripId == Guid.Parse(tripId)).ToListAsync();
            ChatResponseDTO chatResponseDTO = new ChatResponseDTO()
            {
                DriverId = chat.DriverId,
                PassengerId = chat.PassengerId,
                TripId = chat.TripId,
                Messages = messages,
                TripCreatedTime = chat.TripCreatedTime
            };
            return chatResponseDTO;
        }

        public async Task<int> ClearTable()
        {
            context.RemoveRange(context.Chat);
            return await context.SaveChangesAsync();
        }
    }
}
