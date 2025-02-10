using System;
using RWCustom;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bandanas;

public partial class Bandanas
{
    private void PlayerGraphicsOnInitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sleaser, RoomCamera rcam)
    {
        if (!self.player.TryGetData(out var data))
        {
            orig(self, sleaser, rcam);
            return;
        }

        if (!rcam.game.DEBUGMODE)
        {
            data.CallingAddToContainerFromOrigInitiateSprites = true;
        }

        orig(self, sleaser, rcam);

        if (!rcam.game.DEBUGMODE)
        {
            data.TotalSprites = sleaser.sprites.Length;
            data.Rag.SpriteIndex[0] = data.TotalSprites; //Band
            data.Rag.SpriteIndex[1] = data.TotalSprites + 1; //Rag
            data.Rag.SpriteIndex[2] = data.TotalSprites + 2; //Rag2

            Array.Resize(ref sleaser.sprites, data.TotalSprites + data.NewSprites);

            var headBandFront = new FSprite("Symbol_Rock", false);
            var trgMesh = TriangleMesh.MakeLongMesh(data.Rag.RagSize[0].GetLength(0), false, data.Rag.UseRainbow);
            var trgMesh2 = TriangleMesh.MakeLongMesh(data.Rag.RagSize[1].GetLength(0), false, data.Rag.UseRainbow);

            headBandFront.color = data.Rag.RagColor[0];
            headBandFront.SetPosition(self.head.pos);

            if (data.Rag.UseRainbow)
            {
                var hueOffset = Random.Range(0f, Random.Range(0f, 1f));

                data.Rag.CustomColors[0] = new Color[trgMesh.verticeColors.Length];
                for (var i = 0; i < trgMesh.verticeColors.Length; i++)
                {
                    data.Rag.CustomColors[0][i] = new HSLColor(Mathf.Clamp(((float)i / (float)trgMesh.verticeColors.Length) + hueOffset, 0f, 1f), Random.Range(0.45f, 1f), 0.5f).rgb; //Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);
                    trgMesh.verticeColors[i] = data.Rag.CustomColors[0][i];
                }

                data.Rag.CustomColors[1] = new Color[trgMesh2.verticeColors.Length];
                for (var i = 0; i < trgMesh2.verticeColors.Length; i++)
                {
                    var grCol = new HSLColor(Mathf.Clamp(((float)i / (float)trgMesh.verticeColors.Length) + hueOffset, 0f, 1f), Random.Range(0f, 1f), 0.5f).rgb;
                    grCol = new Color(grCol.grayscale, grCol.grayscale, grCol.grayscale);
                    data.Rag.CustomColors[1][i] = grCol; //Random.ColorHSV(0f, 1f, 0f, 0.5f);
                    trgMesh2.verticeColors[i] = data.Rag.CustomColors[1][i];
                }
            }

            trgMesh.color = data.Rag.RagColor[0];
            trgMesh2.color = data.Rag.RagColor[1];

            if (!data.Rag.UseRainbow)
            {
                trgMesh.shader = rcam.game.rainWorld.Shaders["JaggedSquare"];
                trgMesh2.shader = rcam.game.rainWorld.Shaders["JaggedSquare"];
            }

            sleaser.sprites[data.Rag.SpriteIndex[0]] = headBandFront;
            sleaser.sprites[data.Rag.SpriteIndex[1]] = trgMesh;
            sleaser.sprites[data.Rag.SpriteIndex[2]] = trgMesh2;
        }

        data.CallingAddToContainerFromOrigInitiateSprites = false;
        self.AddToContainer(sleaser, rcam, null);
    }

    private void PlayerGraphicsOnAddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sleaser, RoomCamera rcam, FContainer newcontatiner)
    {
        if (!self.player.TryGetData(out var data))
        {
            orig(self, sleaser, rcam, newcontatiner);
            return;
        }

        if (data.CallingAddToContainerFromOrigInitiateSprites) return;

        orig(self, sleaser, rcam, newcontatiner);

        if (!rcam.game.DEBUGMODE)
        {
            var container = rcam.ReturnFContainer("Midground");
            container.AddChild(sleaser.sprites[data.Rag.SpriteIndex[0]]);
            container.AddChild(sleaser.sprites[data.Rag.SpriteIndex[1]]);
            container.AddChild(sleaser.sprites[data.Rag.SpriteIndex[2]]);
            sleaser.sprites[data.Rag.SpriteIndex[0]].MoveBehindOtherNode(sleaser.sprites[9]);
            sleaser.sprites[data.Rag.SpriteIndex[1]].MoveBehindOtherNode(sleaser.sprites[0]);
            sleaser.sprites[data.Rag.SpriteIndex[2]].MoveBehindOtherNode(sleaser.sprites[0]);
        }
    }

    private void PlayerGraphicsOnDrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sleaser, RoomCamera rcam, float timestacker, Vector2 campos)
    {
        orig(self, sleaser, rcam, timestacker, campos);

        if (!self.player.TryGetData(out var data)) return;
        if (rcam.game.DEBUGMODE) return;

        var isCrawl = self.player.bodyMode == Player.BodyModeIndex.Crawl || self.player.bodyMode == Player.BodyModeIndex.CorridorClimb;

        var headPos = Vector2.Lerp(self.head.lastPos, self.head.pos, timestacker);
        var bodyZeroPos = Vector2.Lerp(self.drawPositions[0, 1], self.drawPositions[0, 0], timestacker);
        var bodyOnePos = Vector2.Lerp(self.drawPositions[1, 1], self.drawPositions[1, 0], timestacker);

        var newPos = new Vector2(0f, 3.5f);

        var dirUp = (bodyZeroPos - bodyOnePos).normalized;

        newPos = Custom.RotateAroundOrigo(newPos, Custom.VecToDeg(dirUp));
        newPos = headPos + newPos + new Vector2(isCrawl ? data.FlipDirection == 1 ? -4f : 4f : 0f, isCrawl ? 3f : 0f);

        sleaser.sprites[data.Rag.SpriteIndex[0]].SetPosition(newPos - campos);
        sleaser.sprites[data.Rag.SpriteIndex[0]].rotation = isCrawl ? 0f : Custom.VecToDeg(self.head.connection.Rotation);
        sleaser.sprites[data.Rag.SpriteIndex[0]].color = data.Rag.RagColor[0];
        sleaser.sprites[data.Rag.SpriteIndex[0]].scaleY = isCrawl ? 0.4f : 0.5f;
        sleaser.sprites[data.Rag.SpriteIndex[0]].scaleX = isCrawl ? 1.0f : 1.3f;

        for (var i = 0; i < 2; i++)
        {
            if (!data.Rag.UseRainbow)
            {
                sleaser.sprites[data.Rag.SpriteIndex[1 + i]].color = data.Rag.RagColor[i];
            }
            else
            {
                sleaser.sprites[data.Rag.SpriteIndex[1 + i]].color = Color.white;

                for (var j = 0; j < data.Rag.CustomColors[i].Length; j++)
                {
                    ((TriangleMesh)sleaser.sprites[data.Rag.SpriteIndex[1 + i]]).verticeColors[j] = data.Rag.CustomColors[i][j];
                }

            }

            var num = 0f;
            var a = data.Rag.RagAttachPos(i, timestacker, self);
            for (var j = 0; j < data.Rag.RagSize[i].GetLength(0); j++)
            {
                var f = (float)j / (float)(data.Rag.RagSize[i].GetLength(0) - 1);
                var vector = Vector2.Lerp(data.Rag.RagSize[i][j, 1], data.Rag.RagSize[i][j, 0], timestacker);
                var num2 = (2f + 2f * Mathf.Sin(Mathf.Pow(f, 2f) * 3.1415927f)) * Vector3.Slerp(data.Rag.RagSize[i][j, 4], data.Rag.RagSize[i][j, 3], timestacker).x;
                var normalized = (a - vector).normalized;
                var a2 = Custom.PerpendicularVector(normalized);
                var d = Vector2.Distance(a, vector) / 5f;
                ((TriangleMesh)sleaser.sprites[data.Rag.SpriteIndex[1 + i]]).MoveVertice(j * 4, a - normalized * d - a2 * (num2 + num) * 0.5f - campos);
                ((TriangleMesh)sleaser.sprites[data.Rag.SpriteIndex[1 + i]]).MoveVertice(j * 4 + 1, a - normalized * d + a2 * (num2 + num) * 0.5f - campos);
                ((TriangleMesh)sleaser.sprites[data.Rag.SpriteIndex[1 + i]]).MoveVertice(j * 4 + 2, vector + normalized * d - a2 * num2 - campos);
                ((TriangleMesh)sleaser.sprites[data.Rag.SpriteIndex[1 + i]]).MoveVertice(j * 4 + 3, vector + normalized * d + a2 * num2 - campos);
                a = vector;
                num = num2;
            }
        }
    }

    private void PlayerGraphicsOnUpdate(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);

        if (!self.player.TryGetData(out var data)) return;
        if (self.player.room.game.DEBUGMODE) return;

        var player = self.player;
        var conRad = 7f;

        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < data.Rag.RagSize[i].GetLength(0); j++)
            {
                var t = (float)j / (float)(data.Rag.RagSize[i].GetLength(0) - 1);
                data.Rag.RagSize[i][j, 1] = data.Rag.RagSize[i][j, 0];
                data.Rag.RagSize[i][j, 0] += data.Rag.RagSize[i][j, 2];
                data.Rag.RagSize[i][j, 2] -= player.firstChunk.Rotation * Mathf.InverseLerp(1f, 0f, (float)j) * 0.8f;
                data.Rag.RagSize[i][j, 4] = data.Rag.RagSize[i][j, 3];
                data.Rag.RagSize[i][j, 3] = (data.Rag.RagSize[i][j, 3] + data.Rag.RagSize[i][j, 5] * Custom.LerpMap(Vector2.Distance(data.Rag.RagSize[i][j, 0], data.Rag.RagSize[i][j, 1]), 1f, 18f, 0.05f, 0.3f)).normalized;
                data.Rag.RagSize[i][j, 5] = (data.Rag.RagSize[i][j, 5] + Custom.RNV() * Random.value * Mathf.Pow(Mathf.InverseLerp(1f, 18f, Vector2.Distance(data.Rag.RagSize[i][j, 0], data.Rag.RagSize[i][j, 1])), 0.3f)).normalized;
                if (player.room.PointSubmerged(data.Rag.RagSize[i][j, 0]))
                {
                    data.Rag.RagSize[i][j, 2] *= Custom.LerpMap(data.Rag.RagSize[i][j, 2].magnitude, 1f, 10f, 1f, 0.5f, Mathf.Lerp(1.4f, 0.4f, t));
                    data.Rag.RagSize[i][j, 2].y += 0.05f;
                    data.Rag.RagSize[i][j, 2] += Custom.RNV() * 0.1f;
                }
                else
                {
                    data.Rag.RagSize[i][j, 2] *= Custom.LerpMap(Vector2.Distance(data.Rag.RagSize[i][j, 0], data.Rag.RagSize[i][j, 1]), 1f, 6f, 0.999f, 0.7f, Mathf.Lerp(1.5f, 0.5f, t));
                    data.Rag.RagSize[i][j, 2].y -= player.room.gravity * Custom.LerpMap(Vector2.Distance(data.Rag.RagSize[i][j, 0], data.Rag.RagSize[i][j, 1]), 1f, 6f, 0.6f, 0f);
                    if (j % 3 == 2 || j == data.Rag.RagSize[i].GetLength(0) - 1)
                    {
                        var terrainCollisionData = data.Rag.ScratchTerrainCollisionData.Set(data.Rag.RagSize[i][j, 0], data.Rag.RagSize[i][j, 1], data.Rag.RagSize[i][j, 2], 1f, new IntVector2(0, 0), false);
                        terrainCollisionData = SharedPhysics.HorizontalCollision(player.room, terrainCollisionData);
                        terrainCollisionData = SharedPhysics.VerticalCollision(player.room, terrainCollisionData);
                        terrainCollisionData = SharedPhysics.SlopesVertically(player.room, terrainCollisionData);
                        data.Rag.RagSize[i][j, 0] = terrainCollisionData.pos;
                        data.Rag.RagSize[i][j, 2] = terrainCollisionData.vel;
                        if (terrainCollisionData.contactPoint.x != 0)
                        {
                            data.Rag.RagSize[i][j, 2].y *= 0.6f;
                        }

                        if (terrainCollisionData.contactPoint.y != 0)
                        {
                            data.Rag.RagSize[i][j, 2].x *= 0.6f;
                        }
                    }
                }
            }

            for (var j = 0; j < data.Rag.RagSize[i].GetLength(0); j++)
            {
                if (j > 0)
                {
                    var normalized = (data.Rag.RagSize[i][j, 0] - data.Rag.RagSize[i][j - 1, 0]).normalized;
                    var num = Vector2.Distance(data.Rag.RagSize[i][j, 0], data.Rag.RagSize[i][j - 1, 0]);
                    var d = (num > conRad) ? 0.5f : 0.25f;
                    data.Rag.RagSize[i][j, 0] += normalized * (conRad - num) * d;
                    data.Rag.RagSize[i][j, 2] += normalized * (conRad - num) * d;
                    data.Rag.RagSize[i][j - 1, 0] -= normalized * (conRad - num) * d;
                    data.Rag.RagSize[i][j - 1, 2] -= normalized * (conRad - num) * d;
                    if (j > 1)
                    {
                        normalized = (data.Rag.RagSize[i][j, 0] - data.Rag.RagSize[i][j - 2, 0]).normalized;
                        data.Rag.RagSize[i][j, 2] += normalized * 0.2f;
                        data.Rag.RagSize[i][j - 2, 2] -= normalized * 0.2f;
                    }

                    if (j < data.Rag.RagSize[i].GetLength(0) - 1)
                    {
                        data.Rag.RagSize[i][j, 3] = Vector3.Slerp(data.Rag.RagSize[i][j, 3], (data.Rag.RagSize[i][j - 1, 3] * 2f + data.Rag.RagSize[i][j + 1, 3]) / 3f, 0.1f);
                        data.Rag.RagSize[i][j, 5] = Vector3.Slerp(data.Rag.RagSize[i][j, 5], (data.Rag.RagSize[i][j - 1, 5] * 2f + data.Rag.RagSize[i][j + 1, 5]) / 3f, Custom.LerpMap(Vector2.Distance(data.Rag.RagSize[i][j, 1], data.Rag.RagSize[i][j, 0]), 1f, 8f, 0.05f, 0.5f));
                    }
                }
                else
                {
                    data.Rag.RagSize[i][j, 0] = data.Rag.RagAttachPos(i, 1f, self);
                    data.Rag.RagSize[i][j, 2] *= 0f;
                }
            }
        }

        data.Rag.LastRotation = self.head.connection.Rotation;
    }

    private void PlayerOnNewRoom(On.Player.orig_NewRoom orig, Player self, Room newroom)
    {
        orig(self, newroom);
        if (!self.TryGetData(out var data)) return;

        var graphics = self.graphicsModule as PlayerGraphics;
        if (graphics == null) return;

        for (var i = 0; i < 2; i++)
        {
            var vector = data.Rag.RagAttachPos(i, 1f, graphics);
            for (var j = 0; j < data.Rag.RagSize[i].GetLength(0); j++)
            {
                data.Rag.RagSize[i][j, 0] = vector;
                data.Rag.RagSize[i][j, 1] = vector;
                data.Rag.RagSize[i][j, 2] *= 0f;
            }
        }
    }

}