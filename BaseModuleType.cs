﻿using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    public class BaseModuleType : ModuleType
    {
        public BaseModuleType(Texture2D icon, GameObject[] moduleObjects)
        {
            // These settings are designed to provide safe defaults that keep the game
            // from crashing, not to provide meaningful functionality.
            mIcon = icon;
            mMinSize = 0;
            mDefaultSize = 0;
            mMaxSize = moduleObjects.Length - 1;
            initStrings();
            mCost = new ResourceAmounts();
            mModels = moduleObjects;
        }

        /// <summary>
        /// Provides a model for the module type. The following assumptions are made:
        /// * Collision geometries are added as needed
        /// * All child GameObjects are appropriately tagged
        /// * The top level object is set as inactive
        /// * The GameObject can be modified without other adverse effects
        /// * The mesh of all child objects has been "smoothed", following
        ///     calculateSmoothMeshRecursive logic
        ///
        /// It is recommended that the objects provided here are generated by the
        /// ModuleModelBuilder class.
        /// </summary>
        /// <param name="sizeIndex"></param>
        /// <returns></returns>
        public override GameObject loadPrefab(int sizeIndex)
        {
            var moduleObject = mModels[sizeIndex - mMinSize]; // Index takes into account the edge case where mMinSize != 0

            // Upon transition from GameStateGame, the GroupName GameObject and all children will be destroyed.
            var moduleTypeRootObject = GameObject.Find(GroupName) ?? new GameObject { name = GroupName };

            moduleObject.transform.SetParent(moduleTypeRootObject.transform, false);

            return moduleObject;
        }
    }
}