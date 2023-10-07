using Harmony;
using Planetbase;
using PlanetbaseFramework.GameMechanics.Buildings;
using PlanetbaseFramework.Patches.Planetbase.Module;
using UnityEngine;

namespace PlanetbaseFramework.Patches.Planetbase.GameStateGame
{
    [HarmonyPatch(typeof(global::Planetbase.GameStateGame))]
    [HarmonyPatch("tryPlaceModule")]
    public class TryPlaceModulePatch
    {

        // __instance patch does the following:
        // * Makes the code much easier to read
        // * Adds support for modules  with more than five sizes
        // * Improves performance a bit
        public static bool Prefix(global::Planetbase.GameStateGame __instance)
        {
            TryPlaceModuleReplacement(__instance);

            // Skip the original function call
            return false;
        }

        protected static void TryPlaceModuleReplacement(global::Planetbase.GameStateGame instance)
        {
            var mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(mousePositionRay, out var mouseHitInfo, 150f, Constants.LayerMaskTerrain))
                // Return if the cursor isn't over a GameObject with a collider
                return;

            if (instance.mActiveModule == null)
            {
                global::Planetbase.Module module;
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (instance.mPlacedModuleType is ICustomModuleProvider customModuleProvider)
                    module = customModuleProvider.Create(mouseHitInfo.point, instance.mCurrentModuleSize);
                else
                    module = global::Planetbase.Module.create(mouseHitInfo.point, instance.mCurrentModuleSize, instance.mPlacedModuleType);

                module.setRenderTop(instance.mRenderTops);
                module.setValidPosition(false);

                instance.mActiveModule = module;
                instance.mCost = module.calculateCost();
            } else if (instance.mCurrentModuleSize != instance.mActiveModule.getSizeIndex())
            {
                // This logic handles changing the module size (via scrolling)
                instance.mActiveModule.changeSize(instance.mCurrentModuleSize);
                instance.mCost = instance.mActiveModule.calculateCost();
                instance.mActiveModule.setValidPosition(false);
            }

            // This call has b een updated to use the `GetValidSize` function instead of a `Module.ValidSizes` array lookup, which allows for modules larger than
            // `Module.ValidSizes.Length`
            var canPlaceModule = instance.mActiveModule.canPlaceModule(mouseHitInfo.point, mouseHitInfo.normal, ReplacementLogic.GetValidSize(instance.mCurrentModuleSize));
            if (instance.inTutorial())
                instance.snapToTutorialPosition(ref mouseHitInfo, ref canPlaceModule);

            var mouseTerrainPoint = mouseHitInfo.point;
            var terrainFloorHeight = Singleton<TerrainGenerator>.getInstance().getFloorHeight();
            mouseTerrainPoint.y = terrainFloorHeight;

            if (canPlaceModule || !instance.mActiveModule.isValidPosition() || (mouseTerrainPoint - instance.mActiveModule.getPosition()).magnitude > 5.0)
            {
                instance.mActiveModule.setValidPosition(canPlaceModule);
                instance.mActiveModule.setPosition(mouseTerrainPoint);
            }
            instance.mActiveModule.setPositionY(terrainFloorHeight + 0.1f);
        }
    }
}