using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    class BaseModuleType : ModuleType
    {
        public BaseModuleType(string PrefabName) : base()
        {
            this.PrefabName = PrefabName;
        }

        public string PrefabName { get; set; }

        public override GameObject loadPrefab(int SizeIndex)
        {
            GameObject moduleObject = UnityEngine.Object.Instantiate<GameObject>(ResourceUtil.loadPrefab("Prefabs/Modules/" + PrefabName + (SizeIndex + 1)));
            moduleObject.calculateSmoothMeshRecursive(ModuleType.mMeshes);
            if (moduleObject.GetComponent<Collider>() != null)
            {
                Debug.LogWarning("COLLISION IN THE ROOT");
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
