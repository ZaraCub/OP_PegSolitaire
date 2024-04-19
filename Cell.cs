using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace PegSolitaire
{
    public class Cell : INotifyPropertyChanged
    {
        // Enumeracija za različite stanja ćelije
        public enum CellState
        {
            Empty,      // Prazna ćelija
            Peg,       // ćelija s peg-om
            Inactive   // Neaktivna ćelija
        }

        // Privatna varijabla za boju ćelije
        private Brush _color;

        // Javna svojstva za boju ćelije koja koristi INotifyPropertyChanged za obavijest promjena
        public Brush Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        // Trenutno stanje (Empty, Peg, Inactive)
        public CellState State { get; set; }

        // Pozicija na ploči
        public Point Position { get; set; }

        // Event koji se okida kada se neko svojstvo promijeni
        public event PropertyChangedEventHandler PropertyChanged;

        // Metoda koja okida PropertyChanged event
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Privatna varijabla za horizontalno pomicanje ćelije
        private double _translateX;

        // Javna svojstva za horizontalno pomicanje ćelije koja koristi INotifyPropertyChanged za obavijest promjena
        public double TranslateX
        {
            get => _translateX;
            set
            {
                _translateX = value;
                OnPropertyChanged(nameof(TranslateX));
            }
        }

        // Privatna varijabla za vertikalno pomicanje ćelije
        private double _translateY;

        // Javna svojstva za vertikalno pomicanje ćelije koja koristi INotifyPropertyChanged za obavijest promjena
        public double TranslateY
        {
            get => _translateY;
            set
            {
                _translateY = value;
                OnPropertyChanged(nameof(TranslateY));
            }
        }
    }
}
