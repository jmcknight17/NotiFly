using Notlify.Core00.Interfaces;

namespace Notlify.Core00;

public class WhatsAppClient : IWhatsAppClient
{
    public Task<bool> SendPriceAlertAsync(string msgBody)
    {
        throw new NotImplementedException();
    }
}