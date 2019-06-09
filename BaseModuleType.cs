using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    public class BaseModuleType : ModuleType
    {
        public GameObject[] ModuleObjects { get; protected set; }
        public string ModuleName { get; protected set; }

        public BaseModuleType(Texture2D icon, GameObject[] moduleObjects)
        {
            //These settings are designed to keep the game from crashing, not to provide any functionality.
            ModuleObjects = moduleObjects;
            mIcon = icon;
            mMinSize = 0;
            mMaxSize = 0;
            mDefaultSize = 0;
            initStrings();
            mCost = new ResourceAmounts();
        }

        public override GameObject loadPrefab(int sizeIndex)
        {
            int adjustedSizeIndex = sizeIndex - mMinSize;   //Takes into account the edge case where mMinSize != 0

            ModuleObjects[adjustedSizeIndex].calculateSmoothMeshRecursive(mMeshes);

            //Add collider to object for raycasting
            foreach (Transform transform in ModuleObjects[adjustedSizeIndex].transform)
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
            GameObject moduleObject2 = GameObject.Find(GroupName) ?? new GameObject { name = GroupName };

            ModuleObjects[adjustedSizeIndex].transform.SetParent(moduleObject2.transform, false);

            ModuleObjects[adjustedSizeIndex].SetActive(false);

            return ModuleObjects[adjustedSizeIndex];
        }
    }
}