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
        public event Action? MenuOpen;
        public event Action? MenuClose;
        void Render();
        public ISubMenu AddSubMenu(string title);
        void LoadSettings();
        void SaveSettings();
        void RemoveMenu(IMenu menu);
        IMenu CreateMenu(string title, ScriptType scriptType);
    }
}
