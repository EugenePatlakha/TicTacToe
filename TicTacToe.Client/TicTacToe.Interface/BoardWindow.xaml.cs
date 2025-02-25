using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TicTacToe.ServerClient.Entities;
using TicTacToe.ServerClient.Enums;

namespace TicTacToe.Interface
{
    /// <summary>
    /// Interaction logic for BoardWindow.xaml
    /// </summary>
    public partial class BoardWindow : Window
    {
        private Player currentPlayer;
        private List<Player> players;
        private Dictionary<Tuple<int, int>, Symbol> moves;

        private double cellSize;

        public BoardWindow(Player currentPlayer)
        {
            InitializeComponent();

            this.currentPlayer = currentPlayer;
            moves = new Dictionary<Tuple<int, int>, Symbol>();

            BlockGame();
            GetPlayers();
            DrawGrid();

            ServerClient.ServerClient.GameResetBroadcasted += OnGameResetBroadcasted;
            ServerClient.ServerClient.MoveBroadcasted += OnMoveBroadcasted;
            ServerClient.ServerClient.MoveConfirmed += OnMoveConfirmed;
            ServerClient.ServerClient.VictoryChecked += OnVictoryChecked;
            ServerClient.ServerClient.DrawChecked += OnDrawChecked;
            ServerClient.ServerClient.VictoryBroadcasted += OnVictoryBroadcasted;
            ServerClient.ServerClient.DrawBroadcasted += OnDrawBroadcasted;
            ServerClient.ServerClient.TurnBroadcasted += OnTurnBroadcasted;
            ServerClient.ServerClient.PlayerListBroadcasted += OnPlayerListBroadcasted;
            ServerClient.ServerClient.GameStartedBroadcasted += OnGameStartedBroadcasted;
            ServerClient.ServerClient.GameBlockedBroadcasted += OnGameBlockedBroadcasted;
        }

        //GELERAL RESPONSES

        private void OnPlayerListBroadcasted(List<Player> players)
        {
            this.players = players;

            Dispatcher.Invoke(() =>
            {
                if (players.Count > 0)
                {
                    Player1Label.Content = players[0].Login + " - " + players[0].Symbol.ToString();
                    Player2Label.Content = string.Empty;
                }
                if (players.Count > 1)
                {
                    Player2Label.Content = players[1].Login + " - " + players[1].Symbol.ToString();
                }
            });
        }

        private void OnMoveBroadcasted(int row, int col, Symbol symbol)
        {
            string? loginX = players.FirstOrDefault(p => p.Symbol == Symbol.X)?.Login;
            string? loginO = players.FirstOrDefault(p => p.Symbol == Symbol.O)?.Login;

            Dispatcher.Invoke(() =>
            {
                var moveKey = new Tuple<int, int>(row, col);

                if (symbol == Symbol.X)
                {
                    DrawX(row, col);
                    ChangeLogs("Player " + loginX + " moved on " + row + col);
                    moves.Add(moveKey, symbol);
                }
                else if (symbol == Symbol.O)
                {
                    DrawO(row, col);
                    ChangeLogs("Player " + loginO + " moved on " + row + col);
                    moves.Add(moveKey, symbol);
                }
            });
        }

        private void OnTurnBroadcasted(Symbol symbol)
        {
            string? turnsLogin = players.FirstOrDefault(p => p.Symbol == symbol)?.Login;

            Dispatcher.Invoke(() =>
            {
                TurnLabel.Content = "Turn: " + turnsLogin;
            });

        }

        private void OnVictoryBroadcasted(Symbol symbol)
        {
            string? winnersLogin = players.FirstOrDefault(p => p.Symbol == symbol)?.Login;

            Dispatcher.Invoke(() =>
            {
                EndGame("Player " + winnersLogin + " has won!");
            });
        }

        private void OnDrawBroadcasted()
        {
            Dispatcher.Invoke(() =>
            {
                EndGame("Draw!");
            });
        }

        private void OnGameResetBroadcasted()
        {
            Dispatcher.Invoke(() =>
            {
                Logs.Items.Clear();
                moves.Clear();
                DrawGrid();
            });
        }

        private void OnGameStartedBroadcasted()
        {
            Dispatcher.Invoke(() =>
            {
                StartGame();
            });
        }

        private void OnGameBlockedBroadcasted()
        {
            Dispatcher.Invoke(() =>
            {
                BlockGame();
            });
        }

        //CALLER RESPONSES

        private async void OnMoveConfirmed(bool moveConfirmed)
        {
            if (moveConfirmed)
            {
                await ServerClient.ServerClient.UpdateTurn();
                await ServerClient.ServerClient.CheckWin();
            }
            else
            {
                MessageBox.Show("You can't move!");
            }
        }

        private async void OnVictoryChecked(Symbol victorysSymbol)
        {
            if (victorysSymbol != Symbol.None)
            {
                await ServerClient.ServerClient.ResetGame();
            }
            else
            {
                await ServerClient.ServerClient.CheckDraw();
            }
        }

        private async void OnDrawChecked(bool isDraw)
        {
            if (isDraw)
            {
                await ServerClient.ServerClient.ResetGame();
            }
        }

        private async void EndGame(string endMessage)
        {
            BlockGame();

            ChangeLogs(endMessage);
            MessageBox.Show(this, endMessage);

            MessageBoxResult messageBoxResult = MessageBox.Show(this, "Wanna play one more time?",
                "New game", MessageBoxButton.YesNo, MessageBoxImage.Question);

            bool wantsToContinue = messageBoxResult == MessageBoxResult.Yes ? true : false;

            await ServerClient.ServerClient.PlayerWantsToContinue(wantsToContinue);

            if (!wantsToContinue)
            {
                Close();
            }
        }

