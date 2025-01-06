using UnityEngine;

namespace Bandanas;

public class HeadRag
{
    public readonly PlayerData Data;
    public Player Owner => Data.Owner;

    public bool UseRainbow => Bandanas.ModOptions.HeadRagRainbow(Owner.playerState.playerNumber);
    public Vector2[][,] RagSize = new Vector2[2][,];
    public Color[] RagColor = new Color[2];
    public Color[][] CustomColors = new Color[2][];
    public SharedPhysics.TerrainCollisionData ScratchTerrainCollisionData = new SharedPhysics.TerrainCollisionData();
    public Vector2 LastRotation;

    public int[] SpriteIndex = new int[3];

    public HeadRag(PlayerData data)
    {
        Data = data;

        var color = Bandanas.ModOptions.HeadRagColor(Owner.playerState.playerNumber);

        var treshold = 0.15f;
        var minLength = Bandanas.ModOptions.HeadRagLength(Owner.playerState.playerNumber);
        var maxLength = Mathf.CeilToInt(minLength * 1.8f);
        RagSize[0] = new Vector2[Random.Range(minLength, Random.Range(minLength, maxLength)), 6];
        RagSize[1] = new Vector2[Random.Range(minLength, Random.Range(minLength, maxLength)), 6];
        RagColor[0] = color;
        RagColor[1] = new Color(color.r > treshold ? color.r - treshold : color.r * 0.5f, color.g > treshold ? color.g - treshold : color.g * 0.5f, color.b > treshold ? color.b - treshold : color.b * 0.5f);
    }

    public Vector2 RagAttachPos(int i, float timeStacker, PlayerGraphics self)
    {
        return Vector2.Lerp(self.head.lastPos + new Vector2(i == 0 ? -5f : 5f, 0f), self.head.pos + new Vector2(i == 0 ? -5f : 5f, 0f), timeStacker) + Vector3.Slerp(LastRotation, self.head.connection.Rotation, timeStacker).ToVector2InPoints() * 15f;
    }
}