using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using UnityEngine;
using RWCustom;
using BepInEx;
using BepInEx.Logging;
using MonoMod.Cil;
using MoreSlugcats;
using Debug = UnityEngine.Debug;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ExplodingBatflies;

[BepInPlugin("NoirCatto.ExplodingBatflies", "ExplodingBatflies", "1.0.0")]
public partial class ExplodingBatflies : BaseUnityPlugin
{
    private ExplodingBatfliesOptions Options;
    
    public ExplodingBatflies()
    {
        try
        {
            Options = new ExplodingBatfliesOptions(this, Logger);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
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
        try
        {
            if (IsInit) return;
            
            FlyHooks();

            MachineConnector.SetRegisteredOI("NoirCatto.ExplodingBatflies", Options);
            IsInit = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }
    
}
