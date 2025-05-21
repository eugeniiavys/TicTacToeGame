using System;
using System.Collections.Generic;

namespace TicTacToe.Logic
{
    public class TicTacToeGame
    {
        #region Player
        private Player[] _players;
        private int _currentPlayerIndex;
        #endregion

        private static readonly int BOARD_SIZE = 3;
        private readonly char[,,] _gameBoard;

        private List<PositionPoint>? _winningLine = null;
        private Player? _winner = null;
        private bool _winnerChecked = false;

        public int BoardSize => _gameBoard.GetLength(0);

        public Player? Winner
        {
            get
            {
                if (!_winnerChecked)
                {
                    CheckWinner(); 
                }
                return _winner;
            }
        }

        public Player CurrentPlayer
        {
            get { return _players[_currentPlayerIndex]; }
        }

        public bool IsGameOver
        {
            get { return Winner != null || IsBoardFull(); }
        }

        public TicTacToeGame(params Player[] players)
        {
            _gameBoard = new char[BOARD_SIZE, BOARD_SIZE, BOARD_SIZE];
            _players = players;
            _currentPlayerIndex = 0;
        }

        public void MakeMove(PositionPoint point)
        {
            if (IsGameOver)
            {
                throw new Exception("The game is already over.");
            }

            int x = point.X;
            int y = point.Y;
            int z = point.Z;

            if (x < 0 || x >= BoardSize || y < 0 || y >= BoardSize || z < 0 || z >= BoardSize)
            {
                throw new ArgumentOutOfRangeException("The coordinates are out of bounds.");
            }
            if (_gameBoard[x, y, z] != '\0')
            {
                throw new Exception("The cell is already occupied.");
            }

            _gameBoard[x, y, z] = CurrentPlayer.Symbol;
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Length;


            _winnerChecked = false;
        }

        private void CheckWinner()
        {
            _winnerChecked = true;
            _winner = null;
            _winningLine = null;



            // 1. Перевірка рядків (9 комбінацій: 3 рядки * 3 шари)
            for (int x = 0; x < BoardSize; x++)
            {
                for (int z = 0; z < BoardSize; z++)
                {
                    if (_gameBoard[x, 0, z] != '\0' &&
                        _gameBoard[x, 0, z] == _gameBoard[x, 1, z] &&
                        _gameBoard[x, 1, z] == _gameBoard[x, 2, z])
                    {
                        _winningLine = new List<PositionPoint>
                        {
                            new PositionPoint(x, 0, z),
                            new PositionPoint(x, 1, z),
                            new PositionPoint(x, 2, z)
                        };
                        _winner = GetPlayerBySymbol(_gameBoard[x, 0, z]);
                        return;
                    }
                }
            }

            // 2. Перевірка стовпців (9 комбінацій: 3 стовпці * 3 шари)
            for (int y = 0; y < BoardSize; y++)
            {
                for (int z = 0; z < BoardSize; z++)
                {
                    if (_gameBoard[0, y, z] != '\0' &&
                        _gameBoard[0, y, z] == _gameBoard[1, y, z] &&
                        _gameBoard[1, y, z] == _gameBoard[2, y, z])
                    {
                        _winningLine = new List<PositionPoint>
                        {
                            new PositionPoint(0, y, z),
                            new PositionPoint(1, y, z),
                            new PositionPoint(2, y, z)
                        };
                        _winner = GetPlayerBySymbol(_gameBoard[0, y, z]);
                        return;
                    }
                }
            }

            // 3. Перевірка вертикальних ліній вздовж осі Z (9 комбінацій: 3*3 позиції на плоскій дошці)
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    if (_gameBoard[x, y, 0] != '\0' &&
                        _gameBoard[x, y, 0] == _gameBoard[x, y, 1] &&
                        _gameBoard[x, y, 1] == _gameBoard[x, y, 2])
                    {
                        _winningLine = new List<PositionPoint>
                        {
                            new PositionPoint(x, y, 0),
                            new PositionPoint(x, y, 1),
                            new PositionPoint(x, y, 2)
                        };
                        _winner = GetPlayerBySymbol(_gameBoard[x, y, 0]);
                        return;
                    }
                }
            }

