using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Logic
{
    public class ComputerPlayer
    {
        private readonly TicTacToeGame _game;
        private readonly char _playerSymbol;
        private readonly char _opponentSymbol;

        private const int MAX_DEPTH = 6;
        private const int WIN_SCORE = 1000;
        private const int POTENTIAL_LINE_SCORE = 10;
        private const int CENTER_SCORE = 5;
        private const int CORNER_SCORE = 3;

        private Dictionary<string, int> _transpositionTable;
        private List<List<(int, int, int)>> _winningLines;
        private Random _random;
        private readonly int _maxDepth;

        public ComputerPlayer(TicTacToeGame game, Player playerSymbol, int maxDepth = 6)
        {
            _game = game;
            _playerSymbol = playerSymbol.Symbol;
            _transpositionTable = new Dictionary<string, int>();
            _random = new Random();
            _maxDepth = maxDepth;

            foreach (Player player in game.Players)
            {
                if (player.Symbol != playerSymbol.Symbol)
                {
                    _opponentSymbol = player.Symbol;
                    break;
                }
            }

            InitializeWinningLines();
        }

        // Ініціалізуємо всі 49 можливих виграшних комбінацій
        private void InitializeWinningLines()
        {
            _winningLines = new List<List<(int, int, int)>>();

            // 1. Рядки - 9 комбінацій
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    var line = new List<(int, int, int)>();
                    for (int y = 0; y < 3; y++)
                    {
                        line.Add((x, y, z));
                    }
                    _winningLines.Add(line);
                }
            }

            // 2. Стовпці - 9 комбінацій
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    var line = new List<(int, int, int)>();
                    for (int x = 0; x < 3; x++)
                    {
                        line.Add((x, y, z));
                    }
                    _winningLines.Add(line);
                }
            }

            // 3. Вертикальні лінії - 9 комбінацій
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    var line = new List<(int, int, int)>();
                    for (int z = 0; z < 3; z++)
                    {
                        line.Add((x, y, z));
                    }
                    _winningLines.Add(line);
                }
            }

            // 4. Діагоналі в площині XY - 6 комбінацій
            for (int z = 0; z < 3; z++)
            {
                // Діагональ \
                var line1 = new List<(int, int, int)>
                {
                    (0, 0, z),
                    (1, 1, z),
                    (2, 2, z)
                };
                _winningLines.Add(line1);

                // Діагональ /
                var line2 = new List<(int, int, int)>
                {
                    (2, 0, z),
                    (1, 1, z),
                    (0, 2, z)
                };
                _winningLines.Add(line2);
            }

            // 5. Діагоналі в площині XZ - 6 комбінацій
            for (int y = 0; y < 3; y++)
            {
                // Діагональ \
                var line1 = new List<(int, int, int)>
                {
                    (0, y, 0),
                    (1, y, 1),
                    (2, y, 2)
                };
                _winningLines.Add(line1);

                // Діагональ /
                var line2 = new List<(int, int, int)>
                {
                    (0, y, 2),
                    (1, y, 1),
                    (2, y, 0)
                };
                _winningLines.Add(line2);
            }

            // 6. Діагоналі в площині YZ - 6 комбінацій
            for (int x = 0; x < 3; x++)
            {
                // Діагональ \
                var line1 = new List<(int, int, int)>
                {
                    (x, 0, 0),
                    (x, 1, 1),
                    (x, 2, 2)
                };
                _winningLines.Add(line1);

                // Діагональ /
                var line2 = new List<(int, int, int)>
                {
                    (x, 0, 2),
                    (x, 1, 1),
                    (x, 2, 0)
                };
                _winningLines.Add(line2);
            }

            // 7. Чотири діагоналі через весь куб
            var cube1 = new List<(int, int, int)>
            {
                (0, 0, 0),
                (1, 1, 1),
                (2, 2, 2)
            };
            _winningLines.Add(cube1);

            var cube2 = new List<(int, int, int)>
            {
                (0, 0, 2),
                (1, 1, 1),
                (2, 2, 0)
            };
            _winningLines.Add(cube2);

            var cube3 = new List<(int, int, int)>
            {
                (0, 2, 0),
                (1, 1, 1),
                (2, 0, 2)
            };
            _winningLines.Add(cube3);

            var cube4 = new List<(int, int, int)>
            {
                (0, 2, 2),
                (1, 1, 1),
                (2, 0, 0)
            };
            _winningLines.Add(cube4);
        }

        // Основний метод для знаходження ходу
        public PositionPoint MakeMove()
        {
            // Створюємо копію ігрової дошки
            char[,,] boardCopy = new char[3, 3, 3];

            // Отримуємо поточний стан дошки
            char[,,] gameBoard = _game.GetBoard();

            // Копіюємо дошку
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        boardCopy[x, y, z] = gameBoard[x, y, z];
                    }
                }
            }

            // Підрахунок зайнятих клітинок для визначення стадії гри
            int filledCells = CountFilledCells(boardCopy);

            // Для початкової стадії гри використовуємо швидку стратегію
            if (filledCells <= 2)
            {
                return MakeEarlyGameMove(boardCopy);
            }

            // Для решти ходів використовуємо повний мінімакс
            return FindBestMove(boardCopy, filledCells);
        }

        // Оптимізований хід для початку гри
        private PositionPoint MakeEarlyGameMove(char[,,] board)
        {
            // Спочатку перевіряємо, чи центр вільний - найкраща стратегічна позиція
            if (board[1, 1, 1] == '\0')
            {
                // 80% ймовірність вибрати центр, 20% - інші клітинки
                if (_random.Next(100) < 80)
                {
                    return new PositionPoint(1, 1, 1);
                }
            }

            // Визначаємо доступні кути
            var corners = new List<(int, int, int)>
            {
                (0, 0, 0), (0, 0, 2), (0, 2, 0), (0, 2, 2),
                (2, 0, 0), (2, 0, 2), (2, 2, 0), (2, 2, 2)
            };

            // Збираємо вільні кути
            var availableCorners = new List<(int, int, int)>();
            foreach (var corner in corners)
            {
                if (board[corner.Item1, corner.Item2, corner.Item3] == '\0')
                {
                    availableCorners.Add(corner);
                }
            }

            // Якщо є вільні кути, вибираємо випадковий
            if (availableCorners.Count > 0)
            {
                int index = _random.Next(availableCorners.Count);
                var corner = availableCorners[index];
                return new PositionPoint(corner.Item1, corner.Item2, corner.Item3);
            }

            // Якщо кути зайняті, вибираємо випадковий доступний хід
            var availableMoves = new List<(int, int, int)>();
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        if (board[x, y, z] == '\0')
                        {
                            availableMoves.Add((x, y, z));
                        }
                    }
                }
            }

            if (availableMoves.Count > 0)
            {
                int index = _random.Next(availableMoves.Count);
                var move = availableMoves[index];
                return new PositionPoint(move.Item1, move.Item2, move.Item3);
            }

            // На випадок, якщо дошка повна (не повинно статися)
            return new PositionPoint(0, 0, 0);
        }

        // Метод для підрахунку заповнених клітинок
        private int CountFilledCells(char[,,] board)
        {
            int count = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        if (board[x, y, z] != '\0')
                            count++;
                    }
                }
            }
            return count;
        }

        // Метод для знаходження найкращого ходу
        private PositionPoint FindBestMove(char[,,] board, int filledCells)
        {
            // Очищаємо кеш транспозицій перед новим пошуком
            _transpositionTable.Clear();

            // Визначаємо глибину пошуку залежно від стадії гри
            int searchDepth = DetermineSearchDepth(filledCells);

            int bestScore = int.MinValue;
            List<PositionPoint> bestMoves = new List<PositionPoint>();

            // Перебираємо всі можливі ходи у оптимізованому порядку
            foreach (var (x, y, z) in GetOrderedEmptyCells(board))
            {
                // Робимо хід
                board[x, y, z] = _playerSymbol;

                // Оцінюємо хід через мінімакс із альфа-бета відсіканням
                int score = Minimax(board, 0, false, int.MinValue, int.MaxValue, searchDepth);

                // Відміняємо хід
                board[x, y, z] = '\0';

                // Якщо знайдено кращий хід, оновлюємо список найкращих ходів
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(new PositionPoint(x, y, z));
                }
                // Якщо хід має таку ж оцінку, додаємо його до списку найкращих ходів
                else if (score == bestScore)
                {
                    bestMoves.Add(new PositionPoint(x, y, z));
                }

                // Якщо знайдено виграшний хід, можемо одразу його обрати
                if (score >= WIN_SCORE)
                {
                    return new PositionPoint(x, y, z);
                }
            }

            // Випадково обираємо один з найкращих ходів для різноманітності
            return bestMoves[_random.Next(bestMoves.Count)];
        }

        // Визначає глибину пошуку залежно від стадії гри
        private int DetermineSearchDepth(int filledCells)
        {
            int baseDepth;

            if (filledCells <= 2)
                baseDepth = 3;
            else if (filledCells <= 8)
                baseDepth = 4;
            else
                baseDepth = 5;

            return Math.Min(baseDepth, _maxDepth);
        }

        // Повертає впорядковані порожні клітинки для оптимізації перебору
        private List<(int, int, int)> GetOrderedEmptyCells(char[,,] board)
        {
            // Отримуємо стратегічно важливі позиції
            var center = (1, 1, 1);

            var corners = new List<(int, int, int)>
            {
                (0, 0, 0), (0, 0, 2), (0, 2, 0), (0, 2, 2),
                (2, 0, 0), (2, 0, 2), (2, 2, 0), (2, 2, 2)
            };

            var faceCenters = new List<(int, int, int)>
            {
                (1, 0, 1), (1, 2, 1), (0, 1, 1), (2, 1, 1), (1, 1, 0), (1, 1, 2)
            };

            var orderedCells = new List<(int, int, int)>();

            // Перевіряємо центр куба
            if (board[center.Item1, center.Item2, center.Item3] == '\0')
            {
                orderedCells.Add(center);
            }

            // Додаємо вільні кути
            foreach (var corner in corners)
            {
                if (board[corner.Item1, corner.Item2, corner.Item3] == '\0')
                {
                    orderedCells.Add(corner);
                }
            }

            // Додаємо вільні центри граней
            foreach (var faceCenter in faceCenters)
            {
                if (board[faceCenter.Item1, faceCenter.Item2, faceCenter.Item3] == '\0')
                {
                    orderedCells.Add(faceCenter);
                }
            }

            // Додаємо решту вільних клітинок
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        var cell = (x, y, z);
                        if (board[x, y, z] == '\0' &&
                            cell != center &&
                            !corners.Contains(cell) &&
                            !faceCenters.Contains(cell))
                        {
                            orderedCells.Add(cell);
                        }
                    }
                }
            }

            return orderedCells;
        }

        // Метод для перетворення дошки в хеш-ключ для таблиці транспозицій
        private string GetBoardKey(char[,,] board)
        {
            var key = new char[27];
            int index = 0;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        key[index++] = board[x, y, z] == '\0' ? '-' : board[x, y, z];
                    }
                }
            }

            return new string(key);
        }

        // Покращений алгоритм мінімакс з альфа-бета відсіканням
        private int Minimax(char[,,] board, int depth, bool isMaximizing, int alpha, int beta, int maxDepth)
        {
            // Перевіряємо, чи позиція вже є в кеші транспозицій
            string boardKey = GetBoardKey(board);
            if (_transpositionTable.TryGetValue(boardKey, out int cachedScore))
            {
                return cachedScore;
            }

            // Перевіряємо, чи є виграшна комбінація
            int winnerScore = CheckWinner(board);
            if (winnerScore != 0)
            {
                // Регулюємо оцінку залежно від глибини
                if (winnerScore > 0)
                    return WIN_SCORE - depth;  // Виграш комп'ютера, швидкий виграш кращий
                else
                    return -WIN_SCORE + depth; // Виграш опонента, повільніший програш кращий
            }

            // Досягнуто максимальну глибину або дошка повна
            if (depth == maxDepth || IsBoardFull(board))
            {
                int positionScore = EvaluatePosition(board);
                _transpositionTable[boardKey] = positionScore;
                return positionScore;
            }

            if (isMaximizing)
            {
                int bestScore = int.MinValue;

                // Перебір ходів у оптимізованому порядку
                foreach (var move in GetOrderedMoves(board, true))
                {
                    int x = move.Item1, y = move.Item2, z = move.Item3;

                    if (board[x, y, z] == '\0')
                    {
                        board[x, y, z] = _playerSymbol;
                        int score = Minimax(board, depth + 1, false, alpha, beta, maxDepth);
                        board[x, y, z] = '\0';

                        bestScore = Math.Max(bestScore, score);
                        alpha = Math.Max(alpha, bestScore);

                        // Альфа-бета відсікання
                        if (beta <= alpha)
                            break;
                    }
                }

                // Зберігаємо результат у кеші транспозицій
                _transpositionTable[boardKey] = bestScore;
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;

                // Перебір ходів у оптимізованому порядку
                foreach (var move in GetOrderedMoves(board, false))
                {
                    int x = move.Item1, y = move.Item2, z = move.Item3;

                    if (board[x, y, z] == '\0')
                    {
                        board[x, y, z] = _opponentSymbol;
                        int score = Minimax(board, depth + 1, true, alpha, beta, maxDepth);
                        board[x, y, z] = '\0';

                        bestScore = Math.Min(bestScore, score);
                        beta = Math.Min(beta, bestScore);

                        // Альфа-бета відсікання
                        if (beta <= alpha)
                            break;
                    }
                }

                // Зберігаємо результат у кеші транспозицій
                _transpositionTable[boardKey] = bestScore;
                return bestScore;
            }
        }

        // Метод для оптимізації порядку перебору ходів (для покращення альфа-бета відсікання)
        private List<(int, int, int)> GetOrderedMoves(char[,,] board, bool isMaximizing)
        {
            var potentialMoves = new List<(int, int, int, int)>(); // x, y, z, потенціал

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        if (board[x, y, z] == '\0')
                        {
                            int potential = 0;

                            // Додаємо потенціал за стратегічно важливі клітинки
                            if (x == 1 && y == 1 && z == 1)
                                potential += CENTER_SCORE * 2; // Центр куба
                            else if ((x == 0 || x == 2) && (y == 0 || y == 2) && (z == 0 || z == 2))
                                potential += CORNER_SCORE; // Кути
                            else if ((x == 1 && (y == 0 || y == 2) && (z == 0 || z == 2)) ||
                                    ((x == 0 || x == 2) && y == 1 && (z == 0 || z == 2)) ||
                                    ((x == 0 || x == 2) && (y == 0 || y == 2) && z == 1))
                                potential += 2; // Центри граней

                            // Перевіряємо потенціал виграшу для кожної лінії
                            foreach (var line in _winningLines)
                            {
                                if (line.Contains((x, y, z)))
                                {
                                    int computerCount = 0;
                                    int opponentCount = 0;
                                    int emptyCount = 0;

                                    foreach (var (lx, ly, lz) in line)
                                    {
                                        if (board[lx, ly, lz] == _playerSymbol)
                                            computerCount++;
                                        else if (board[lx, ly, lz] == _opponentSymbol)
                                            opponentCount++;
                                        else
                                            emptyCount++;
                                    }

                                    // Додаємо потенціал за майже виграшні лінії
                                    if (isMaximizing)
                                    {
                                        if (computerCount == 2 && emptyCount == 1)
                                            potential += 30; // Майже виграш комп'ютера
                                        else if (opponentCount == 2 && emptyCount == 1)
                                            potential += 25; // Блокування виграшу опонента
                                        else if (computerCount == 1 && emptyCount == 2)
                                            potential += 5;  // Розвиток лінії
                                    }
                                    else
                                    {
                                        if (opponentCount == 2 && emptyCount == 1)
                                            potential += 30; // Майже виграш опонента
                                        else if (computerCount == 2 && emptyCount == 1)
                                            potential += 25; // Блокування виграшу комп'ютера
                                        else if (opponentCount == 1 && emptyCount == 2)
                                            potential += 5;  // Розвиток лінії
                                    }
                                }
                            }

                            potentialMoves.Add((x, y, z, potential));
                        }
                    }
                }
            }

            // Сортуємо ходи за потенціалом (спадно для максимізуючого гравця, зростаюче для мінімізуючого)
            if (isMaximizing)
                potentialMoves.Sort((a, b) => b.Item4.CompareTo(a.Item4));
            else
                potentialMoves.Sort((a, b) => a.Item4.CompareTo(b.Item4));

            // Повертаємо відсортований список без оцінок
            return potentialMoves.Select(m => (m.Item1, m.Item2, m.Item3)).ToList();
        }

        // Розширений метод для оцінки поточної позиції (евристична функція)
        private int EvaluatePosition(char[,,] board)
        {
            int score = 0;

            // Оцінка по всіх можливих лініях
            foreach (var line in _winningLines)
            {
                int computerCount = 0;
                int opponentCount = 0;
                int emptyCount = 0;

                foreach (var (x, y, z) in line)
                {
                    if (board[x, y, z] == _playerSymbol)
                        computerCount++;
                    else if (board[x, y, z] == _opponentSymbol)
                        opponentCount++;
                    else
                        emptyCount++;
                }

                // Оцінка за потенціал виграшу
                if (computerCount == 3)
                    return WIN_SCORE;   // Комп'ютер виграв
                else if (opponentCount == 3)
                    return -WIN_SCORE;  // Опонент виграв
                else if (computerCount == 2 && emptyCount == 1)
                    score += POTENTIAL_LINE_SCORE;  // Потенційний виграш комп'ютера
                else if (opponentCount == 2 && emptyCount == 1)
                    score -= POTENTIAL_LINE_SCORE;  // Потенційний виграш опонента
                else if (computerCount == 1 && emptyCount == 2)
                    score += 1;         // Потенціал розвитку лінії комп'ютера
                else if (opponentCount == 1 && emptyCount == 2)
                    score -= 1;         // Потенціал розвитку лінії опонента
            }

            // Додаткова оцінка за контроль стратегічних клітинок
            // Центр куба
            if (board[1, 1, 1] == _playerSymbol)
                score += CENTER_SCORE;
            else if (board[1, 1, 1] == _opponentSymbol)
                score -= CENTER_SCORE;

            // Кути куба
            int computerCorners = 0;
            int opponentCorners = 0;
            var corners = new (int, int, int)[]
            {
                (0, 0, 0), (0, 0, 2), (0, 2, 0), (0, 2, 2),
                (2, 0, 0), (2, 0, 2), (2, 2, 0), (2, 2, 2)
            };

            foreach (var (x, y, z) in corners)
            {
                if (board[x, y, z] == _playerSymbol)
                    computerCorners++;
                else if (board[x, y, z] == _opponentSymbol)
                    opponentCorners++;
            }

            score += computerCorners * CORNER_SCORE;
            score -= opponentCorners * CORNER_SCORE;

            // Оцінка за кількість можливих ходів
            int computerMoves = CountPotentialWinningLines(board, _playerSymbol);
            int opponentMoves = CountPotentialWinningLines(board, _opponentSymbol);

            score += computerMoves - opponentMoves;

            return score;
        }

        // Метод для підрахунку потенційних ліній перемоги (мобільність)
        private int CountPotentialWinningLines(char[,,] board, char symbol)
        {
            int count = 0;
            char oppositeSymbol = symbol == _playerSymbol ? _opponentSymbol : _playerSymbol;

            foreach (var line in _winningLines)
            {
                bool isPotential = true;

                foreach (var (x, y, z) in line)
                {
                    if (board[x, y, z] == oppositeSymbol)
                    {
                        isPotential = false;
                        break;
                    }
                }

                if (isPotential)
                    count++;
            }

            return count;
        }

        // Перевіряє, чи є переможець на дошці
        private int CheckWinner(char[,,] board)
        {
            foreach (var line in _winningLines)
            {
                int x1 = line[0].Item1, y1 = line[0].Item2, z1 = line[0].Item3;
                int x2 = line[1].Item1, y2 = line[1].Item2, z2 = line[1].Item3;
                int x3 = line[2].Item1, y3 = line[2].Item2, z3 = line[2].Item3;

                if (board[x1, y1, z1] != '\0' &&
                    board[x1, y1, z1] == board[x2, y2, z2] &&
                    board[x2, y2, z2] == board[x3, y3, z3])
                {
                    if (board[x1, y1, z1] == _playerSymbol)
                        return WIN_SCORE;
                    else if (board[x1, y1, z1] == _opponentSymbol)
                        return -WIN_SCORE;
                }
            }

            return 0;
        }

        // Перевірка чи дошка заповнена
        private bool IsBoardFull(char[,,] board)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        if (board[x, y, z] == '\0')
                            return false;
                    }
                }
            }
            return true;
        }
    }
}