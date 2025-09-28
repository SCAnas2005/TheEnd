
using System;
using Microsoft.Xna.Framework.Input;

public class ActionInteraction
{
    public string Name;
    public string Description;

    public Keys Key;
    private Func<Player, bool> _originalConditionToShow;
    public Func<Player, bool> ConditionToShow;
    private Func<Player, bool> _originalConditionToAction;
    public Func<Player, bool> ConditionToAction;
    public Action<Player> Action;

    public bool Show;

    public ActionInteraction(string name, string description, Keys key, Func<Player, bool> conditionToShow, Func<Player, bool> conditionToAction, Action<Player> action)
    {
        Name = name;
        Description = $"[{key}] {Name}";
        Key = key;
        _originalConditionToShow = conditionToShow;
        ConditionToShow = (player) =>
        {
            bool val = _originalConditionToShow(player);
            Show = val;
            return val;
        };
        _originalConditionToAction = conditionToAction;
        ConditionToAction = (player) => { return InputManager.IsPressed(key) && _originalConditionToAction(player); };
        Action = action;
    }

    public void Edit(string name = null, string description = null, Keys? key = null,
        Func<Player, bool> conditionToShow = null, Func<Player, bool> conditionToAction = null, Action<Player> action = null
    )
    {
        if (description != null) Description = description;
        if (name!=null) Name = name; Description = $"[{Key}] {Name}";
        if (key != null) Key = key.Value;
        if (conditionToShow != null) {
            _originalConditionToShow = conditionToShow;
            ConditionToShow = (player) =>
            {
                bool val = _originalConditionToShow(player);
                Show = val;
                return val;
            };
        }
        if (conditionToAction != null)
        {
            _originalConditionToAction = conditionToAction;
            ConditionToAction = (player) => { return InputManager.IsPressed(Key) && _originalConditionToAction(player); };
        }
        if (action != null) Action = action;
    }
}