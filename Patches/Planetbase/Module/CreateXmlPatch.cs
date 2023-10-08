using System.Xml;
using Harmony;
using Planetbase;
using PlanetbaseFramework.GameMechanics.Buildings;

namespace PlanetbaseFramework.Patches.Planetbase.Module
{
    [HarmonyPatch(typeof(global::Planetbase.Module))]
    [HarmonyPatch("create")]
    [HarmonyPatch(new []{typeof(XmlNode)})]
    public class CreateXmlPatch
    {
        // This patch allows for the injection of custom Module instances, if the provided moduleType supports it.
        public static bool Prefix(XmlNode node)
        {
            // TODO consider changing the base game logic to use the fully qualified type, rather than just the name
            var moduleTypeNode = node["module-type"];
            // Run base game logic when the required node is not found
            if (moduleTypeNode == null)
                return true;

            var nodeModuleType = ModuleTypeList.find(Serialization.deserializeString(moduleTypeNode));
            // Run base game logic when the module type cannot be found
            if (nodeModuleType == null)
                return true;

            // Don't run custom logic for base game moduletypes
            if (Utils.IsBaseGameType(nodeModuleType))
                return true;

            // Don't run custom logic when the custom module type does not support it
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!(nodeModuleType is ICustomModuleProvider customModuleProvider)) 
                return true;

            customModuleProvider.Create(node);

            // Don't run base game logic
            return false;
        }
    }
}