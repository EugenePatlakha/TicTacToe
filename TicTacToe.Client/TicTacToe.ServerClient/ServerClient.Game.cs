using Microsoft.AspNetCore.SignalR.Client;
using TicTacToe.ServerClient.Enums;

namespace TicTacToe.ServerClient
{
    public partial class ServerClient
    {
        public static async Task ResetGame()
        {
            await connection.InvokeAsync("ResetGame");
        }

        public static async Task GetPlayers()
        {
            await connection.InvokeAsync("GetPlayers");
        }

        public static async Task MakeMove(int row, int col, Symbol symbol)
        {
            await connection.InvokeAsync("MakeMove", row, col, symbol);
        }

        public static async Task CheckWin()
        {
            await connection.InvokeAsync("CheckWin");
        }

        public static async Task CheckDraw()
        {
            await connection.InvokeAsync("CheckDraw");
        }

        public static async Task UpdateTurn()
        {
            await connection.InvokeAsync("UpdateTurn");
        }

        public static async Task PlayerWantsToContinue(bool wantsToContinue)
        {
            await connection.InvokeAsync("PlayerWantsToContinue", wantsToContinue);
        }
    }
}
