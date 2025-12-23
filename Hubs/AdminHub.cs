using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SoruCevapPortal.Hubs;

[Authorize(Roles = "Admin")]
public class AdminHub : Hub
{
    private readonly ILogger<AdminHub> _logger;

    public AdminHub(ILogger<AdminHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Admin connected: {ConnectionId}", Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        await Clients.Group("Admins").SendAsync("AdminConnected", Context.User?.Identity?.Name);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Admin disconnected: {ConnectionId}", Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
        await Clients.Group("Admins").SendAsync("AdminDisconnected", Context.User?.Identity?.Name);
        await base.OnDisconnectedAsync(exception);
    }

    // Yeni soru bildirimi
    public async Task NotifyNewQuestion(int questionId, string title, string category)
    {
        await Clients.Group("Admins").SendAsync("NewQuestion", new
        {
            QuestionId = questionId,
            Title = title,
            Category = category,
            Time = DateTime.Now.ToString("HH:mm")
        });
    }

    // Yeni cevap bildirimi
    public async Task NotifyNewAnswer(int answerId, int questionId, string questionTitle)
    {
        await Clients.Group("Admins").SendAsync("NewAnswer", new
        {
            AnswerId = answerId,
            QuestionId = questionId,
            QuestionTitle = questionTitle,
            Time = DateTime.Now.ToString("HH:mm")
        });
    }

    // Yeni kullanıcı kaydı bildirimi
    public async Task NotifyNewUser(string username, string email)
    {
        await Clients.Group("Admins").SendAsync("NewUser", new
        {
            Username = username,
            Email = email,
            Time = DateTime.Now.ToString("HH:mm")
        });
    }

    // Genel bildirim gönder
    public async Task SendNotification(string message, string type = "info")
    {
        await Clients.Group("Admins").SendAsync("Notification", new
        {
            Message = message,
            Type = type,
            Time = DateTime.Now.ToString("HH:mm:ss")
        });
    }

    // Dashboard istatistiklerini güncelle
    public async Task UpdateDashboardStats(object stats)
    {
        await Clients.Group("Admins").SendAsync("DashboardUpdate", stats);
    }
}

// SignalR'dan bildirim göndermek için servis
public interface IAdminNotificationService
{
    Task NotifyNewQuestionAsync(int questionId, string title, string category);
    Task NotifyNewAnswerAsync(int answerId, int questionId, string questionTitle);
    Task NotifyNewUserAsync(string username, string email);
    Task SendNotificationAsync(string message, string type = "info");
}

public class AdminNotificationService : IAdminNotificationService
{
    private readonly IHubContext<AdminHub> _hubContext;

    public AdminNotificationService(IHubContext<AdminHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyNewQuestionAsync(int questionId, string title, string category)
    {
        await _hubContext.Clients.Group("Admins").SendAsync("NewQuestion", new
        {
            QuestionId = questionId,
            Title = title,
            Category = category,
            Time = DateTime.Now.ToString("HH:mm")
        });
    }

    public async Task NotifyNewAnswerAsync(int answerId, int questionId, string questionTitle)
    {
        await _hubContext.Clients.Group("Admins").SendAsync("NewAnswer", new
        {
            AnswerId = answerId,
            QuestionId = questionId,
            QuestionTitle = questionTitle,
            Time = DateTime.Now.ToString("HH:mm")
        });
    }

    public async Task NotifyNewUserAsync(string username, string email)
    {
        await _hubContext.Clients.Group("Admins").SendAsync("NewUser", new
        {
            Username = username,
            Email = email,
            Time = DateTime.Now.ToString("HH:mm")
        });
    }

    public async Task SendNotificationAsync(string message, string type = "info")
    {
        await _hubContext.Clients.Group("Admins").SendAsync("Notification", new
        {
            Message = message,
            Type = type,
            Time = DateTime.Now.ToString("HH:mm:ss")
        });
    }
}

