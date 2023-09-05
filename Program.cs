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


using Microsoft.AspNetCore.SignalR;

namespace Web_Socket_Web_App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddControllers();
            builder.Services.AddSignalR(); // Adiciona o serviço SignalR

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseDeveloperExceptionPage();


            app.UseRouting();

            app.UseCors(builder => builder
                .WithOrigins("http://localhost:3000") // Permite conexões apenas a partir deste domínio
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat"); // Define a rota para o hub do SignalR
            });


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }

        public class ChatHub : Hub
        {
            public async Task SendMessage(string user, string message)
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
        }

    }
}
