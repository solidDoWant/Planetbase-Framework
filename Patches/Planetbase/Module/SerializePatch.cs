using System.Xml;
using Harmony;

namespace PlanetbaseFramework.Patches.Planetbase.Module
{
    [HarmonyPatch(typeof(global::Planetbase.Module))]
    [HarmonyPatch("serialize")]
    public class SerializePatch
    {
        public const string TypeAttributeName = "type";

        // Update the "type" attribute of "construction" tags to
        // always be "Module" when serializing a module. This
        // will otherwise cause deserialization issues.
        public static void Postfix(XmlNode parent)
        {
            var typeAttribute = parent.LastChild?.Attributes?[TypeAttributeName];
            if (typeAttribute == null) 
                return;

            typeAttribute.Value = nameof(global::Planetbase.Module);
        }
    }
}