using Harmony;
using UnityEngine;

namespace PlanetbaseFramework.Patches.Planetbase.Resource
{
    [HarmonyPatch(typeof(global::Planetbase.Resource))]
    [HarmonyPatch("find")]
    [HarmonyPatch(new[]{typeof(GameObject)})]
    public class FindPatch
    {
        // This patch ensures that gameobject -> resource lookups will always use the correct parent
        // This fixes an issue with selection when resource model gameobjects are nested with the collider
        // at the "wrong" level.
        public static bool Prefix(global::Planetbase.Resource __instance, ref GameObject gameObject, ref global::Planetbase.Resource __result)
        {
            var objectToSearchFor = FindResourceRootObject(gameObject);
            if (objectToSearchFor == null)
            {
                __result = null;
                return false;
            }

            gameObject = objectToSearchFor;
            return true;
        }

        // TODO move this to a more general file
        public static GameObject FindResourceRootObject(GameObject @object)
        {
            while (true)
            {
                if (@object.transform.parent == null)
                    return null;

                if (@object.transform.parent.Equals(global::Planetbase.Resource.mParentObject.transform)) 
                    return @object;

                @object = @object.transform.parent?.gameObject;
            }
        }
    }
}