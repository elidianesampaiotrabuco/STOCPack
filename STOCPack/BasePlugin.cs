using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace STOCPack
{
    [BepInPlugin("starrie.bbplus.stoc", "Shaldi's Tower of Chaos Pack", "0.0.2")]

    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

    public class BasePlugin : BaseUnityPlugin
    {
        public AssetManager assetMan = new AssetManager();
        private static string npcSubDirectory = "Textures/NPCs";
        private static string npcAudioSubDirectory = "Audio/NPCs";
        private static string itemSubDirectory = "Textures/Items";
        private static string itemAudioSubDirectory = "Audio/Items";
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

            Stroketon plankton = new NPCBuilder<Stroketon>(Info)
            .SetName("Stroketon")
            .SetEnum("Plankton")
            .SetMinMaxAudioDistance(1, 300)
            .IgnorePlayerOnSpawn()
            .AddSpawnableRoomCategories(new RoomCategory[] { RoomCategory.Hall, RoomCategory.Class, RoomCategory.Office, RoomCategory.Faculty })
            .SetPoster(assetMan.Get<Texture2D>("Stroketon_Poster"), "Stroketon", "Dr. Jr.")
            .Build();

            yield return "Building items...";

            ItemObject krabbyPatty = new ItemBuilder(Info)
    .SetNameAndDescription("Krabby Patty", "TBA")
    .SetSprites(assetMan.Get<Sprite>("KrabbyPattySmallSprite"), assetMan.Get<Sprite>("KrabbyPattyLargeSprite"))
    .SetEnum("KrabbyPatty")
    .SetShopPrice(350)
    .SetGeneratorCost(40)
    .SetItemComponent<ITM_KrabbyPatty>()
    .SetMeta(ItemFlags.Persists, new string[] { "food" })
    .Build();
            //((ITM_KrabbyPatty)krabbyPatty.item).eatSound = (SoundObject)((ITM_ZestyBar)ItemMetaStorage.Instance.FindByEnum(Items.ZestyBar).value.item).ReflectionGetVariable("audEat");
            assetMan.Add<ItemObject>("KrabbyPatty", krabbyPatty);

            yield return "Doing some miscellaneous stuff...";

            PinkGuy.shaldiSprite = assetMan.Get<Sprite>("ShaldiSprite");
            plankton.stroketonSprite = assetMan.Get<Sprite>("StroketonSprite");
            plankton.stroketonDrJr = assetMan.Get<SoundObject>("StroketonDrJr");

            assetMan.Add<NPC>("Shaldi", PinkGuy);
            assetMan.Add<NPC>("Stroketon", plankton);

        }

        private void GetAssets()
        {
            assetMan.Add<Texture2D>("Shaldi", AssetLoader.TextureFromMod(this, npcSubDirectory, "Shaldi/Shaldi.png"));
            assetMan.Add<Texture2D>("Shaldi_Poster", AssetLoader.TextureFromMod(this, npcSubDirectory, "Shaldi/Shaldi_Poster.png"));
            assetMan.Add<Sprite>("ShaldiSprite", AssetLoader.SpriteFromTexture2D(assetMan.Get<Texture2D>("Shaldi"), 30));
            assetMan.Add<Texture2D>("Stroketon", AssetLoader.TextureFromMod(this, npcSubDirectory, "Stroketon/Stroketon.png"));
            assetMan.Add<Texture2D>("Stroketon_Poster", AssetLoader.TextureFromMod(this, npcSubDirectory, "Stroketon/Stroketon_Poster.png"));
            assetMan.Add<Sprite>("StroketonSprite", AssetLoader.SpriteFromTexture2D(assetMan.Get<Texture2D>("Stroketon"), 50));
            assetMan.Add<SoundObject>("StroketonDrJr", ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(this, npcAudioSubDirectory, "Stroketon/STRO_DrJr.wav"), "Dr. Jr.", SoundType.Music, Color.green));
            assetMan.Add<SoundObject>("KrabbyPattyCrunch", ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(this, itemAudioSubDirectory, "KrabbyPatty/Crunch.wav"), "*CRUNCH*", SoundType.Effect, Color.white));
            assetMan.Add<Texture2D>("KrabbyPattySmall", AssetLoader.TextureFromMod(this, itemSubDirectory, "KrabbyPatty/KrabbyPattyIcon_Small.png"));
            assetMan.Add<Texture2D>("KrabbyPattyLarge", AssetLoader.TextureFromMod(this, itemSubDirectory, "KrabbyPatty/KrabbyPattyIcon_Large.png"));
            assetMan.Add<Sprite>("KrabbyPattySmallSprite", AssetLoader.SpriteFromTexture2D(assetMan.Get<Texture2D>("KrabbyPattySmall"), 50));
            assetMan.Add<Sprite>("KrabbyPattyLargeSprite", AssetLoader.SpriteFromTexture2D(assetMan.Get<Texture2D>("KrabbyPattyLarge"), 50));
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
                floorObject.potentialNPCs.Add(new WeightedNPC()
                {
                    selection = assetMan.Get<NPC>("Stroketon"),
                    weight = floorNumber < 2 ? 150 * floorNumber : 475
                }
                );
                //floorObject.levelObject.potentialItems = floorObject.levelObject.potentialItems.AddItem(new WeightedItemObject() { selection = assetMan.Get<ItemObject>("KrabbyPatty"), weight = 100 }).ToArray(); - maybe commenting that out would fix it
            }
            else if (floor == "END")
            {
                floorObject.potentialNPCs.Add(new WeightedNPC()
                {
                    selection = assetMan.Get<NPC>("Shaldi"),
                    weight = 250
                }
                );
                floorObject.potentialNPCs.Add(new WeightedNPC()
                {
                    selection = assetMan.Get<NPC>("Stroketon"),
                    weight = 475
                }
                );
                //floorObject.levelObject.potentialItems = floorObject.levelObject.potentialItems.AddItem(new WeightedItemObject() { selection = assetMan.Get<ItemObject>("KrabbyPatty"), weight = 100 }).ToArray(); - ditto
            }

            //floorObject.shopItems = floorObject.shopItems.AddItem(new WeightedItemObject() { selection = assetMan.Get<ItemObject>("KrabbyPatty"), weight = 100 }).ToArray(); - the mod is gonna focus on adding npcs, for now

            floorObject.MarkAsNeverUnload();
        }
    }
}