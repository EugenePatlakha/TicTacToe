using TicTacToe.Game.Enums;
using TicTacToe.Game;
using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.Service.Hubs
{
    public partial class ServiceHub
    {
        private static GameLogic gameLogic = new GameLogic();
        private static Dictionary<string, bool> playersDecisions = new Dictionary<string, bool>();

        public async Task ResetGame()
        {
            gameLogic.ResetGame();
            await Clients.All.SendAsync("GameResetBroadcasted");
        }
            
        public async Task MakeMove(int row, int col, Symbol symbol)
        {
            bool moveConfirmed = gameLogic.MakeMove(row, col, symbol);
            if (moveConfirmed)
            {
                await Clients.All.SendAsync("MoveBroadcasted", row, col, symbol);
            }
            await Clients.Caller.SendAsync("MoveConfirmed", moveConfirmed);
        }

        public async Task CheckWin()
        {
            Symbol victorysSymbol= gameLogic.CheckWin();
            if (victorysSymbol != Symbol.None)
            {
                await Clients.All.SendAsync("VictoryBroadcasted", victorysSymbol);
            }
            await Clients.Caller.SendAsync("VictoryChecked", victorysSymbol);
        }

        public async Task CheckDraw()
        {
            bool isDraw = gameLogic.CheckDraw();
            if (isDraw)
            {
                await Clients.All.SendAsync("DrawBroadcasted");
            }
            await Clients.Caller.SendAsync("DrawChecked", isDraw);
        }

        public async Task UpdateTurn()
        {
            Symbol turn = gameLogic.CheckTurn();
            await Clients.All.SendAsync("TurnBroadcasted", turn);
        }

        public async Task GetPlayers()
        {
            await Clients.All.SendAsync("PlayerListBroadcasted", players);

            if (players.Count > 1)
            {
                await UpdateTurn();
                await Clients.All.SendAsync("GameStartedBroadcasted");
            }
        }

        public async Task PlayerWantsToContinue(bool wantsToContinue)
        {
            string playerId = Context.ConnectionId;
            playersDecisions[playerId] = wantsToContinue;

            if (playersDecisions.Count == 2)
            {
                if (playersDecisions.Values.All(x => x))
                {
                    await UpdateTurn();
                    await Clients.All.SendAsync("GameStartedBroadcasted");
                }
                playersDecisions.Clear();
            }
        }
    }
}