            // 4. Діагоналі в площині XY (6 комбінацій: 2 діагоналі * 3 шари)
            for (int z = 0; z < BoardSize; z++)
            {
                // Діагональ зліва-зверху до права-знизу
                if (_gameBoard[0, 0, z] != '\0' &&
                    _gameBoard[0, 0, z] == _gameBoard[1, 1, z] &&
                    _gameBoard[1, 1, z] == _gameBoard[2, 2, z])
                {
                    _winningLine = new List<PositionPoint>
                    {
                        new PositionPoint(0, 0, z),
                        new PositionPoint(1, 1, z),
                        new PositionPoint(2, 2, z)
                    };
                    _winner = GetPlayerBySymbol(_gameBoard[0, 0, z]);
                    return;
                }

                // Діагональ зліва-знизу до права-зверху
                if (_gameBoard[2, 0, z] != '\0' &&
                    _gameBoard[2, 0, z] == _gameBoard[1, 1, z] &&
                    _gameBoard[1, 1, z] == _gameBoard[0, 2, z])
                {
                    _winningLine = new List<PositionPoint>
                    {
                        new PositionPoint(2, 0, z),
                        new PositionPoint(1, 1, z),
                        new PositionPoint(0, 2, z)
                    };
                    _winner = GetPlayerBySymbol(_gameBoard[2, 0, z]);
                    return;
                }
            }

            // 5. Діагоналі в площині XZ (6 комбінацій: 2 діагоналі * 3 шари)
            for (int y = 0; y < BoardSize; y++)
            {
                // Діагональ зліва-ззаду до права-спереду
                if (_gameBoard[0, y, 0] != '\0' &&
                    _gameBoard[0, y, 0] == _gameBoard[1, y, 1] &&
                    _gameBoard[1, y, 1] == _gameBoard[2, y, 2])
                {
                    _winningLine = new List<PositionPoint>
                    {
                        new PositionPoint(0, y, 0),
                        new PositionPoint(1, y, 1),
                        new PositionPoint(2, y, 2)
                    };
                    _winner = GetPlayerBySymbol(_gameBoard[0, y, 0]);
                    return;
                }

                // Діагональ зліва-спереду до права-ззаду
                if (_gameBoard[0, y, 2] != '\0' &&
                    _gameBoard[0, y, 2] == _gameBoard[1, y, 1] &&
                    _gameBoard[1, y, 1] == _gameBoard[2, y, 0])
                {
                    _winningLine = new List<PositionPoint>
                    {
                        new PositionPoint(0, y, 2),
                        new PositionPoint(1, y, 1),
                        new PositionPoint(2, y, 0)
                    };
                    _winner = GetPlayerBySymbol(_gameBoard[0, y, 2]);
                    return;
                }
            }

            // 6. Діагоналі в площині YZ (6 комбінацій: 2 діагоналі * 3 шари)
            for (int x = 0; x < BoardSize; x++)
            {
                // Діагональ зліва-ззаду до права-спереду
                if (_gameBoard[x, 0, 0] != '\0' &&
                    _gameBoard[x, 0, 0] == _gameBoard[x, 1, 1] &&
                    _gameBoard[x, 1, 1] == _gameBoard[x, 2, 2])
                {
                    _winningLine = new List<PositionPoint>
                    {
                        new PositionPoint(x, 0, 0),
                        new PositionPoint(x, 1, 1),
                        new PositionPoint(x, 2, 2)
                    };
                    _winner = GetPlayerBySymbol(_gameBoard[x, 0, 0]);
                    return;
                }

                // Діагональ зліва-спереду до права-ззаду
                if (_gameBoard[x, 0, 2] != '\0' &&
                    _gameBoard[x, 0, 2] == _gameBoard[x, 1, 1] &&
                    _gameBoard[x, 1, 1] == _gameBoard[x, 2, 0])
                {
                    _winningLine = new List<PositionPoint>
                    {
                        new PositionPoint(x, 0, 2),
                        new PositionPoint(x, 1, 1),
                        new PositionPoint(x, 2, 0)
                    };
                    _winner = GetPlayerBySymbol(_gameBoard[x, 0, 2]);
                    return;
                }
            }

