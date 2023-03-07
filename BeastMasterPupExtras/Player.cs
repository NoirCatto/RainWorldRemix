using System.Collections.Generic;
using MoreSlugcats;
using RWCustom;

namespace BeastMasterPupExtras;

public partial class BeastMasterPupExtras
{
    #region PlayerData
    private static readonly Dictionary<Player, PlayerData> PlayerDeets = new Dictionary<Player, PlayerData>();
    public class PlayerData
    {
        public readonly Player Cat;
        public bool Jumping;
        public WorldCoordinate JumpStartPos;
        public WorldCoordinate JumpLandPos;

        protected const int StoredJumps = 50;
        public readonly Queue<WorldCoordinate[]> LastJumps = new Queue<WorldCoordinate[]>();

        protected Player.BodyModeIndex lastBodyMode;
        protected Player.AnimationIndex lastAnimation;
        

        public PlayerData(Player cat)
        {
            Cat = cat;
            AddToDictionary(cat);
            
        }

        public virtual void AddToDictionary(Player cat)
        {
            PlayerDeets.Add(Cat, this);
        }

        #region uh beam stuff I guess
        public bool OnVerticalBeam()
        {
            return Cat.bodyMode == Player.BodyModeIndex.ClimbingOnBeam;
        }
        public bool OnHorizontalBeam()
        {
            return Cat.animation == Player.AnimationIndex.HangFromBeam || Cat.animation == Player.AnimationIndex.StandOnBeam;
        }
        public bool OnAnyBeam()
        {
            return OnVerticalBeam() || OnHorizontalBeam();
        }
        public bool WasOnVerticalBeam()
        {
            return lastBodyMode == Player.BodyModeIndex.ClimbingOnBeam;
        }
        public bool WasOnHorizontalBeam()
        {
            return lastAnimation == Player.AnimationIndex.HangFromBeam || lastAnimation == Player.AnimationIndex.StandOnBeam;
        }
        public bool WasOnAnyBeam()
        {
            return WasOnVerticalBeam() || WasOnHorizontalBeam();
        }
        #endregion

        public virtual void Update(bool eu)
        {
            if (Jumping)
            {
                if (Cat.bodyChunks[1].lastContactPoint == new IntVector2(0, 0) && Cat.bodyChunks[1].contactPoint == new IntVector2(0, -1) || !WasOnAnyBeam() && OnAnyBeam())
                {
                    Jumping = false;
                    JumpLandPos = Cat.abstractCreature.pos;
                    if (LastJumps.Count >= StoredJumps)
                    {
                        LastJumps.Dequeue();
                    }
                    LastJumps.Enqueue(new []{JumpStartPos, JumpLandPos});
                }
            }
            
            lastBodyMode = Cat.bodyMode;
            lastAnimation = Cat.animation;
        }

    }
    
    private static readonly Dictionary<Player, PupData> PupDeets = new Dictionary<Player, PupData>();

    public class PupData : PlayerData
    {
        public int StuckCounter;
        public int JumpCounter;

        public IntVector2 JumpDir;

        public PupData(Player cat) : base(cat)
        {
        }
        
        public override void AddToDictionary(Player cat)
        {
            PupDeets.Add(Cat, this);
        }

        public override void Update(bool eu)
        {
            if (JumpCounter > 0)
            {
                JumpCounter--;
            }
            
            if (Jumping)
            {
                if (Cat.bodyChunks[1].lastContactPoint == new IntVector2(0, 0) && Cat.bodyChunks[1].contactPoint == new IntVector2(0, -1) || !WasOnAnyBeam() && OnAnyBeam())
                {
                    Jumping = false;
                    JumpCounter = 0;
                }
            }
            
            lastBodyMode = Cat.bodyMode;
            lastAnimation = Cat.animation;
        }
    }
    #endregion

    private void PlayerOnUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);
        if (PlayerDeets.ContainsKey(self))
        {
            PlayerDeets[self].Update(eu);
        }

        if (PupDeets.ContainsKey(self))
        {
            PupDeets[self].Update(eu);
        }
    }
    private void PlayerOnJump(On.Player.orig_Jump orig, Player self)
    {
        orig(self);
        if (PlayerDeets.ContainsKey(self))
        {
            PlayerDeets[self].Jumping = true;
            PlayerDeets[self].JumpStartPos = self.abstractCreature.pos;
        }

        if (PupDeets.ContainsKey(self))
        {
            PupDeets[self].Jumping = true;
        }
    }
    
    private Player.ObjectGrabability PlayerOnGrabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
    {
        if (obj != self && obj is Player pl && pl.abstractCreature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
        {
            if (self.abstractCreature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
            {
                return Player.ObjectGrabability.CantGrab;
            }
            
            if (pl.playerState.isPup)
            {
                return Player.ObjectGrabability.OneHand;
            }
            return Player.ObjectGrabability.BigOneHand;
        }
        
        return orig(self, obj);
    }
    
    // Todo: Will move this to RideableLizards
    // private bool PlayerOnNPCGrabCheck(On.Player.orig_NPCGrabCheck orig, Player self, PhysicalObject item)
    // {
    //     if (ModManager.ActiveMods.Exists(x => x.id == "NoirCat.RideableLizards"))
    //     {
    //         LoggerLog("Rideable Lizards found");
    //         if (item is Lizard liz)
    //         {
    //             if (LikesPlayer(self, liz)) //Only allowed to grab tamed lizards
    //             {
    //                 return true;
    //             }
    //         }
    //     }
    //
    //     return orig(self, item);
    // }
}