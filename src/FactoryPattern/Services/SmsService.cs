using FactoryPattern.Interfaces;

namespace FactoryPattern.Services;

public class SmsService : IMessageService
{
    public void Send(string recipient, string message) => 
        Console.WriteLine($"[SMS] Sending to {recipient}: {message}");
}