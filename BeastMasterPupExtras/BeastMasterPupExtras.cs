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
using MonoMod.Cil;
using MoreSlugcats;
using Debug = UnityEngine.Debug;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace BeastMasterPupExtras;

[BepInPlugin("NoirCatto.BeastMasterPupExtras", "BeastMasterPupExtras", "1.0.0")]
public partial class BeastMasterPupExtras : BaseUnityPlugin
{
    private BeastMasterPupExtrasOptions Options;

    private const bool LoggingEnabled = false;

    public BeastMasterPupExtras()
    {
        try
        {
            Options = new BeastMasterPupExtrasOptions(this, Logger);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }
    private void OnEnable()
    {
        On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
        On.RainWorld.PostModsInit += RainWorldOnPostModsInit;
    }

    private bool IsInit;
    private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return;
            IsInit = true;

            On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
            On.GameSession.ctor += GameSessionOnctor;
            
            On.MoreSlugcats.PlayerNPCState.ToString += PlayerNPCStateOnToString;
            On.MoreSlugcats.PlayerNPCState.LoadFromString += PlayerNPCStateOnLoadFromString;
            On.AbstractCreature.MSCRealizeCustom += AbstractCreatureOnMSCRealizeCustom;
            IL.MoreSlugcats.PlayerNPCState.CycleTick += PlayerNPCStateILCycleTick;

            IL.Player.MovementUpdate += PlayerOnMovementUpdate;
            On.Player.Grabability += PlayerOnGrabability;
            //On.Player.NPCGrabCheck += PlayerOnNPCGrabCheck;
            
            On.Player.Update += PlayerOnUpdate;
            On.Player.Jump += PlayerOnJump;
            On.MoreSlugcats.SlugNPCAI.Move += SlugNPCAIOnMove;
            
            MachineConnector.SetRegisteredOI("NoirCatto.BeastMasterPupExtras", Options);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    private bool IsPostInit;
    private bool PupsPlus;
    private void RainWorldOnPostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsPostInit) return;
            IsPostInit = true;

            if (ModManager.ActiveMods.Any(mod => mod.id == "iwantbread.slugpupstuff"))
            {
                PupsPlus = true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    private void RainWorldGameOnShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
    {
        orig(self);
        ClearMemory();
    }
    private void GameSessionOnctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
    {
        orig(self, game);
        ClearMemory();
    }

    #region Helper Methods

    private void ClearMemory()
    {
        PlayerDeets.Clear();
    }

    private void DebugLog(object data)
    {
        if (LoggingEnabled)
        {
            UnityEngine.Debug.Log(data);
        }
    }

    private void LoggerLog(object data)
    {
        if (LoggingEnabled)
        {
            Logger.LogInfo(data);
        }
    }

    #endregion
}
