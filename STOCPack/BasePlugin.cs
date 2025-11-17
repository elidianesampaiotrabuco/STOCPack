using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STOCPack
{
    [BepInPlugin("starrie.bbplus.stoc", "Shaldi's Tower of Chaos Pack", "0.0.1")]

    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

    public class BasePlugin : BaseUnityPlugin
    {
        public AssetManager assetMan = new AssetManager();
        private static string npcSubDirectory = "Textures/NPCs";
        public void Awake()
        {
            Harmony harmony = new Harmony("starrie.bbplus.stoc");

            harmony.PatchAll();

            GetAssets();

            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterAssets(), LoadingEventOrder.Pre);
            GeneratorManagement.Register(this, GenerationModType.Addend, AddObjects);
        }

        private IEnumerator RegisterAssets()
        {
            yield return 1;

            yield return "Building NPCs...";

            Shaldi PinkGuy = new NPCBuilder<Shaldi>(Info)
            .SetName("Shaldi")
            .SetEnum("ThatPinkGuy")
            .IgnorePlayerOnSpawn()
            .AddLooker()
            .AddTrigger()
            .AddSpawnableRoomCategories(new RoomCategory[] { RoomCategory.Hall })
            .SetPoster(assetMan.Get<Texture2D>("Shaldi_Poster"), "Shaldi", "Do not let him catch you.")
            .Build();

            yield return "Doing some miscellaneous stuff...";

            PinkGuy.shaldiSprite = assetMan.Get<Sprite>("ShaldiSprite");

            assetMan.Add<NPC>("Shaldi", PinkGuy);

        }

        private void GetAssets()
        {
            assetMan.Add<Texture2D>("Shaldi", AssetLoader.TextureFromMod(this, npcSubDirectory, "Shaldi/Shaldi.png"));
            assetMan.Add<Texture2D>("Shaldi_Poster", AssetLoader.TextureFromMod(this, npcSubDirectory, "Shaldi/Shaldi_Poster.png"));
            assetMan.Add<Sprite>("ShaldiSprite", AssetLoader.SpriteFromTexture2D(assetMan.Get<Texture2D>("Shaldi"), 50));
        }

        private void AddObjects(string floor, int floorNumber, SceneObject floorObject)
        {
            if (floor.StartsWith("F"))
            {
                floorObject.potentialNPCs.Add(new WeightedNPC()
                {
                    selection = assetMan.Get<NPC>("Shaldi"),
                    weight = floorNumber < 2 ? 100 * floorNumber : 250
                }
                );
            }
            else if (floor == "END")
            {
                floorObject.potentialNPCs.Add(new WeightedNPC()
                {
                    selection = assetMan.Get<NPC>("Shaldi"),
                    weight = 250
                }
                );
            }

            floorObject.MarkAsNeverUnload();
        }
    }
}