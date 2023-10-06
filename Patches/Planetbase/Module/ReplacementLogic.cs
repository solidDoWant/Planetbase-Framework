namespace PlanetbaseFramework.Patches.Planetbase.Module
{
    /// <summary>
    /// Logic that should be scoped to the Planetbase.Module class, and is used
    /// for patches.
    /// </summary>
    public class ReplacementLogic
    {
        /// <summary>
        /// Replaces array lookups on Module.ValidSizes. This will return the same values
        /// as Module.ValidSizes[] for the first Module.ValidSizes.Length indices, and
        /// will continue scaling linearly for larger sizeIndex
        /// </summary>
        /// <param name="sizeIndex">The module size index to look up the ValidSize value for</param>
        public static float GetValidSize(int sizeIndex)
        {
            // This should not be necessary right now but should reduce the chance of breakage
            // upon future game updates
            if(sizeIndex < global::Planetbase.Module.ValidSizes.Length - 1)
                return global::Planetbase.Module.ValidSizes[sizeIndex];

            return 2.5f * sizeIndex + 7.5f;
        }
    }
}