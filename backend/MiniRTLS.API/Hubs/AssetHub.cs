using Microsoft.AspNetCore.SignalR;

namespace MiniRTLS.API.Hubs
{
    public class AssetHub : Hub
    {
        public async Task SubscribeToAsset(string assetId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, assetId);
        }

        public async Task UnsubscribeFromAsset(string assetId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, assetId);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            // İstersen bağlantı logu veya ilk veri gönderimi yapılabilir
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            // Temizlik işlemleri vs
        }
    }
}
