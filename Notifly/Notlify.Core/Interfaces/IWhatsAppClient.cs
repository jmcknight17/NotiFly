namespace Notlify.Core00.Interfaces;

public interface IWhatsAppClient
{
    Task<bool> SendPriceAlertAsync(string msgBody);
}