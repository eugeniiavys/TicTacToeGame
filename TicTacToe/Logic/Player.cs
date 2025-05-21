using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TicTacToe.Logic
{
    // This class used to represent a player in the game
    public class Player
    {
        public string Name { get; private set; }
        public char Symbol { get; private set; }

        #region Display
        public Brush Color { get; private set; }
        public BitmapImage CubeImage { get; set; }
        #endregion

        public Player(string name, Brush color, char symbol)
        {
            Name = name;
            Color = color;
            Symbol = symbol;

            CubeImage = new BitmapImage(new Uri($"Assets/cube_{symbol}.png", UriKind.Relative));
        }
    }
}
