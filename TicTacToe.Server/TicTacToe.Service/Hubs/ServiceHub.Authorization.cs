using Microsoft.AspNetCore.SignalR;
using TicTacToe.Game.Entities;
using TicTacToe.Game.Enums;

namespace TicTacToe.Service.Hubs
{
    public partial class ServiceHub
    {
        public async Task Authorize(string login)
        {
            Symbol symbol = Symbol.X;

            if (players.Count == 1)
            {
                symbol = players[0].Symbol == Symbol.X ? Symbol.O : Symbol.X;
            }

            Player newPlayer = new Player() { ConnectionId = Context.ConnectionId, Login = login, Symbol = symbol };
            players.Add(newPlayer);

            await Clients.Caller.SendAsync("AuthorizationReceived", newPlayer);
        }
    }
}
