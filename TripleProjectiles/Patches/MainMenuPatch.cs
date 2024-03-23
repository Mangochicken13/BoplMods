using HarmonyLib;
using Steamworks;

namespace TripleProjectiles.Patches
{
    [HarmonyPatch(typeof(MainMenu))]
    public class MainMenuPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MainMenu.Start))]
        static void Postfix()
        {
            if (!TripleProjectiles.HasCheckedDemo)
            {
                TripleProjectiles.IsFullGame = SteamClient.AppId == 1686940U;
                TripleProjectiles.HasCheckedDemo = true;

                if (!TripleProjectiles.IsFullGame)
                    TripleProjectiles.Log.LogWarning($"\n\nThis mod {TripleProjectiles.ModName} does not work for the demo of Bopl!\nPlease either purchase the full game or launch steam before launching the game");
            }
        }
    }
}
