using Helper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers
{
    [Route("api/Chat/[controller]/[action]")]
    [ApiController]
    public class ChatController : BaseController
    {
        [HttpGet]
        public async Task<ResponseMsg> ClearDb()
        {
            await Repository.Chat.ClearTable();
            await Repository.ChatMessage.ClearTable();
            return new ResponseMsg
            {
                status = true,
                data = null,
                message = "Executed clear Chat services Db"
            };
        }
    }
}
