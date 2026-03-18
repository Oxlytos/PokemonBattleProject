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
            //ge bara referens till vart MAUI har sina filer
            //Går väl och hårdkorda, fast det verkar lite onödigt
            string path = FileSystem.AppDataDirectory;
            Console.WriteLine(path);
            Console.WriteLine(FileSystem.AppDataDirectory.ToString());
            return FileSystem.AppDataDirectory;
        }
    }
}
