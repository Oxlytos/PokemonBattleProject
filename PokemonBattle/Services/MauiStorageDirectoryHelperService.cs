using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonBattle.Interfaces;

namespace PokemonBattle.Services
{
    public class MauiStorageDirectoryHelperService : IMauiStorageDirectoryHelper
    {
        public string GetDirectory()
        {
            //ge bara referens till vart
            return FileSystem.AppDataDirectory;
        }
    }
}
