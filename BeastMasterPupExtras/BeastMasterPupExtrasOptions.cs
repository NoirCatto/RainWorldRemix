using BepInEx.Logging;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace BeastMasterPupExtras;

public class BeastMasterPupExtrasOptions : OptionInterface
{
    private readonly ManualLogSource Logger;

    public BeastMasterPupExtrasOptions(BeastMasterPupExtras modInstance, ManualLogSource loggerSource)
    {
        Logger = loggerSource;
        SlugNPCAttemptJump = this.config.Bind<bool>("SlugNPCAttemptJump", true);
    }

    public readonly Configurable<bool> SlugNPCAttemptJump;
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
            new OpLabel(10f, 520f, "SlugNPC will attempt to replicate a Player's jump if no other path to Player is found."),
            new OpCheckBox(SlugNPCAttemptJump, 10f,490f),
        };
        opTab.AddItems(UIArrPlayerOptions);
    }

    public override void Update()
    {
        
    }

}