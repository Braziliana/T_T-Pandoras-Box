using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Inputs;
using Api.Settings;

namespace Api.Menus
{
    public interface IMenuElement
    {
        string SaveId { get; }
        string Name { get; }
        string Description { get; }
        void Render();
        void LoadSettings(ISettingsProvider settingsProvider);
        void SaveSettings(ISettingsProvider settingsProvider);
    }
}
