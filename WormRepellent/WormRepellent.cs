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

namespace WormRepellent;

[BepInPlugin("NoirCatto.WormRepellent", "WormRepellent", "1.0.0")]
public partial class WormRepellent : BaseUnityPlugin
{
    public void OnEnable()
    {
        On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
    }

    public bool IsInit;
    public void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return;
            
            On.GarbageWorm.ctor += GarbageWormOnctor;
            On.GarbageWorm.Extend += GarbageWormOnExtend;
            
            IsInit = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    public void GarbageWormOnExtend(On.GarbageWorm.orig_Extend orig, GarbageWorm self)
    {
        orig(self);
        self.grabSpears = false;
    }

    public void GarbageWormOnctor(On.GarbageWorm.orig_ctor orig, GarbageWorm self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);
        self.grabSpears = false;
    }
}
