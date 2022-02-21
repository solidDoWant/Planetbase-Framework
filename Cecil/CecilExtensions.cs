using System.Linq;
using Mono.Cecil;

namespace PlanetbaseFramework.Cecil
{
    public static class CecilExtensions
    {
        public static bool HasIgnoreAttribute(this TypeDefinition type)
        {
            if (!type.HasCustomAttributes)
                return false;

            return type.CustomAttributes.Any(attribute =>
                attribute.AttributeType.HasTypeAsParent(typeof(ModLoaderIgnoreAttribute).FullName));
        }

        public static bool HasTypeAsParent(this TypeReference child, string parentFullName)
        {
            var typeToCheck = child;
            while (typeToCheck != null && typeToCheck.FullName != typeof(object).FullName)
            {
                if (typeToCheck.FullName == parentFullName)
                    return true;

                typeToCheck = typeToCheck.Resolve().BaseType;
            }

            return false;
        }

        // ReSharper disable once UnusedMember.Global
        public static bool IsSameTypeAs(this TypeReference typeA, TypeReference typeB)
        {
            return typeA.FullName == typeB.FullName;
        }

        // ReSharper disable once UnusedMember.Global
        public static bool IsSameTypeAs(this TypeDefinition typeA, TypeDefinition typeB)
        {
            return typeA.Module == typeB.Module && typeA.MetadataToken.RID == typeB.MetadataToken.RID;
        }
    }
}
