using Microsoft.AspNetCore.SignalR.Client;

namespace TicTacToe.ServerClient
{
    public partial class ServerClient
    {
        public static async Task Authorize(string login)
        {
            await connection.InvokeAsync("Authorize", login);
        }
    }
}
