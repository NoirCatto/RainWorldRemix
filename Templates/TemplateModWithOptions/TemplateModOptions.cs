using BepInEx.Logging;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace TemplateMod;

public class TemplateModOptions : OptionInterface
{
    private readonly ManualLogSource Logger;

    public TemplateModOptions(TemplateMod modInstance, ManualLogSource loggerSource)
    {
        Logger = loggerSource;
        PlayerSpeed = this.config.Bind<float>("PlayerSpeed", 1f, new ConfigAcceptableRange<float>(0f, 100f));
    }

    public readonly Configurable<float> PlayerSpeed;
    private UIelement[] UIArrPlayerOptions;
    
    
    public override void Initialize()
    {
        var opTab = new OpTab(this, "Options");
        this.Tabs = new[]
        {
            opTab
        };

        UIArrPlayerOptions = new UIelement[]
        {
            new OpLabel(10f, 550f, "Options", true),
            new OpLabel(10f, 520f, "Player run speed factor"),
            new OpUpdown(PlayerSpeed, new Vector2(10f,490f), 100f, 1),
            
            new OpLabel(10f, 460f, "Gotta go fast!", false){ color = new Color(0.2f, 0.5f, 0.8f) }
        };
        opTab.AddItems(UIArrPlayerOptions);
    }

    public override void Update()
    {
        if (((OpUpdown)UIArrPlayerOptions[2]).GetValueFloat() > 10)
        {
            ((OpLabel)UIArrPlayerOptions[3]).Show();
        }
        else
        {
            ((OpLabel)UIArrPlayerOptions[3]).Hide();
        }
    }

}