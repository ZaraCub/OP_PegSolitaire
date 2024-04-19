using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PegSolitaire
{
    public partial class MainWindow : Window
    {
        // Listi ćelija na ploči za igru
        public List<Cell> Cells { get; set; }

        // 2D niz koji predstavlja ploču igre
        private Cell[,] BoardArray;

        // Trenutno odabrana ćelija (peg)
        private Cell SelectedCell;

        // Instanca logike igre
        private GameLogic Logic = new GameLogic();

        // Štoperica za mjerenje proteklog vremena igre
        private System.Diagnostics.Stopwatch stopwatch;

        // Indikator da li je timer pokrenut
        private bool isTimerRunning = true;

        // Timer igre
        private DispatcherTimer gameTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            DataContext = this;

            // Inicijalizacija i pokretanje štoperice
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // Postavljanje timera
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            // Dohvaćanje najboljeg rezultata iz baze podataka
            DatabaseHelper dbHelper = new DatabaseHelper();
            string highScore = GetHighestScoreFromDatabase();
            highScoreLabel.Content = $"High Score: {highScore}";
        }

        // Ažuriranje prikaza vremena svake sekunde
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            timeLabel.Content = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
        }

        // Pauziranje igre
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            // Ako je igra trenutno aktivna, pauzira se
            if (isTimerRunning)
            {
                // Pauziranje štoperice i timera
                stopwatch.Stop();
                isTimerRunning = false;
                gameTimer.Stop();

                // Poruka s opcijom nastavka igre
                MessageBoxResult result = MessageBox.Show("Game paused. Do you want to continue?", "Pause", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Nastavak štoperice i timera
                    stopwatch.Start();
                    gameTimer.Start();
                    isTimerRunning = true;
                }
                else
                {
                    continueButton.Visibility = Visibility.Visible;
                }
            }
        }

        // Nastavak igre
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isTimerRunning)
            {
                // Nastavak štoperice i timera
                stopwatch.Start();
                gameTimer.Start();
                isTimerRunning = true;
                continueButton.Visibility = Visibility.Collapsed;
            }
        }


        // Inicijalizacija ploče za igru
        private void InitializeBoard()
        {
            Cells = new List<Cell>();
            BoardArray = new Cell[7, 7];

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Cell cell;

                    if ((i < 2 && j < 2) || (i < 2 && j > 4) || (i > 4 && j < 2) || (i > 4 && j > 4))
                        cell = new Cell { Color = Brushes.Gray, State = Cell.CellState.Inactive, Position = new Point(j, i) };
                    else if (i == 3 && j == 3)
                        cell = new Cell { Color = Brushes.White, State = Cell.CellState.Empty, Position = new Point(j, i) };
                    else
                        cell = new Cell { Color = Brushes.Blue, State = Cell.CellState.Peg, Position = new Point(j, i) };

                    Cells.Add(cell);
                    BoardArray[i, j] = cell;
                }
            }
            BoardItemsControl.ItemsSource = null;
            BoardItemsControl.ItemsSource = Cells;
        }

        // Event handler za klik na peg
        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            Cell clickedCell = ellipse.DataContext as Cell;

            if (SelectedCell == null)
            {
                if (clickedCell.State == Cell.CellState.Peg)
                {
                    SelectedCell = clickedCell;
                    SelectedCell.Color = Brushes.Red; // Highlight the selected peg.
                }
            }
            else
            {
                if (SelectedCell == clickedCell) // Deselecting the peg
                {
                    SelectedCell.Color = Brushes.Blue;
                    SelectedCell = null;
                }
                else if (Logic.IsValidMove(SelectedCell, clickedCell, BoardArray))
                {
                    Logic.MakeMove(SelectedCell, clickedCell, BoardArray);
                    SelectedCell = null;
                    CheckForEndGame();
                }
                else
                {
                    SelectedCell.Color = Brushes.Blue; // Reset the color of the previously selected peg.
                    SelectedCell = null;
                }
            }
        }

        private int CountRemainingPegs()
        {
            int count = 0;
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    if (BoardArray[y, x].State == Cell.CellState.Peg)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void CheckForEndGame()
        {
            if (!Logic.AreMovesPossible(BoardArray))
            {
                stopwatch.Stop(); // Stop timer
                int remainingPegs = CountRemainingPegs();

                if (remainingPegs == 1 && BoardArray[3, 3].State == Cell.CellState.Peg)
                {
                    MessageBox.Show("Congratulations! You won!", "End of Game", MessageBoxButton.OK, MessageBoxImage.Asterisk); 
                    var timeTaken = stopwatch.Elapsed.TotalSeconds; 
                    DatabaseHelper dbHelper = new DatabaseHelper();
                    dbHelper.AddScore(timeTaken.ToString()); // Save the score only if the user wins
                }
                else
                {
                    MessageBox.Show($"Game Over! You have {remainingPegs} pegs remaining.", "End of Game", MessageBoxButton.OK, MessageBoxImage.Exclamation); 
                }

                var result = MessageBox.Show("Would you like to restart?", "Restart Game", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    InitializeBoard();
                    stopwatch.Restart(); // Restart the stopwatch for the new game
                    isTimerRunning = true;
                    continueButton.Visibility = Visibility.Collapsed;

                    // Refresh the high score
                    string highScore = GetHighestScoreFromDatabase();
                    highScoreLabel.Content = $"High Score: {highScore}";
                }
            }
        }

        private string GetHighestScoreFromDatabase()
        {
            using (var dbHelper = new DatabaseHelper())
            {
                var scoresReader = dbHelper.GetScores();
                double? bestScore = null;

                while (scoresReader.Read())
                {
                    double currentScore;
                    if (double.TryParse(scoresReader["TimeTaken"].ToString(), out currentScore))
                    {
                        if (!bestScore.HasValue || currentScore < bestScore.Value)
                        {
                            bestScore = currentScore;
                        }
                    }
                }
                return bestScore.HasValue ? FormatTime((int)bestScore.Value) : "N/A";
            }
        }


        private string FormatTime(int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

    }
}

