using Notifly.Service.Models;

namespace Notifly.Builders;

public interface IWhatsAppNotificationBuilder
{
    string BuildNotificationMessage(List<FlightInfo> flightInfo);
}
public class WhatsAppNotificationBuilder
{
    public string BuildNotificationMessage(List<FlightInfo> flightInfo)
    {
        return "Test message";
    }
}