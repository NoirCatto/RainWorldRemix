using System;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using RWCustom;
using BepInEx;
using Debug = UnityEngine.Debug;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace TemplateMod;

[BepInPlugin("AuthorName.ModName", "Mod Name", "1.0.0")]
public partial class TemplateMod : BaseUnityPlugin
{
    private TemplateModOptions Options;

    public TemplateMod()
    {
        try
        {
            Options = new TemplateModOptions(this, Logger);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }
    private void OnEnable()
    {
        On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
    }

    private bool IsInit;
    private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        if (IsInit) return;

        try
        {
            IsInit = true;

            //Your hooks go here
            On.Player.ctor += PlayerOnctor;

            MachineConnector.SetRegisteredOI("AuthorName.ModName", Options);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

}
