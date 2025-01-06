using System.Runtime.CompilerServices;
using UnityEngine;

namespace Bandanas;

public static class PlayerCWT
{
    public static readonly ConditionalWeakTable<Player, PlayerData> PlayerDeets = new ConditionalWeakTable<Player, PlayerData>();
    public static PlayerData GetData(this Player player) => PlayerDeets.GetValue(player, _ => new(player));
    public static bool TryGetData(this Player player, out PlayerData playerData)
    {
        if (Bandanas.ModOptions.UseHeadRag(player.playerState.playerNumber))
        {
            playerData = GetData(player);
            return true;
        }

        playerData = null;
        return false;
    }
}

public class PlayerData
{
    public readonly Player Owner;
    public HeadRag Rag;

    public int TotalSprites;
    public int NewSprites;

    public bool CallingAddToContainerFromOrigInitiateSprites;

    public int FlipDirection
    {
        get
        {
            if (Mathf.Abs(Owner.bodyChunks[0].pos.x - Owner.bodyChunks[1].pos.x) < 2f)
            {
                return Owner.flipDirection;
            }
            else
            {
                return Owner.bodyChunks[0].pos.x > Owner.bodyChunks[1].pos.x ? 1 : -1;
            }
        }
    }

    public PlayerData(Player owner)
    {
        Owner = owner;
        Rag = new HeadRag(this);
        NewSprites = Rag.SpriteIndex.Length;
    }
}