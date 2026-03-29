using FactoryPattern.Interfaces;

namespace FactoryPattern.Services;

public class EmailService : IMessageService
{
    public void Send(string recipient, string message) => 
        Console.WriteLine($"[Email] Sending to {recipient}: {message}");
}