using HarmonyLib;

namespace IllegalLoudoutPrevention
{
    public static class Patches
    {

        [HarmonyPatch(typeof(AbilityGridEntry), nameof(AbilityGridEntry.Init))]
        public class AlwaysLockAbilities
        {
            [HarmonyPrefix]
            public static void Prefix(int index, ref bool isLocked, AbilityGrid grid)
            {
                if (index == grid.abilityIcons.IndexOf("Random") - 1 || index == grid.abilityIcons.IndexOf("Revival") - 1)
                {
                    isLocked = true;
                }
            }
        }

        [HarmonyPatch(typeof(CharacterSelectBox), nameof(CharacterSelectBox.CloseAbilityGrid))]
        public class PreventChoosingSpecificAbilities
        {

            [HarmonyPrefix]
            public static bool Prefix(int abilityChoice, CharacterSelectBox __instance)
            {

                if (abilityChoice == __instance.abilityGrid.abilityIcons.IndexOf("Random") || abilityChoice == __instance.abilityGrid.abilityIcons.IndexOf("Revival"))
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(SelectAbility), nameof(SelectAbility.Select))]
        public class FixSelect
        {
            [HarmonyPrefix]
            public static bool Prefix(ref int index, SelectAbility __instance)
            {
                if (index < 0)
                {
                    index = __instance.selectedIndex;
                }
                else if (index == 0)
                {
                    index = (__instance.selectedIndex == 1) ? (__instance.abilityIcons.sprites.Count - 1) : 1;
                }
                if (index == 15)
                {
                	index = (__instance.selectedIndex == 14) ? 16 : 14;
                }
                if (index == 1)
                {
                	index = (__instance.selectedIndex == 2) ? (__instance.abilityIcons.sprites.Count - 1) : 2;
                }

                return true;
            }
        }

        public static class MidGameAbilitySwitching
        {
            static bool SwitchingRight = false;

            [HarmonyPatch(typeof(MidGameAbilitySelect), nameof(MidGameAbilitySelect.Left))]
            public class MidGameLeft
            {
                [HarmonyPrefix]
                public static void Prefix()
                {
                    SwitchingRight = false;
                }
            }

            [HarmonyPatch(typeof(MidGameAbilitySelect), nameof(MidGameAbilitySelect.Right))]
            public class MidGameRight
            {
                [HarmonyPrefix]
                public static void Prefix()
                {
                    SwitchingRight = true;
                }
            }

            [HarmonyPatch(typeof(MidGameAbilitySelect), nameof(MidGameAbilitySelect.UpdatePlayerAbilityChoices))]
            public class UpdatePlayerAbilities
            {
                [HarmonyPrefix]
                public static void Prefix(ref int[] ___XPos, ref int ___YPos, ref NamedSpriteList ___localAbilityIcons)
                {
                    if (SwitchingRight)
                    {
                        if (___XPos[___YPos] == 0)
                        {
                            ___XPos[___YPos] = ___localAbilityIcons.sprites.Count - 1;
                        }
                        if (___XPos[___YPos] == 1)
                        {
                        	___XPos[___YPos] = ___localAbilityIcons.sprites.Count - 1;
                        }
                        if (___XPos[___YPos] == 15)
                        {
                        	___XPos[___YPos] = 14;
                        }
                    }
                    else
                    {
                        if (___XPos[___YPos] == 0)
                        {
                            ___XPos[___YPos] = 1;
                        }
                        if (___XPos[___YPos] == 1)
                        {
                            ___XPos[___YPos] = 2;
                        }
                        if (___XPos[___YPos] == 15)
                        {
                            ___XPos[___YPos] = 16;
                        }
                    }
                }
            }
        }
        


        [HarmonyPatch(typeof(DynamicAbilityPickup), nameof(DynamicAbilityPickup.Init))]
        public class KillAbilities
        {
            [HarmonyPrefix]
            public static bool Prefix(DynamicAbilityPickup __instance)
            {
                __instance.gameObject.SetActive(false);

                return false;
            }
        }
    }
}
