using UnityEngine;

namespace TemplateMod;

public partial class TemplateMod
{
    private void PlayerOnUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu); //Always call original code, either before or after your code, depending on what you need to achieve

        self.slugcatStats.runspeedFac += 0.01f;
        Debug.Log($"Player {self.playerState.playerNumber} feels like zooming.");
        Debug.Log($"Player {self.playerState.playerNumber}'s run speed: {self.slugcatStats.runspeedFac}");
    }
}