            // 7. Чотири діагоналі через весь куб
            // Діагональ від (0,0,0) до (2,2,2)
            if (_gameBoard[0, 0, 0] != '\0' &&
                _gameBoard[0, 0, 0] == _gameBoard[1, 1, 1] &&
                _gameBoard[1, 1, 1] == _gameBoard[2, 2, 2])
            {
                _winningLine = new List<PositionPoint>
                {
                    new PositionPoint(0, 0, 0),
                    new PositionPoint(1, 1, 1),
                    new PositionPoint(2, 2, 2)
                };
                _winner = GetPlayerBySymbol(_gameBoard[0, 0, 0]);
                return;
            }

            // Діагональ від (0,0,2) до (2,2,0)
            if (_gameBoard[0, 0, 2] != '\0' &&
                _gameBoard[0, 0, 2] == _gameBoard[1, 1, 1] &&
                _gameBoard[1, 1, 1] == _gameBoard[2, 2, 0])
            {
                _winningLine = new List<PositionPoint>
                {
                    new PositionPoint(0, 0, 2),
                    new PositionPoint(1, 1, 1),
                    new PositionPoint(2, 2, 0)
                };
                _winner = GetPlayerBySymbol(_gameBoard[0, 0, 2]);
                return;
            }

            // Діагональ від (0,2,0) до (2,0,2)
            if (_gameBoard[0, 2, 0] != '\0' &&
                _gameBoard[0, 2, 0] == _gameBoard[1, 1, 1] &&
                _gameBoard[1, 1, 1] == _gameBoard[2, 0, 2])
            {
                _winningLine = new List<PositionPoint>
                {
                    new PositionPoint(0, 2, 0),
                    new PositionPoint(1, 1, 1),
                    new PositionPoint(2, 0, 2)
                };
                _winner = GetPlayerBySymbol(_gameBoard[0, 2, 0]);
                return;
            }

            // Діагональ від (0,2,2) до (2,0,0)
            if (_gameBoard[0, 2, 2] != '\0' &&
                _gameBoard[0, 2, 2] == _gameBoard[1, 1, 1] &&
                _gameBoard[1, 1, 1] == _gameBoard[2, 0, 0])
            {
                _winningLine = new List<PositionPoint>
                {
                    new PositionPoint(0, 2, 2),
                    new PositionPoint(1, 1, 1),
                    new PositionPoint(2, 0, 0)
                };
                _winner = GetPlayerBySymbol(_gameBoard[0, 2, 2]);
                return;
            }

            // Якщо не знайдено жодної переможної лінії
            _winner = null;
            _winningLine = null;
        }

        // Метод для отримання переможної лінії
        public List<PositionPoint>? GetWinningLine()
        {
            if (!_winnerChecked)
            {
                CheckWinner();
            }
            return _winningLine;
        }

        // Допоміжний метод для пошуку гравця за символом
        private Player? GetPlayerBySymbol(char symbol)
        {
            foreach (var player in _players)
            {
                if (player.Symbol == symbol)
                {
                    return player;
                }
            }

            return null;
        }

        // Перевірка, чи дошка заповнена
        private bool IsBoardFull()
        {
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    for (int z = 0; z < BoardSize; z++)
                    {
                        if (_gameBoard[x, y, z] == '\0')
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}