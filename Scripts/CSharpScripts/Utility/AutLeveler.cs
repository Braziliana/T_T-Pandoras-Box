using Api.Game.GameInputs;
using Api.Game.Objects;
using Api.Menus;
using Api.Scripts;
using Scripts.Utils;

namespace Scripts.CSharpScripts.Utility;

public class AutLeveler : IScript
{
    public enum SpellLevelOrder
    {
        QWE,
        QEW,
        WEQ,
        WQE,
        EQW,
        EWQ,
    }
    public string Name => "AutLeveler";
    public ScriptType ScriptType => ScriptType.Utility;
    public bool Enabled { get; set; }

    public SpellLevelOrder Order { get; set; }
    
    
    private IMenu? _menu;
    private IEnumComboBox<SpellLevelOrder>? _levelSelectionComboBox;

    private readonly IMainMenu _mainMenu;
    private readonly IGameInput _gameInput;
    private readonly ILocalPlayer _localPlayer;
    private int _lastLevel = 0;
    private IValueSlider? _levelAfterSlider;
    
    public AutLeveler(IMainMenu mainMenu, IGameInput gameInput, ILocalPlayer localPlayer)
    {
        _mainMenu = mainMenu;
        _gameInput = gameInput;
        _localPlayer = localPlayer;
    }
    
    public void OnLoad()
    {
        _menu = _mainMenu.CreateMenu("AutLeveler", ScriptType.TargetSelector);
        _levelSelectionComboBox =
            _menu.AddEnumComboBox("Spells order", SpellLevelOrder.QWE);
        _levelSelectionComboBox.SelectionChanged += delegate(SpellLevelOrder order)
        {
            Order = order;
        };

        _levelAfterSlider = _menu.AddFloatSlider("Auto level after", 3, 0, 6, 1, 0);
    }

    public void OnUnload()
    {
        if (_menu != null)
        {
            _mainMenu.RemoveMenu(_menu);
        }
    }

    public void OnUpdate(float deltaTime)
    {
        if (_lastLevel < _localPlayer.Level)
        {
            _lastLevel = _localPlayer.Level;
            if (_levelAfterSlider != null && _levelAfterSlider.IntValue < _lastLevel)
            {
                _gameInput.LevelUpSpell(GetSpellSlotToLevel(_lastLevel));
            }
        }
    }

    private Dictionary<SpellLevelOrder, List<SpellSlot>>? _spellsOrders;
    private SpellSlot GetSpellSlotToLevel(int level)
    {
        if (_spellsOrders == null)
        {
            _spellsOrders = new Dictionary<SpellLevelOrder, List<SpellSlot>>();
            //QWE
            _spellsOrders.Add(SpellLevelOrder.QWE, new List<SpellSlot>
            {
                //1          //2          //3          //4          //5          //6
                SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.Q, SpellSlot.Q, SpellSlot.R,
                //7          //8          //9          //10          //11
                SpellSlot.Q, SpellSlot.W, SpellSlot.Q, SpellSlot.W, SpellSlot.R,
                //12         //13         //14         //15         //16
                SpellSlot.W, SpellSlot.W, SpellSlot.E, SpellSlot.E, SpellSlot.R, SpellSlot.E, SpellSlot.E
            });
            _spellsOrders.Add(SpellLevelOrder.QEW, new List<SpellSlot>
            {
                //1          //2          //3          //4          //5          //6
                SpellSlot.Q, SpellSlot.E, SpellSlot.W, SpellSlot.Q, SpellSlot.Q, SpellSlot.R,
                //7          //8          //9          //10         //11
                SpellSlot.Q, SpellSlot.E, SpellSlot.Q, SpellSlot.E, SpellSlot.R,
                //12         //13         //14         //15         //16
                SpellSlot.E, SpellSlot.E, SpellSlot.W, SpellSlot.W, SpellSlot.R, SpellSlot.W, SpellSlot.W
            });
            _spellsOrders.Add(SpellLevelOrder.WEQ, new List<SpellSlot>
            {
                //1    //2    //3    //4    //5    //6
                SpellSlot.W, SpellSlot.E, SpellSlot.Q, SpellSlot.W, SpellSlot.W, SpellSlot.R,
                //7    //8    //9    //10   //11
                SpellSlot.W, SpellSlot.E, SpellSlot.W, SpellSlot.E, SpellSlot.R,
                //12   //13   //14   //15   //16
                SpellSlot.E, SpellSlot.E, SpellSlot.Q, SpellSlot.Q, SpellSlot.R, SpellSlot.Q, SpellSlot.Q
            });
            _spellsOrders.Add(SpellLevelOrder.WQE, new List<SpellSlot>
            {
                //1    //2    //3    //4    //5    //6
                SpellSlot.W, SpellSlot.Q, SpellSlot.E, SpellSlot.W, SpellSlot.W, SpellSlot.R,
                //7    //8    //9    //10   //11
                SpellSlot.W, SpellSlot.Q, SpellSlot.W, SpellSlot.Q, SpellSlot.R,
                //12   //13   //14   //15   //16
                SpellSlot.Q, SpellSlot.Q, SpellSlot.E, SpellSlot.E, SpellSlot.R, SpellSlot.E, SpellSlot.E
            });
            _spellsOrders.Add(SpellLevelOrder.EQW, new List<SpellSlot>
            {
                //1    //2    //3    //4    //5    //6
                SpellSlot.E, SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.E, SpellSlot.R,
                //7    //8    //9    //10   //11
                SpellSlot.E, SpellSlot.Q, SpellSlot.E, SpellSlot.Q, SpellSlot.R,
                //12   //13   //14   //15   //16
                SpellSlot.Q, SpellSlot.Q, SpellSlot.W, SpellSlot.W, SpellSlot.R, SpellSlot.W, SpellSlot.W
            });
            _spellsOrders.Add(SpellLevelOrder.EWQ, new List<SpellSlot>
            {
                //1    //2    //3    //4    //5    //6
                SpellSlot.E, SpellSlot.W, SpellSlot.Q, SpellSlot.E, SpellSlot.E, SpellSlot.R,
                //7    //8    //9    //10   //11
                SpellSlot.E, SpellSlot.W, SpellSlot.E, SpellSlot.W, SpellSlot.R,
                //12   //13   //14   //15   //16
                SpellSlot.W, SpellSlot.W, SpellSlot.Q, SpellSlot.Q, SpellSlot.R, SpellSlot.Q, SpellSlot.Q
            });
        }

        return _spellsOrders[Order][level-1];
    }

    public void OnRender(float deltaTime)
    {
    }
}