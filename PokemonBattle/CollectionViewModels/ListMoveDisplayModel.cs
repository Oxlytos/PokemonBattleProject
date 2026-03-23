using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;

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
        public string? DamageClass { get; set; }

        public string? DisplayInfo => $"{Name} | {TypeName} | {Power} | {Accuracy} | {DamageClass} ";

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
        //emtpy moves are unknown, not really moves, just to spread out the UI properly, otherwise assignt he correct data
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
            if (move.Type != null)
            {
                DamageClass = move.Type.IsSpecialDamage ? "Special" : "Physical";
            }

        }
    }
}
