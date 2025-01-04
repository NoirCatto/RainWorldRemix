using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace BeastMasterPupExtras;

public partial class BeastMasterPupExtras
{
    public void PlayerOnMovementUpdate(ILContext il)
    {
        //Allow SlugNPC to use Creature Shortcuts
        try
        {
            var c = new ILCursor(il);
            ILLabel label = null;

            c.GotoNext(i => i.MatchLdsfld<ShortcutData.Type>("NPCTransportation"));
            c.GotoNext(i => i.MatchLdarg(0));
            label = il.DefineLabel(c.Next);
            c.GotoPrev(i => i.MatchCallOrCallvirt<PhysicalObject>("get_bodyChunks"));
            c.GotoPrev(MoveType.After, i => i.MatchBrfalse(out _));
            c.Emit(OpCodes.Br, label);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }
    
    private void SlugNPCAIOnMove(On.MoreSlugcats.SlugNPCAI.orig_Move orig, MoreSlugcats.SlugNPCAI self)
    {
        if (!Options.SlugNPCAttemptJump.Value)
        {
            orig(self);
            return;
        }
        
        if (!PupDeets.ContainsKey(self.cat))
        {
            new PupData(self.cat);
        }
        
        if (self.friendTracker.friend is Player mom)
        {
            if (!PlayerDeets.ContainsKey(mom))
            {
                new PlayerData(mom);
            }
            if (PlayerDeets[mom].LastJumps.Any())
            {
                // DebugLog($"Jumps Count: {PlayerDeets[mom].LastJumps.Count}");
                // DebugLog($"LastJump StartPos: {PlayerDeets[mom].LastJumps.Last()[0].x}, {PlayerDeets[mom].LastJumps.Last()[0].y}");
                // DebugLog($"LastJump LandPos: {PlayerDeets[mom].LastJumps.Last()[1].x}, {PlayerDeets[mom].LastJumps.Last()[1].y}");
            }
        }
        orig(self);

        if (PupDeets[self.cat].JumpCounter > 0)
        {
            self.cat.input[0].x = PupDeets[self.cat].JumpDir.x;
            self.cat.input[0].y = PupDeets[self.cat].JumpDir.y;
        }
        

        if (self.behaviorType != SlugNPCAI.BehaviorType.Following && self.behaviorType != SlugNPCAI.BehaviorType.Idle) return;
        if (self.abstractAI.DoIHaveAPathToCoordinate(self.abstractAI.destination))
        {
            PupDeets[self.cat].StuckCounter = 0;
            return;
        }

        PupDeets[self.cat].StuckCounter++;
        if (PupDeets[self.cat].StuckCounter < 40) return;
        if (self.jumping) return;

        if (self.friendTracker.friend is Player mother)
        {
            if (self.cat.room != mother.room) return;
            
            LoggerLog("Can't find path to Mother.");
            LoggerLog($"Room: {self.cat.room.abstractRoom.name}");
            LoggerLog($"Cat Pos: {self.cat.abstractCreature.pos.x}, {self.cat.abstractCreature.pos.y}"); 
            LoggerLog($"Mother Pos: {mother.abstractCreature.pos.x}, {mother.abstractCreature.pos.y}");

            //Potential Jumps within 5 units of Pup, where the jump was longer than x units, ordered by closest landing distance to mom
            //Todo: Check if destination will bring pup closer to mother
            //Todo Maybe?: The pup could try getting closer to startPos before jumping
            var considerJumps = PlayerDeets[mother].LastJumps.
                Where(x => Custom.DistLess(self.cat.abstractCreature.pos, x[0], 2.5f) && !Custom.DistLess(x[0], x[1], 3f)).
                OrderByDescending(x => Custom.DistLess(mother.abstractCreature.pos, x[1], 5));

            if (considerJumps.Any())
            {
                var jumpOfInterest = considerJumps.First();
                if (self.OnVerticalBeam())
                {
                    if (self.cat.abstractCreature.pos.y < jumpOfInterest[1].y + 0.5f)
                    {
                        DebugLog($"{self.cat.SlugCatClass} NPC wants to jump, climbing up to jump position!");
                        self.cat.input[0].y = 1;
                        return;
                    }
                }

                DebugLog($"{self.cat.SlugCatClass} NPC attempting to jump!");
                DebugLog($"Cat Pos: {self.cat.abstractCreature.pos.x}, {self.cat.abstractCreature.pos.y}");
                DebugLog($"Mother Pos: {mother.abstractCreature.pos.x}, {mother.abstractCreature.pos.y}");
                DebugLog($"Jump StartPos x: {jumpOfInterest[0].x}, y: {jumpOfInterest[0].y}");
                DebugLog($"Jump Destination x: {jumpOfInterest[1].x}, y: {jumpOfInterest[1].y}");
                Jump(self, jumpOfInterest[1]);
            }
        }

    }

    private void Jump(SlugNPCAI self, WorldCoordinate destination)
    {
        self.jumping = true;

        var dir = Custom.DirVec(new Vector2(self.cat.abstractCreature.pos.x, self.cat.abstractCreature.pos.y), new Vector2(destination.x, destination.y));
        PupDeets[self.cat].JumpDir = new IntVector2(Convert.ToInt32(Math.Round(dir.x, MidpointRounding.AwayFromZero)), Convert.ToInt32(Math.Round(dir.y, MidpointRounding.AwayFromZero)));
        
        //self.cat.input[0].x = Convert.ToInt32(dir.x);
        self.cat.input[0].x = PupDeets[self.cat].JumpDir.x;
        
        if (self.OnVerticalBeam())
        {
            self.cat.input[0].y = 0;
        }
        else
        {
            //self.cat.input[0].y = Convert.ToInt32(dir.y);
            self.cat.input[0].y = PupDeets[self.cat].JumpDir.y;
        }

        self.cat.input[0].jmp = true;
        DebugLog($"input X: {self.cat.input[0].x}");
        DebugLog($"input Y: {self.cat.input[0].y}");
        DebugLog($"input jmp: {self.cat.input[0].jmp}");

        self.forceJump = 10;
        PupDeets[self.cat].StuckCounter = 0;
        PupDeets[self.cat].JumpCounter = 20;
    }
    
}