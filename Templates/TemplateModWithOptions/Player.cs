using UnityEngine;

namespace TemplateMod;

public partial class TemplateMod
{
    private void PlayerOnctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractcreature, World world)
    {
        orig(self, abstractcreature, world); //Always call original code, either before or after your code, depending on what you need to achieve
        
        self.slugcatStats.runspeedFac = Options.PlayerSpeed.Value;
        Debug.Log($"Player {self.playerState.playerNumber} feels like zooming.");
        Debug.Log($"Player {self.playerState.playerNumber}'s run speed: {self.slugcatStats.runspeedFac}");
    }
}