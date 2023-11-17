using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Scripts;

namespace Api.Menus
{
    public interface IMainMenu
    {
        void LoadSettings();
        void SaveSettings();
        void RemoveMenu(IMenu menu);
        IMenu CreateMenu(string title, ScriptType scriptType);
    }
}
