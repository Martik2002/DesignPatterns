using FactoryPattern.Enums;
using FactoryPattern.Factory;

Console.WriteLine("=== Notification Factory Pattern Demo ===");

// Example 1: Creating an SMS Service
var smsService = MessageFactory.CreateService(MessageType.Sms);
smsService.Send("+1234567890", "This is a secure authentication code: 5542.");

// Example 2: Creating a Telegram Service
var telegramService = MessageFactory.CreateService(MessageType.Telegram);
telegramService.Send("@dev_community", "New deployment successful in Production environment.");

// Example 3: Batch processing different notification types
var servicesToTest = new List<MessageType> 
{ 
    MessageType.Email, 
    MessageType.WhatsApp 
};

Console.WriteLine("\n--- Processing Batch Notifications ---");

foreach (var type in servicesToTest)
{
    var service = MessageFactory.CreateService(type);
    service.Send("system.administrator@example.com", $"Alert: Storage capacity is at 90% for {type} gateway.");
}

Console.WriteLine("\n=== Demo Completed Successfully ===");