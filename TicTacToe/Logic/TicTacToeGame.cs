using System;
using System.Collections.Generic;
using System.Windows;

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
        public char[,,] GetBoard()
        {
            return _gameBoard;
        }
        public Player[] Players
        {
            get { return _players; }
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

            for (int z = 0; z < BoardSize; z++)
            {
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

            for (int y = 0; y < BoardSize; y++)
            {
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

            for (int x = 0; x < BoardSize; x++)
            {
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

            _winner = null;
            _winningLine = null;
        }

        public List<PositionPoint>? GetWinningLine()
        {
            if (!_winnerChecked)
            {
                CheckWinner();
            }
            return _winningLine;
        }

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