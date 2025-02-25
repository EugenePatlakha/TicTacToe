using Microsoft.AspNetCore.SignalR;
using TicTacToe.Game.Entities;

namespace TicTacToe.Service.Hubs
{
    public partial class ServiceHub : Hub
    {
        private static List<Player> players = new List<Player>();
        private const int MaxPlayers = 2;

        public override Task OnConnectedAsync()
        {
            if (players.Count >= MaxPlayers)
            {
                throw new InvalidOperationException("The game is already full. Try again later.");
            }

            Console.WriteLine("Сlient " + Context.ConnectionId + " connected");
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            int removedPlayers = players.RemoveAll(p => p.ConnectionId == Context.ConnectionId);
            if (removedPlayers > 0)
            {
                await Clients.All.SendAsync("PlayerListBroadcasted", players);

                if (players.Count < MaxPlayers)
                {
                    await ResetGame();
                    await Clients.All.SendAsync("GameBlockedBroadcasted");
                }
            }
            
            Console.WriteLine("Сlient " + Context.ConnectionId + " disconnected");
            await base.OnDisconnectedAsync(ex);
        }
    }
}
