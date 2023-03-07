using System;
using System.Linq;
using System.Text.RegularExpressions;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;

namespace BeastMasterPupExtras;

public partial class BeastMasterPupExtras
{
    
    private string PlayerNPCStateOnToString(On.MoreSlugcats.PlayerNPCState.orig_ToString orig, MoreSlugcats.PlayerNPCState self)
    {
        var text = orig(self);
        text += "SlugcatCharacter<cC>" + self.slugcatCharacter.ToString() + "<cB>";
        text += "PlayerNumber<cC>" + self.playerNumber.ToString() + "<cB>";
        return text;
    }

    private void PlayerNPCStateOnLoadFromString(On.MoreSlugcats.PlayerNPCState.orig_LoadFromString orig, MoreSlugcats.PlayerNPCState self, string[] s)
    {
        orig(self, s);
        for (var i = 0; i < s.Length - 1; i++)
        {
            var array = Regex.Split(s[i], "<cC>");
            var text = array[0];

            switch (text)
            {
                case "SlugcatCharacter":
                    self.slugcatCharacter = StringToSlugName(array[1]);
                    break;
                case "PlayerNumber":
                    self.playerNumber = int.Parse(array[1]);
                    break;
            }
        }
    }
    
    private void AbstractCreatureOnMSCRealizeCustom(On.AbstractCreature.orig_MSCRealizeCustom orig, AbstractCreature self)
    {
        orig(self);

        if (self.realizedCreature is Player pl)
        {
            if (self.creatureTemplate.TopAncestor().type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
            {

                pl.SlugCatClass = pl.playerState.slugcatCharacter;
                pl.slugcatStats.name = pl.playerState.slugcatCharacter;
                pl.graphicsModule = new PlayerGraphics(pl); //MSC chara fix
                var malnourished = pl.Malnourished;
                pl.npcCharacterStats = new SlugcatStats(pl.playerState.slugcatCharacter, malnourished);
                var foodNeeded = SlugcatStats.SlugcatFoodMeter(pl.playerState.isPup ? MoreSlugcatsEnums.SlugcatStatsName.Slugpup : pl.playerState.slugcatCharacter);
                pl.slugcatStats.maxFood = foodNeeded.x;
                pl.slugcatStats.foodToHibernate = foodNeeded.y;

                if (pl.playerState.slugcatCharacter == MoreSlugcatsEnums.SlugcatStatsName.Saint)
                {
                    pl.tongue = new Player.Tongue(pl, 0);
                }

                new PupData(pl);
            }
        }
    }
    
    private void PlayerNPCStateILCycleTick(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);
            
            for (var k = 0; k < 3; k++)
            {
                c.GotoNext(MoveType.AfterLabel, i => i.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>("Slugpup"));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate((PlayerNPCState self) =>
                {
                    //LoggerLog($"IL{k}");
                    return self.isPup ? MoreSlugcatsEnums.SlugcatStatsName.Slugpup : self.slugcatCharacter;
                });
                c.Remove();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }
}