using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using RWCustom;
using UnityEngine;

namespace Bandanas;

public class BandanasOptions : OptionInterface
{
    private readonly ManualLogSource Logger;

    public int MaxPlayers;

    public BandanasOptions(Bandanas modInstance, ManualLogSource loggerSource)
    {
        Logger = loggerSource;
    }

    public readonly List<Configurable<bool>> UseHeadRagList = [];
    public readonly List<Configurable<Color>> HeadRagColorList = [];
    public readonly List<Configurable<bool>> HeadRagColorRainbowList = [];
    public readonly List<Configurable<int>> HeadRagLengthList = [];

    public bool UseHeadRag(int playerNum) => UseHeadRagList[playerNum].Value;
    public Color HeadRagColor(int playerNum) => HeadRagColorList[playerNum].Value;
    public bool HeadRagRainbow(int playerNum) => HeadRagColorRainbowList[playerNum].Value;
    public int HeadRagLength(int playerNum) => HeadRagLengthList[playerNum].Value;


    internal void InternalInit()
    {
        MaxPlayers = Custom.rainWorld.options.controls.Length;

        for (var i = 0; i <= MaxPlayers; i++)
        {
            var hsl = Custom.RGB2HSL(new Color(0.8f, 0.05f, 0.04f));
            hsl.x += i * 0.1f;
            if (hsl.x > 1f) hsl.x -= 1f;
            var color = Custom.HSL2RGB(hsl.x, hsl.y, hsl.z);

            UseHeadRagList.Add(config.Bind<bool>(nameof(UseHeadRagList) + i, false));
            HeadRagColorList.Add(config.Bind<Color>(nameof(HeadRagColorList) + i, color));
            HeadRagColorRainbowList.Add(config.Bind<bool>(nameof(HeadRagColorRainbowList) + i, false));
            HeadRagLengthList.Add(config.Bind<int>(nameof(HeadRagLengthList) + i, 5, new ConfigAcceptableRange<int>(3, 15)));
        }
    }

    public override void Initialize()
    {
        var fullTabs = Mathf.FloorToInt(MaxPlayers / 4f);
        var remainder = MaxPlayers % 4;

        var tabs = new List<OpTab>();

        for (var i = 0; i < fullTabs; i++)
        {
            Logger.LogInfo($"Creating tab: {i}");
            tabs.Add(GenerateOpTab(4, i * 4));
        }
        if (remainder != 0)
        {
            Logger.LogInfo($"Creating Remainder tab: {remainder}");
            tabs.Add(GenerateOpTab(remainder, fullTabs * 4));
        }

        this.Tabs = tabs.ToArray();
    }

    private OpTab GenerateOpTab(int maxPlayers, int plrNumOffset)
    {
        var tab = new OpTab(this, $"Players {plrNumOffset + 1}-{plrNumOffset + 4}");

        var button = new OpSimpleButton(new Vector2(25f, 500f), new Vector2(550f, 30f), "Enable/Disable All");
        tab.AddItems(button);

        var enableCheckBoxes = new List<OpCheckBox>();

        for (var i = 0; i < maxPlayers; i++)
        {
            var offset = 150f * i;
            var vOffset = -100f;

            var enableCheckbox = new OpCheckBox(UseHeadRagList[plrNumOffset + i], 10f + offset, 520f + vOffset);
            var uiArr = new UIelement[]
            {
                new OpLabel(10f + offset, 550f + vOffset, $"Player {plrNumOffset + 1 + i}"),

                new OpLabel(40f + offset, 520f + vOffset, "Enable"),
                enableCheckbox,

                new OpLabel(10f + offset, 490f + vOffset, "Length (default = 5)"),
                new OpSlider(HeadRagLengthList[plrNumOffset + i], new Vector2(10f + offset, 460f + vOffset), 100),

                new OpLabel(40f + offset, 430f + vOffset, "Raibow"),
                new OpCheckBox(HeadRagColorRainbowList[plrNumOffset + i], 10f + offset, 430f + vOffset),

                new OpLabel(10f + offset, 400f + vOffset, "Color"),
                new OpColorPicker(HeadRagColorList[plrNumOffset + i], new Vector2(-3f + i * 3f + offset, 240f + vOffset))
            };
            enableCheckBoxes.Add(enableCheckbox);
            tab.AddItems(uiArr);
        }

        button.OnClick += ButtonOnOnClick;

        void ButtonOnOnClick(UIfocusable trigger)
        {
            var enabled = enableCheckBoxes.First().GetValueBool();
            foreach (var checkBox in enableCheckBoxes)
            {
                checkBox.SetValueBool(!enabled);
            }
        }

        return tab;
    }

    public override void Update()
    {
        foreach (var tab in Tabs)
        {
            var playerLabels = tab.items.OfType<OpLabel>().Where(x => x.text.StartsWith("Player")).ToList();
            var colorPickers = tab.items.OfType<OpColorPicker>().ToList();

            for (var i = 0; i < playerLabels.Count; i++)
                playerLabels[i].color = colorPickers[i].valueColor;
        }
    }
}