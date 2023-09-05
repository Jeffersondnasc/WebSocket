using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using static Web_Socket_Web_App.Program;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatController(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.User, message.Text);
        return Ok();
    }

    public class ChatMessage
    {
        public string User { get; set; }
        public string Text { get; set; }
    }
}

