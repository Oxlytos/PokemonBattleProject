using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace PokemonBattle.ListModel
{
    public class ListMoveDisplayModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(Name);
            }
        }
        public string? Description { get; set; }

        public float? Power {  get; set; }

        public float? Accuracy { get; set; }
        public int? PP { get; set; }

        public string? TypeName { get; set; }
        public string? TypeIconPath { get; set; }

        public string? DisplayInfo => $"{Name} | {TypeName} | {Power} | {Accuracy} | {PP} ";

        private ImageSource _typeIconSource;
        public ImageSource TypeIconSource
        {
            get { return _typeIconSource; }
            set
            {
                _typeIconSource = value;
                OnPropertyChanged(nameof(TypeIconSource));
                
            }
        }
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ListMoveDisplayModel(MoveModel move)
        {
            if(move.Name=="-")
            {
                Name = "-";
                Power = 0;
                Accuracy = 0;
                PP =  0;
                TypeName = "Unknown";
            }
            Name = move.Name;
            Power = move.Power;
            Accuracy = move.Accuracy;
            PP = move.Pp ?? 0;

        }
    }
}
