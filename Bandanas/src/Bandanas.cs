using System;
using System.Security;
using System.Security.Permissions;
using BepInEx;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Bandanas;

[BepInPlugin("NoirCatto.Bandanas", "Bandanas", "1.0.0")]
public partial class Bandanas : BaseUnityPlugin
{
    public static BandanasOptions ModOptions;

    public Bandanas()
    {
        try
        {
            ModOptions = new BandanasOptions(this, Logger);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }
    private void OnEnable()
    {
        try
        {
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    private bool IsInit;
    private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return;

            ModOptions.InternalInit();

            On.PlayerGraphics.InitiateSprites += PlayerGraphicsOnInitiateSprites;
            On.PlayerGraphics.AddToContainer += PlayerGraphicsOnAddToContainer;
            On.PlayerGraphics.DrawSprites += PlayerGraphicsOnDrawSprites;
            On.PlayerGraphics.Update += PlayerGraphicsOnUpdate;
            On.Player.NewRoom += PlayerOnNewRoom;

            MachineConnector.SetRegisteredOI("NoirCatto.Bandanas", ModOptions);
            IsInit = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

}
