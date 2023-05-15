using Noise;
using RWCustom;
using UnityEngine;

namespace ExplodingBatflies;

public partial class ExplodingBatflies
{
    public void FlyHooks()
    {
        On.Fly.Update += FlyOnUpdate;
    }

    private void FlyOnUpdate(On.Fly.orig_Update orig, Fly self, bool eu)
    {
        orig(self, eu);
        if (!self.State.alive) return;
        if (self.room == null) return;
        
        foreach (var physicalObject in self.room.physicalObjects[1]) //We should not need to check layer 0 and 2
        {
            if (physicalObject is Fly or not Creature) continue; //Decided to only collide with creetchurs
            if (!(Mathf.Abs(self.bodyChunks[0].pos.x - physicalObject.bodyChunks[0].pos.x) < self.collisionRange + physicalObject.collisionRange) ||
                !(Mathf.Abs(self.bodyChunks[0].pos.y - physicalObject.bodyChunks[0].pos.y) < self.collisionRange + physicalObject.collisionRange)) continue;

            var alreadyCollidedThisFrame = false;
            
            foreach (var chunk in self.bodyChunks)
            {
                foreach (var poChunk in physicalObject.bodyChunks)
                {
                    if (!chunk.collideWithObjects || !poChunk.collideWithObjects) break;
                    if (!Custom.DistLess(chunk.pos, poChunk.pos, chunk.rad + poChunk.rad)) break;

                    // Collision!
                    if (!alreadyCollidedThisFrame)
                    {
                        FlyExplode(self);
                        alreadyCollidedThisFrame = true;
                    }
                }
            }

        }
    }

    private void FlyExplode(Fly self)
    {
        var room = self.room;
        if (room != null)
        {
            var pos = self.firstChunk.pos;
            
            var explosionLifeTime = Options.Lifetime.Value;
            var explosionRadius = Options.Radius.Value;
            var explosionForce = Options.Force.Value;
            var explosionDamage = Options.RandomDMG.Value ? Random.Range(0f, Options.Damage.Value) : Options.Damage.Value;
            var explosionMinStun = Options.MinStun.Value;
            var explosionStun = Options.Stun.Value;

            room.AddObject(new Explosion(room, self, pos, explosionLifeTime, explosionRadius, explosionForce, explosionDamage, explosionStun, 0.25f, self, 1f, explosionMinStun, 1f));
            room.AddObject(new Explosion.ExplosionLight(pos, 28f * explosionForce, 1f, explosionLifeTime + 1, Options.RandomColor.Value ? GetRandomColor() : Options.ExplosionColor.Value));
            room.AddObject(new Explosion.ExplosionLight(pos, 23f * explosionForce, 1f, (int)(explosionLifeTime * 0.5f), Options.RandomColor.Value ? GetRandomColor() : Options.ExplosionColor.Value));
            room.AddObject(new ExplosionSpikes(room, pos, (int)Random.Range(14f, 14 * explosionDamage), 30f, explosionLifeTime + 3, explosionForce - 2, 170f, Options.RandomColor.Value ? GetRandomColor() : Options.ExplosionColor.Value));
            room.AddObject(new ShockWave(pos, 430f, 0.045f, explosionLifeTime - 2, false));
            for (var j = 0; j < 20; j++)
            {
                var a = Custom.RNV();
                room.AddObject(new Spark(pos + a * Random.value * 40f, a * Mathf.Lerp(4f, 30f, Random.value), Options.RandomColor.Value ? GetRandomColor() : Options.ExplosionColor.Value, null, 4, 18));
            }

            room.ScreenMovement(pos, default, 0.7f);

            room.PlaySound(SoundID.Fire_Spear_Explode, pos);
            room.InGameNoise(new InGameNoise(pos, 4000f * explosionDamage, self, 1f));
        }
        self.LoseAllGrasps();
        self.Destroy(); // Removing batfly
    }

    private static Color GetRandomColor() => Color.HSVToRGB(Random.value, Random.value, 1f);
}