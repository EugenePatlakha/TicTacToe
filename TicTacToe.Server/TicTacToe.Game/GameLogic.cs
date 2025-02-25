using TicTacToe.Game.Enums;

namespace TicTacToe.Game
{
    public class GameLogic
    {
        private Symbol[,] board;
        private int moves;
        public bool IsXTurn { get; private set; }

        public GameLogic()
        {
            board = new Symbol[3, 3];
            ResetGame();
        }

        public void ResetGame()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i, j] = Symbol.None;
                }
            }
            IsXTurn = true;
            moves = 0;
        }

        public bool MakeMove(int row, int col, Symbol symbol)
        {
            if (IsXTurn && symbol == Symbol.X || !IsXTurn && symbol == Symbol.O)
            {
                if (board[row, col] == Symbol.None)
                {
                    board[row, col] = symbol;
                    moves++;
                    IsXTurn = !IsXTurn;
                    return true;
                }
            }

            return false;
        }

        public Symbol CheckWin()
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2] && board[i, 0] != Symbol.None)
                    return board[i, 0];

                if (board[0, i] == board[1, i] && board[1, i] == board[2, i] && board[0, i] != Symbol.None)
                    return board[0, i];
            }

            if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] && board[0, 0] != Symbol.None)
                return board[0, 0];

            if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0] && board[0, 2] != Symbol.None)
                return board[0, 2];

            return Symbol.None;
        }

        public bool CheckDraw()
        {
            return moves == 9;
        }

        public Symbol CheckTurn()
        {
            return IsXTurn ? Symbol.X : Symbol.O;
        }
    }
}
