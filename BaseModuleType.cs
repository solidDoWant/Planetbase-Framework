using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    public class BaseModuleType : ModuleType
    {
        public GameObject moduleObject { get; protected set; }
        public BaseModuleType(Texture2D icon, GameObject moduleObject)
        {
            this.moduleObject = moduleObject;
            this.mIcon = icon;
            this.mPowerGeneration = -1000;
            this.mExterior = false;
            this.mMinSize = 2;
            this.mMaxSize = 2;
            this.mHeight = 1f;
            this.mRequiredStructure.set<ModuleTypeOxygenGenerator>();
            //this.mExteriorNavRadius = 3f;
            this.initStrings();
            this.mCost = new ResourceAmounts();
            this.mFlags = FlagDome + FlagLightAtNight + FlagWalkable;
            this.mLayoutType = ModuleType.LayoutType.Normal;
        }

        public override GameObject loadPrefab(int sizeIndex)
        {
            
            moduleObject.calculateSmoothMeshRecursive(ModuleType.mMeshes);

            //Only child objects should have colliders, not the root object
            if (moduleObject.GetComponent<Collider>() != null)
            {
                Debug.LogWarning("COLLISION IN THE ROOT");
            }

            //Add collider to object for raycasting
            foreach (Transform transform in moduleObject.transform)
            {
                if (transform.gameObject.GetComponent<MeshFilter>() != null)
                {
                    transform.gameObject.AddComponent<MeshCollider>().sharedMesh = transform.gameObject.GetComponent<MeshFilter>().sharedMesh;
                }

                if (transform.gameObject.name.isValidTag())
                {
                    transform.gameObject.tag = transform.gameObject.name;
                }
                else
                {
                    Debug.Log(transform.gameObject.name + " is not a valid tag");
                }
            }

            GameObject moduleObject2 = GameObject.Find(ModuleType.GroupName);
            if (moduleObject2 == null)
            {
                moduleObject2 = new GameObject();
                moduleObject2.name = ModuleType.GroupName;
            }
            moduleObject.transform.SetParent(moduleObject2.transform, false);
            moduleObject.SetActive(false);
            return moduleObject;
        }
    }
}
