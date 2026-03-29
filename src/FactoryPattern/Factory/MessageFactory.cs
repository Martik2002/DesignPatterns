using FactoryPattern.Enums;
using FactoryPattern.Interfaces;
using FactoryPattern.Services;

namespace FactoryPattern.Factory;
/// <summary>
/// A static factory responsible for instantiating the appropriate message service 
/// based on the provided MessageType. This implementation promotes loose coupling 
/// by hiding the instantiation logic from the client code.
/// </summary>
public static class MessageFactory
{
    public static IMessageService CreateService(MessageType type)
    {
        return type switch
        {
            MessageType.Sms => new SmsService(),
            MessageType.Email => new EmailService(),
            MessageType.WhatsApp => new WhatsAppService(),
            MessageType.Telegram => new TelegramService(),
            _ => throw new NotSupportedException($"Service type {type} is not supported.")
        };
    }
}