        private async void GetPlayers()
        {
            await ServerClient.ServerClient.GetPlayers();
        }

        private void ChangeLogs(string newRecord)
        {
            Logs.Items.Add(newRecord);
        }

        private void StartGame()
        {
            GameCanvas.IsEnabled = true;
        }

        private void BlockGame()
        {
            GameCanvas.IsEnabled = false;
            TurnLabel.Content = "Waiting for players";
        }

        private async void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickedPoint = e.GetPosition(GameCanvas);

            int row = (int)(clickedPoint.Y / cellSize);
            int col = (int)(clickedPoint.X / cellSize);

            row = Math.Max(0, Math.Min(2, row));
            col = Math.Max(0, Math.Min(2, col));

            await ServerClient.ServerClient.MakeMove(row, col, currentPlayer.Symbol);
        }

        private void CreateLine(double x1, double y1, double x2, double y2, Brush stroke, double thickness, double opacity = 1)
        {
            Line line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = stroke,
                StrokeThickness = thickness,
                Opacity = opacity
            };
            GameCanvas.Children.Add(line);
        }

        private void DrawGrid()
        {
            GameCanvas.Children.Clear();
            double padding = 10;
            double canvasSize = Math.Max(Math.Min(GameCanvas.ActualWidth, GameCanvas.ActualHeight), 300);

            double fieldSize = Math.Max(canvasSize - 2 * padding, 0);
            cellSize = fieldSize / 3;

            double shadowOffset = 5;

            for (int i = 1; i < 3; i++)
            {
                double x = i * cellSize + padding;
                double y = i * cellSize + padding;

                CreateLine(x + shadowOffset, padding + shadowOffset, x + shadowOffset, fieldSize + padding + shadowOffset, Brushes.Gray, 10, 0.5);
                CreateLine(x, padding, x, fieldSize + padding, Brushes.Black, 17);
                CreateLine(x, padding, x, fieldSize + padding, Brushes.White, 10);

                CreateLine(padding + shadowOffset, y + shadowOffset, fieldSize + padding + shadowOffset, y + shadowOffset, Brushes.Gray, 10, 0.5);
                CreateLine(padding, y, fieldSize + padding, y, Brushes.Black, 17);
                CreateLine(padding, y, fieldSize + padding, y, Brushes.White, 10);
            };
        }

        private void DrawX(int row, int col)
        {
            double margin = cellSize * 0.1;
            double shadowOffset = 2;

            double startX = col * cellSize + margin;
            double startY = row * cellSize + margin;
            double endX = (col + 1) * cellSize - margin;
            double endY = (row + 1) * cellSize - margin;

            CreateLine(startX + shadowOffset, startY + shadowOffset, endX + shadowOffset, endY + shadowOffset, Brushes.Gray, 5);
            CreateLine(startX + shadowOffset, endY + shadowOffset, endX + shadowOffset, startY + shadowOffset, Brushes.Gray, 5);

            CreateLine(startX - 1, startY - 1, endX - 1, endY - 1, Brushes.Black, 7);
            CreateLine(startX - 1, endY - 1, endX - 1, startY - 1, Brushes.Black, 7);

            CreateLine(startX, startY, endX, endY, Brushes.White, 5);
            CreateLine(startX, endY, endX, startY, Brushes.White, 5);
        }

        private void DrawO(int row, int col)
        {
            double margin = cellSize * 0.1;
            double shadowOffset = 2;
            double offset = (cellSize - (cellSize - 1 * margin)) / 2;

            Ellipse shadowEllipse = new Ellipse
            {
                Width = cellSize - 2 * margin,
                Height = cellSize - 2 * margin,
                Stroke = Brushes.Gray,
                StrokeThickness = 5,
                Opacity = 0.5
            };

            Canvas.SetLeft(shadowEllipse, col * cellSize + margin + shadowOffset + offset);
            Canvas.SetTop(shadowEllipse, row * cellSize + margin + shadowOffset + offset);
            GameCanvas.Children.Add(shadowEllipse);

            Ellipse borderEllipse = new Ellipse
            {
                Width = cellSize - 2 * margin + 4,
                Height = cellSize - 2 * margin + 4,
                Stroke = Brushes.Black,
                StrokeThickness = 7
            };

            Canvas.SetLeft(borderEllipse, col * cellSize + margin - 2 + offset);
            Canvas.SetTop(borderEllipse, row * cellSize + margin - 2 + offset);
            GameCanvas.Children.Add(borderEllipse);

            Ellipse ellipse = new Ellipse
            {
                Width = cellSize - 2 * margin,
                Height = cellSize - 2 * margin,
                Stroke = Brushes.White,
                StrokeThickness = 5
            };

            Canvas.SetLeft(ellipse, col * cellSize + margin + offset);
            Canvas.SetTop(ellipse, row * cellSize + margin + offset);
            GameCanvas.Children.Add(ellipse);
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawGrid();

            foreach (var move in moves)
            {
                int row = move.Key.Item1;
                int col = move.Key.Item2;
                Symbol symbol = move.Value;

                if (symbol == Symbol.X)
                {
                    DrawX(row, col);
                }
                else if (symbol == Symbol.O)
                {
                    DrawO(row, col);
                }
            }
        }
    }
}
