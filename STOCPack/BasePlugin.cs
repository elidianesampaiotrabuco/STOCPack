using BepInEx;

using HarmonyLib;

using MTM101BaldAPI;

namespace STOCPack
{
    [BepInPlugin("starrie.bbplus.stoc", "Shaldi's Tower of Chaos Pack", "0.0.1")]

    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

    public class BasePlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony harmony = new Harmony("starrie.bbplus.stoc");

            harmony.PatchAll();
        }
    }
}