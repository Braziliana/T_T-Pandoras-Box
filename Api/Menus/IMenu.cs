using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Inputs;
using Api.Scripts;
using Api.Settings;

namespace Api.Menus
{
    public interface IMenu : ISubMenu
    {
        ScriptType ScriptType { get; }
    }
}
