using Harmony;
using UnityEngine;

namespace PlanetbaseFramework.Patches.Planetbase.ConstructionComponent
{
    [HarmonyPatch(typeof(global::Planetbase.ConstructionComponent))]
    [HarmonyPatch("selectionCast")]
    public class SelectionCastPatch
    {
        // This patch uses colliders for the selection of custom components, rather than weird non-collision raycasting logic that relies
        // on objects being added to the root game object in a specific order.
        public static bool Prefix(global::Planetbase.ConstructionComponent __instance, Ray ray, out float distance, out bool __result)
        {
            if (Utils.IsBaseGameType(__instance.mComponentType))
            {
                distance = default; // Satisfy the compiler
                __result = default; // Satisfy the compiler
                return true;    // Run the original method for construction components that are created from base game component types
            }

            __result = ReplacementMethod(__instance, ray, out distance);
            return false;
        }

        public static bool ReplacementMethod(global::Planetbase.ConstructionComponent __instance, Ray ray,
            out float distance)
        {
            foreach (var collider in __instance.mModel.GetComponentsInChildren<Collider>())
            {
                // 100f is used by the tryPlaceComponent method, so it should be a reasonable default here
                if (!collider.Raycast(ray, out var hitInfo, 100f))
                    continue;

                distance = hitInfo.distance;
                return true;
            }

            distance = default;
            return false;
        }
    }
}