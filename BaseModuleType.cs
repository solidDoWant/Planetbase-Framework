using System.Linq;
using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    public class BaseModuleType : ModuleType
    {
        public GameObject[] ModuleObjects { get; protected set; }
        // This contains a copy of the GameObject once it has been loaded the first time. This prevents issues where the original GameObject has been reused or deleted in multiple places.
        public GameObject[] ModuleObjectCache { get; protected set; }
        public string ModuleName { get; protected set; }

        public BaseModuleType(Texture2D icon, GameObject[] moduleObjects)
        {
            //These settings are designed to keep the game from crashing, not to provide any functionality.
            ModuleObjects = moduleObjects.Select(Object.Instantiate).ToArray(); // Copy to prevent the original object from being deleted when switching out of GameStateGame
            ModuleObjectCache = new GameObject[moduleObjects.Length];
            mIcon = icon;
            mMinSize = 0;
            mMaxSize = 0;
            mDefaultSize = 0;
            initStrings();
            mCost = new ResourceAmounts();
        }

        // The game objects in the module cache will be null after a game is reloaded.
        public virtual GameObject GetCachedGameObject(int index)
        {
            if (ModuleObjectCache[index] == null)
                ModuleObjectCache[index] = Object.Instantiate(ModuleObjects[index]);

            return ModuleObjectCache[index];
        }

        public override GameObject loadPrefab(int sizeIndex)
        {
            var adjustedSizeIndex = sizeIndex - mMinSize;   //Takes into account the edge case where mMinSize != 0
            var moduleObject = GetCachedGameObject(adjustedSizeIndex);

            moduleObject.calculateSmoothMeshRecursive(mMeshes);

            //Add collider to object for raycasting
            foreach (Transform transform in moduleObject.transform)
            {
                //Make the collider for the GameObject the same size/shape as the mesh
                if (transform.gameObject.GetComponent<MeshFilter>() != null)
                {
                    transform.gameObject.AddComponent<MeshCollider>().sharedMesh = transform.gameObject.GetComponent<MeshFilter>().sharedMesh;
                }

                //This is copied directly from PB code. Not really sure that it's needed, but I'll leave it in place in case somebody else is comparing this to
                //PB code
                if (transform.gameObject.name.IsValidTag())
                {
                    transform.gameObject.tag = transform.gameObject.name;
                }
            }

            //This is more or less copied from PB code. Not entirely sure the effect of GroupNames on PB. TODO review this
            // Upon transition from GameStateGame, the GroupName GameObject and all children will be destroyed.
            GameObject moduleObject2 = GameObject.Find(GroupName) ?? new GameObject { name = GroupName };

            moduleObject.transform.SetParent(moduleObject2.transform, false);

            moduleObject.SetActive(false);
            moduleObject.setVisibleRecursive(true);

            return moduleObject;
        }
    }
}