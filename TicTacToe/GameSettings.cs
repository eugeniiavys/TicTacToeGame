using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }
    public class GameSettings
    {
        public bool IsComputerEnabled { get; set; } = true;
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
        public bool ComputerMovesFirst { get; set; } = false;
    }
}
