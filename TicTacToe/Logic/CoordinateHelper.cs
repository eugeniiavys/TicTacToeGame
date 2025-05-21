namespace TicTacToe.Logic
{
    public static class CoordinateHelper
    {
        public static int GetY(string position)
        {  
            if (position.EndsWith("Left"))
            {
                return 0;
            }
            else if (position.EndsWith("Right"))
            {
                return 2;
            }
            return 1;
        }

        public static int GetZ(string position)
        {
            if (position.StartsWith("Top"))
            {
                return 2;
            }
            else if (position.StartsWith("Bottom"))
            {
                return 0;
            }
            return 1;
        }
    }
}
