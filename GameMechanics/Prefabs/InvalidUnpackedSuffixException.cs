using System;

namespace PlanetbaseFramework.GameMechanics.Prefabs
{
    public class InvalidUnpackedSuffixException : Exception
    {
        public string ResourceName { get; }
        public string  Suffix { get; }

        public InvalidUnpackedSuffixException(string resourceName, string suffix) : base($"the resource type \"{resourceName}\" does not support suffix \"{suffix}\"")
        {
            ResourceName = resourceName;
            Suffix = suffix;
        }
    }
}