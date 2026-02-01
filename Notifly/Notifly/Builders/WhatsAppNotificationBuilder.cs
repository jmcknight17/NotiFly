using System.Text;
using Notifly.Service.Models;

namespace Notifly.Builders;

public interface IWhatsAppNotificationBuilder
{
    string BuildNotificationMessage(List<FlightInfo> flightInfo);
}
public class WhatsAppNotificationBuilder : IWhatsAppNotificationBuilder
{
    public string BuildNotificationMessage(List<FlightInfo> flightInfo)
    {
        var sb = new StringBuilder();

        sb.AppendLine("🚨 *FLIGHT PRICE ALERT* 🚨");
        sb.AppendLine($"Here are the top {flightInfo.Count} deals matching your budget:");
        sb.AppendLine(""); 
        
        int rank = 1;
        foreach (var deal in flightInfo)
        {
            string medal = rank switch
            {
                1 => "🥇",
                2 => "🥈",
                3 => "🥉",
                _ => "💰"
            };
            
            string stopText = deal.stops == 0 ? "Direct" : $"{deal.stops} Stop(s)";
            
            string duration = FormatDuration(deal.Duration);

            sb.AppendLine($"{medal} *£{deal.Price:F0}* | {deal.Airline}");
            sb.AppendLine($"⏱ {duration} | 🛑 {stopText}");
            sb.AppendLine("-----------------------------");

            rank++;
        }

        sb.AppendLine("👉 _Check your booking app for details_");

        return sb.ToString();
    }
    
    private string FormatDuration(string isoDuration)
    {
        if (string.IsNullOrEmpty(isoDuration)) return "N/A";
        return isoDuration.Replace("PT", "").Replace("H", "h ").Replace("M", "m").ToLower();
    }
}