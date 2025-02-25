using System.Windows;
using TicTacToe.ServerClient.Entities;

namespace TicTacToe.Interface
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            ServerClient.ServerClient.AuthorizationReceived += OnAuthorizationReceived;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await ServerClient.ServerClient.Authorize(loginTextBox.Text);
        }

        private void OnAuthorizationReceived(Player newPlayer)
        {
            Dispatcher.Invoke(() =>
            {
                BoardWindow boardWindow = new BoardWindow(newPlayer);

                Close();
                boardWindow.Show();
            });
        }
    }
}
