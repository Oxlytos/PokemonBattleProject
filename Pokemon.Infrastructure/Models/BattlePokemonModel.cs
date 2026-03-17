using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;

namespace Pokemon.Infrastructure.Models
{
    public class BattlePokemonModel : INotifyPropertyChanged
    {
        public PartyPokemonModel PartyPokemon { get; }


        private int _currentHealth;

        public int CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                if (_currentHealth != value)
                {
                    _currentHealth = value;
                    OnPropertyChanged(nameof(CurrentHealth));
                    OnPropertyChanged(nameof(DisplayHealth));
                }
            }
        }

        public bool IsFainted => CurrentHealth <= 0;

        public IReadOnlyList<MoveModel> Moves => PartyPokemon.Moves;

        public string NicknameOrName => PartyPokemon.Nickname ?? PartyPokemon.Name;

        public string DisplayHealth => $"Health: {CurrentHealth}/{PartyPokemon.Stats.Health}";

        public BattlePokemonModel(PartyPokemonModel pokemon)
        {
            PartyPokemon = pokemon;
            CurrentHealth = pokemon.Stats.Health;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void TakeDamage(int damage)
        {
           
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                //KO'D
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
