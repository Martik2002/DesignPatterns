using FactoryPattern.Interfaces;

namespace FactoryPattern.Services;

public class TelegramService : IMessageService
{
    public void Send(string recipient, string message) => 
        Console.WriteLine($"[Telegram] Sending to {recipient}: {message}");
}