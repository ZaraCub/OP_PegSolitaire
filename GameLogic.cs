using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PegSolitaire
{
    public class GameLogic
    {
        // Provjerava je li potez s 'from' na 'to' valjan prema pravilima Peg Solitaire igre.
        public bool IsValidMove(Cell from, Cell to, Cell[,] board)
        {
            // Ako početna ćelija nema peg ili ciljna ćelija nije prazna, potez nije valjan
            if (from.State != Cell.CellState.Peg || to.State != Cell.CellState.Empty)
                return false;

            // Izračunava razliku u X i Y koordinatama između početne i ciljne ćelije
            int dx = (int)(to.Position.X - from.Position.X);
            int dy = (int)(to.Position.Y - from.Position.Y);

            // Provjerava je li pomicanje dvije ćelije u vodoravnom ili okomitom smjeru
            if ((Math.Abs(dx) == 2 && dy == 0) || (dx == 0 && Math.Abs(dy) == 2))
            {
                // Pronalazi ćeliju između početne i ciljne ćelije
                Cell midCell = board[(int)(from.Position.Y + dy / 2), (int)(from.Position.X + dx / 2)];
                // Potez je valjan ako ćelija između ima peg
                return midCell.State == Cell.CellState.Peg;
            }

            return false;
        }

        // Izvršava potez od 'from' do 'to' i ažurira stanje ploče
        public void MakeMove(Cell from, Cell to, Cell[,] board)
        {
            // Izračunava razliku u X i Y koordinatama između početne i ciljne ćelije
            int dx = (int)(to.Position.X - from.Position.X);
            int dy = (int)(to.Position.Y - from.Position.Y);
            // Pronalazi ćeliju između početne i ciljne ćelije
            Cell midCell = board[(int)(from.Position.Y + dy / 2), (int)(from.Position.X + dx / 2)];

            // Ažurira stanje ćelija
            from.State = Cell.CellState.Empty;
            midCell.State = Cell.CellState.Empty;
            to.State = Cell.CellState.Peg;

            // Ažurira boje ćelija
            from.Color = Brushes.White;
            midCell.Color = Brushes.White;
            to.Color = Brushes.Blue;
        }

        // Provjerava postoji li moguć potez na ploči
        public bool AreMovesPossible(Cell[,] board)
        {
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    Cell currentCell = board[y, x];

                    // Ako trenutna ćelija ima peg, provjerava se svaki od mogućih smjerova skakanja: gore, dolje, lijevo, desno
                    if (currentCell.State == Cell.CellState.Peg)
                    {
                        foreach (var direction in new[] { (0, -2), (0, 2), (-2, 0), (2, 0) })
                        {
                            int newY = y + direction.Item2;
                            int newX = x + direction.Item1;

                            // Ako su nove koordinate unutar granica ploče
                            if (newY >= 0 && newY < 7 && newX >= 0 && newX < 7)
                            {
                                Cell targetCell = board[newY, newX];

                                // Ako je moguć valjan potez, igra nije gotova
                                if (IsValidMove(currentCell, targetCell, board))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            // Ako je petlja završila bez povratne vrijednosti, nema preostalih poteza
            return false;
        }
    }
}
