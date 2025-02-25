using System.Windows;

namespace TicTacToe.Interface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ServerClient.ServerClient.SubscribeToEvents();

            await ServerClient.ServerClient.Connect();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            ServerClient.ServerClient.UnsubscribeToEvents();

            await ServerClient.ServerClient.Disconnect();

            base.OnExit(e);
        }
    }
}
