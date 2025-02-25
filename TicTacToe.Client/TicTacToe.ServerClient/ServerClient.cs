using Microsoft.AspNetCore.SignalR.Client;
using TicTacToe.ServerClient.Entities;
using TicTacToe.ServerClient.Enums;

namespace TicTacToe.ServerClient
{
    public partial class ServerClient
    {
        private readonly static HubConnection connection = new HubConnectionBuilder().WithUrl("http://localhost:61234/Game").Build();

        public static event Action GameResetBroadcasted;
        public static event Action<Player> AuthorizationReceived;
        public static event Action<Symbol> VictoryChecked;
        public static event Action<bool> MoveConfirmed;
        public static event Action<bool> DrawChecked;
        public static event Action<int, int, Symbol> MoveBroadcasted;
        public static event Action<Symbol> VictoryBroadcasted;
        public static event Action DrawBroadcasted;
        public static event Action<Symbol> TurnBroadcasted;
        public static event Action<List<Player>> PlayerListBroadcasted;
        public static event Action GameStartedBroadcasted;
        public static event Action GameBlockedBroadcasted;

        public static async Task Connect()
        {
            await connection.StartAsync();
        }

        public static async Task Disconnect()
        {
            await connection.StopAsync();
        }

        public static void SubscribeToEvents()
        {
            connection.On("GameResetBroadcasted", () =>
            {
                GameResetBroadcasted?.Invoke();
            });

            connection.On<int, int, Symbol>("MoveBroadcasted", (row, col, symbol) =>
            {
                MoveBroadcasted?.Invoke(row, col, symbol); 
            });

            connection.On<Symbol>("VictoryBroadcasted", (symbol) =>
            {
                VictoryBroadcasted?.Invoke(symbol); 
            });

            connection.On("DrawBroadcasted", () =>
            {
                DrawBroadcasted?.Invoke(); 
            });

            connection.On<Symbol>("TurnBroadcasted", (symbol) =>
            {
                TurnBroadcasted?.Invoke(symbol);
            });

            connection.On<List<Player>>("PlayerListBroadcasted", (players) =>
            {
                PlayerListBroadcasted?.Invoke(players);
            });

            connection.On("GameStartedBroadcasted", () =>
            {
                GameStartedBroadcasted?.Invoke();
            });

            connection.On("GameBlockedBroadcasted", () =>
            {
                GameBlockedBroadcasted?.Invoke();
            });

            connection.On<Player>("AuthorizationReceived", (newPlayer) =>
            {
                AuthorizationReceived?.Invoke(newPlayer);
            });

            connection.On<Symbol>("VictoryChecked", (victorysSymbol) =>
            {
                VictoryChecked?.Invoke(victorysSymbol);
            });

            connection.On<bool>("DrawChecked", (isDraw) =>
            {
                DrawChecked?.Invoke(isDraw);
            });

            connection.On<bool>("MoveConfirmed", (moveConfirmed) =>
            {
                MoveConfirmed?.Invoke(moveConfirmed);
            });
        }

        public static void UnsubscribeToEvents()
        {
            connection.Remove("GameResetBroadcasted");
            connection.Remove("MoveBroadcasted");
            connection.Remove("VictoryBroadcasted");
            connection.Remove("DrawBroadcasted");
            connection.Remove("TurnBroadcasted");
            connection.Remove("PlayerListBroadcasted");
            connection.Remove("GameStartedBroadcasted");
            connection.Remove("GameBlockedBroadcasted");
            connection.Remove("AuthorizationReceived");
            connection.Remove("MoveConfirmed");
            connection.Remove("VictoryChecked");
            connection.Remove("DrawChecked");
        }
    }
}
