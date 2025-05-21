using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Threading.Tasks;
using TicTacToe.Components;
using TicTacToe.Logic;
using System.Globalization;
using System.Threading;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        private TicTacToeGame _gameLogic;
        private ComputerPlayer _computerPlayer;
        private bool _isComputerEnabled;
        private DifficultyLevel _difficulty;
        private bool _computerMovesFirst;

        private static Player[] players =
        [
            new("X", Brushes.Red, 'X'),
            new("O", Brushes.Blue, 'O'),
        ];

        public MainWindow()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            MenuGrid.Visibility = Visibility.Visible;
            GameGrid.Visibility = Visibility.Collapsed;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Зчитуємо налаштування з меню
            _isComputerEnabled = rbComputer.IsChecked == true;
            _difficulty = cbDifficulty.SelectedItem switch
            {
                ComboBoxItem item when item.Content.ToString() == "Easy" => DifficultyLevel.Easy,
                ComboBoxItem item when item.Content.ToString() == "Medium" => DifficultyLevel.Medium,
                ComboBoxItem item when item.Content.ToString() == "Hard" => DifficultyLevel.Hard,
                _ => DifficultyLevel.Medium
            };
            _computerMovesFirst = cbFirstMove.SelectedItem switch
            {
                ComboBoxItem item when item.Content.ToString() == "Computer/Player 2 (O)" => true,
                _ => false
            };
            InitializeGame();

            MenuGrid.Visibility = Visibility.Collapsed;
            GameGrid.Visibility = Visibility.Visible;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void InitializeGame()
        {
            if (_computerMovesFirst)
            {
                players = new Player[]
                {
                    new("O", Brushes.Blue, 'O'),
                    new("X", Brushes.Red, 'X')
                };
            }
            else
            {
                players = new Player[]
                {
                    new("X", Brushes.Red, 'X'),
                    new("O", Brushes.Blue, 'O')
                };
            }

            _gameLogic = new TicTacToeGame(players);

            if (_isComputerEnabled)
            {
                SetupComputerPlayer();
            }
        }

        private void SetupComputerPlayer()
        {
            int maxDepth = _difficulty switch
            {
                DifficultyLevel.Easy => 2,
                DifficultyLevel.Medium => 4,
                DifficultyLevel.Hard => 6,
                _ => 4
            };

            Player computerPlayer = _computerMovesFirst ? players[0] : players[1];
            _computerPlayer = new ComputerPlayer(_gameLogic, computerPlayer, maxDepth);

            if (_gameLogic.CurrentPlayer.Symbol == computerPlayer.Symbol)
            {
                MakeComputerMove();
            }
        }

        private void MakeComputerMove()
        {
            Task.Delay(500).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    PositionPoint move = _computerPlayer.MakeMove();
                    Image cubeImage = (Image)this.FindName($"cube_{move}");
                    cubeImage.Source = _gameLogic.CurrentPlayer.CubeImage;
                    cubeImage.Visibility = Visibility.Visible;

                    Grid leftPanel = (Grid)GameGrid.Children[0];
                    MoveSelector targetSelector = null;

                    foreach (UIElement child in leftPanel.Children)
                    {
                        if (child is MoveSelector selector && selector.RowPosition == move.X.ToString())
                        {
                            targetSelector = selector;
                            break;
                        }
                    }

                    if (targetSelector != null)
                    {
                        Button targetButton = null;
                        var grid = (Grid)targetSelector.Content;
                        var buttonGrid = (Grid)grid.Children[0];

                        foreach (var child in buttonGrid.Children)
                        {
                            if (child is Button button)
                            {
                                int y = CoordinateHelper.GetY(button.Name);
                                int z = CoordinateHelper.GetZ(button.Name);

                                if (y == move.Y && z == move.Z)
                                {
                                    targetButton = button;
                                    break;
                                }
                            }
                        }

                        if (targetButton != null)
                        {
                            targetButton.Content = _gameLogic.CurrentPlayer.Symbol;
                            targetButton.Background = _gameLogic.CurrentPlayer.Color;
                            targetButton.IsEnabled = false;
                            targetSelector.ClickedButton = targetButton;
                        }
                    }

                    _gameLogic.MakeMove(move);
                    CheckGameOver();
                });
            });
        }

        private void OnMoveClicked(object sender, RoutedEventArgs e)
        {
            if (_gameLogic.IsGameOver)
                return;

            MoveSelector selector = (MoveSelector)sender;
            selector.ClickedButton.Content = _gameLogic.CurrentPlayer.Symbol;
            selector.ClickedButton.Background = _gameLogic.CurrentPlayer.Color;

            PositionPoint cubeLocation = new(
                int.Parse(selector.RowPosition),
                CoordinateHelper.GetY(selector.ClickedButton.Name),
                CoordinateHelper.GetZ(selector.ClickedButton.Name));

            char[,,] board = _gameLogic.GetBoard();
            if (board[cubeLocation.X, cubeLocation.Y, cubeLocation.Z] != '\0')
            {
                MessageBox.Show("This cell is already taken. Please select another one.", "Wrong move", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Image cubeImage = (Image)this.FindName($"cube_{cubeLocation}");
            cubeImage.Source = _gameLogic.CurrentPlayer.CubeImage;
            cubeImage.Visibility = Visibility.Visible;

            _gameLogic.MakeMove(cubeLocation);

            if (!CheckGameOver() && _isComputerEnabled)
            {
                MakeComputerMove();
            }
        }

        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            RestartGame();
        }

        private void RestartGame()
        {
            _gameLogic = new TicTacToeGame(players);

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        Image cubeImage = (Image)this.FindName($"cube_{x}_{y}_{z}");
                        if (cubeImage != null)
                        {
                            cubeImage.Source = null;
                            cubeImage.Visibility = Visibility.Hidden;
                            cubeImage.Effect = null;
                        }
                    }
                }
            }

            grid1.Visibility = Visibility.Visible;
            grid2.Visibility = Visibility.Visible;
            grid3.Visibility = Visibility.Visible;

            Grid leftPanel = (Grid)GameGrid.Children[0];
            leftPanel.Visibility = Visibility.Visible;

            foreach (UIElement element in leftPanel.Children)
            {
                if (element is MoveSelector selector)
                {
                    selector.Visibility = Visibility.Visible;
                    selector.ResetButtons();
                }
            }

            if (_isComputerEnabled)
            {
                SetupComputerPlayer();
            }
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Return to main menu? Current game progress will be lost.",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Повертаємося до меню
                MenuGrid.Visibility = Visibility.Visible;
                GameGrid.Visibility = Visibility.Collapsed;
                // Скидаємо налаштування
                rbComputer.IsChecked = true;
                cbDifficulty.SelectedIndex = 0;
                cbFirstMove.SelectedIndex = 0;
            }
        }

        private bool CheckGameOver()
        {
            if (_gameLogic.IsGameOver)
            {
                if (_gameLogic.Winner != null)
                {
                    List<PositionPoint> winningLine = _gameLogic.GetWinningLine();
                    if (winningLine != null)
                    {
                        foreach (PositionPoint point in winningLine)
                        {
                            Image image = (Image)this.FindName($"cube_{point}");
                            DropShadowEffect glowEffect = new DropShadowEffect
                            {
                                Color = ((SolidColorBrush)_gameLogic.Winner.Color).Color,
                                Direction = 0,
                                ShadowDepth = 0,
                                BlurRadius = 15,
                                Opacity = 1
                            };
                            image.Effect = glowEffect;
                        }
                    }

                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Player {_gameLogic.Winner.Name} won!",
                                "Game over", MessageBoxButton.OK, MessageBoxImage.Information);

                            if (MessageBox.Show("Do you want to play again?", "New game",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                RestartGame();
                            }
                            else
                            {
                                Application.Current.Shutdown();
                            }
                        });
                    });
                }
                else
                {
                    MessageBox.Show("The game ended in a draw!", "Game over",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    if (MessageBox.Show("Do you want to play again?", "New game",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        RestartGame();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
                return true;
            }
            return false;
        }

      
    }
}