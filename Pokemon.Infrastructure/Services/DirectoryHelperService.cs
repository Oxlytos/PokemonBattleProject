using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services
{
    public class DirectoryHelperServic
    {
        //Behöver vägen till
        private readonly IMauiStorageDirectoryHelper _mauiStorageDirectoryHelper;
        public DirectoryHelperServic(IMauiStorageDirectoryHelper mauiStorageDirectoryHelper)
        {
            _mauiStorageDirectoryHelper = mauiStorageDirectoryHelper;
        }
        public string GetDirectory()
        {
            //plats på pc
            //Hårdkodat, inte snyggt
            //return @"C:\\Users\\oxlyt\\AppData\\Local\\User Name\\com.companyname.pokemonbattle\\Data";

            //relativ path för enhet
            //snyggare
            string path = _mauiStorageDirectoryHelper.GetDirectory();

            return path;
        }
    }
}
