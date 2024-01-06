using HarmonyLib;

namespace StatRecorder
{
    public class Patches
    {
        [HarmonyPatch(typeof(MenuAbilitySelector), nameof(MenuAbilitySelector.Update))]
        public class MenuAbilitySelector_Update
        {
            [HarmonyPostfix]
            public static void Test(MenuAbilitySelector __instance, ref int ___playerId)
            {
                StatRecorder.Log.LogInfo("test");
                Player player = PlayerHandler.Get().GetPlayer(___playerId);
                StatRecorder.Log.LogInfo(player.Color);
            }

        }
    }
}
