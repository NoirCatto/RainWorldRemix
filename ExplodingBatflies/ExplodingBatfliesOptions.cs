using BepInEx.Logging;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace ExplodingBatflies;

public class ExplodingBatfliesOptions : OptionInterface
{
    private readonly ManualLogSource Logger;
    
    public ExplodingBatfliesOptions(ExplodingBatflies modInstance, ManualLogSource loggerSource)
    {
        Logger = loggerSource;
        RandomDMG = config.Bind("RandomDMG", false);
        Damage = config.Bind("Damage", 2f, new ConfigAcceptableRange<float>(0f, float.MaxValue));
        Radius = config.Bind("Radius", 200f, new ConfigAcceptableRange<float>(0f, float.MaxValue));
        Force = config.Bind("Force", 10f, new ConfigAcceptableRange<float>(0f, float.MaxValue));
        Lifetime = config.Bind<int>("Lifetime", 6, new ConfigAcceptableRange<int>(0, int.MaxValue)); //For some reason OptionInterface crashes without ConfigAcceptableRange
        Stun = config.Bind("Stun", 200f, new ConfigAcceptableRange<float>(0f, float.MaxValue));
        MinStun = config.Bind("MinStun", 100f, new ConfigAcceptableRange<float>(0f, float.MaxValue));

        RandomColor = config.Bind("RandomColor", false);
        ExplosionColor = config.Bind("Color", Color.white);
    }

    public readonly Configurable<bool> RandomDMG;
    public readonly Configurable<float> Damage;
    public readonly Configurable<float> Radius;
    public readonly Configurable<float> Force;
    public readonly Configurable<int> Lifetime;
    public readonly Configurable<float> Stun;
    public readonly Configurable<float> MinStun;

    public readonly Configurable<bool> RandomColor;
    public readonly Configurable<Color> ExplosionColor;
    
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
            new OpLabel(10f, 520f, "Explosion Damage (default: 2)") { verticalAlignment = OpLabel.LabelVAlignment.Center},
            new OpUpdown(Damage, new Vector2(10f, 490f), 100f),
            
            new OpCheckBox(RandomDMG, new Vector2(140f, 494f)),
            new OpLabel(170f, 494f, "Random (between 0 and set value)") { verticalAlignment = OpLabel.LabelVAlignment.Top},
            
            new OpLabel(10f, 460f, "Explosion Radius (default: 200)") { verticalAlignment = OpLabel.LabelVAlignment.Center},
            new OpUpdown(Radius, new Vector2(10f, 430f), 100f),
            
            new OpLabel(10f, 400f, "Explosion Force (default: 10)") { verticalAlignment = OpLabel.LabelVAlignment.Center},
            new OpUpdown(Force, new Vector2(10f, 370f), 100f),
            
            new OpLabel(10f, 340f, "Explosion Lifetime (default: 6)") { verticalAlignment = OpLabel.LabelVAlignment.Center},
            new OpUpdown(Lifetime, new Vector2(10f, 310f), 100f),
            
            new OpLabel(10f, 280f, "Explosion Stun Strenght (default: 200)") { verticalAlignment = OpLabel.LabelVAlignment.Center},
            new OpUpdown(Stun, new Vector2(10f, 250f), 100f),
            
            new OpLabel(10f, 220f, "Minimum Stun Strength (default: 100)") { verticalAlignment = OpLabel.LabelVAlignment.Center},
            new OpUpdown(MinStun, new Vector2(10f, 190f), 100f),
            
            new OpLabel(10f, 160f, "Explosion Color") { verticalAlignment = OpLabel.LabelVAlignment.Center},
            new OpColorPicker(ExplosionColor, new Vector2(10f, 10f)),
            
            new OpCheckBox(RandomColor, new Vector2(190f, 85f)),
            new OpLabel(220f, 85f, "Random") { verticalAlignment = OpLabel.LabelVAlignment.Top},
        };
        opTab.AddItems(UIArrPlayerOptions);
    }

    public override void Update()
    {
        
    }

}