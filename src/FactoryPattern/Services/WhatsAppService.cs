using FactoryPattern.Interfaces;

namespace FactoryPattern.Services;

public class WhatsAppService : IMessageService
{
    public void Send(string recipient, string message) => 
        Console.WriteLine($"[WhatsApp] Sending to {recipient}: {message}");
}