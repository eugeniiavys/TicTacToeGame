using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using TicTacToe.Components;
using TicTacToe.Logic;
namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TicTacToeGame _gameLogic;
        private static Player[] players =
        [
            new("X", Brushes.Red, 'X'),
            new("O", Brushes.Blue, 'O'),
        ];
        public MainWindow()
        {
            InitializeComponent();
            _gameLogic = new TicTacToeGame(players);
        }
        private void OnMoveClicked(object sender, RoutedEventArgs e)
        {
            MoveSelector selector = (MoveSelector)sender;
            selector.ClickedButton.Content = _gameLogic.CurrentPlayer.Symbol;
            selector.ClickedButton.Background = _gameLogic.CurrentPlayer.Color;

            PositionPoint cubeLocation = new(
                int.Parse(selector.RowPosition),
                CoordinateHelper.GetY(selector.ClickedButton.Name),
                CoordinateHelper.GetZ(selector.ClickedButton.Name));

            Image cubeImage = (Image)this.FindName($"cube_{cubeLocation}");
            cubeImage.Source = _gameLogic.CurrentPlayer.CubeImage;
            cubeImage.Visibility = Visibility.Visible;

            _gameLogic.MakeMove(cubeLocation);

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
                            MessageBox.Show($"Player {_gameLogic.Winner.Name} won the game!", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                            Application.Current.Shutdown();
                        });
                    });
                    return;
                }
                else 
                {
                    MessageBox.Show("The game ended in a draw!", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                    return;
                }
            }
        }
    }
}