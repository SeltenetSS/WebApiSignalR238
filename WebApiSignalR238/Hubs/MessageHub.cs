using Microsoft.AspNetCore.SignalR;
using WebApiSignalR238.Services;
using System.Threading.Tasks;

namespace WebApiSignalR238.Hubs
{
    public class MessageHub : Hub
    {
        private readonly IFileService _fileService;
        private static string currentBidder = null;
        private static double currentOffer = 0;
        private static bool isBiddingActive = false;

        public MessageHub(IFileService fileService)
        {
            _fileService = fileService;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Others.SendAsync("ReceiveConnectInfo", "User Connected");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.Others.SendAsync("ReceiveDisconnectInfo", "User Disconnected");
        }

        public async Task SendMessage(string user, double data)
        {
            if (!isBiddingActive)
            {
                currentBidder = user;
                currentOffer = data;
                isBiddingActive = true;
                await Clients.All.SendAsync("ReceiveMessage", user + "'s Offer is ", data);
                await Clients.Others.SendAsync("BidPlaced", user, data);
                await Task.Delay(10000); 
                if (currentBidder != null)
                {
                    await Clients.All.SendAsync("ReceiveMessage", currentBidder + " won with the offer of ", currentOffer);
                    isBiddingActive = false;
                }
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Bid time expired for you! Please wait.");
            }
        }
    }
}
