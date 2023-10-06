using UnityEngine;

namespace PlanetbaseFramework.Patches.Planetbase.ModuleType
{
    /// <summary>
    /// Logic that should be scoped to the Planetbase.ModuleType class, and is used
    /// for patches.
    /// </summary>
    public class ReplacementLogic
    {

        /// <summary>
        /// Replaces array lookups on ModuleType.BaseCosts. This will return the same values
        /// as ModuleType.BaseCosts[] for the first ModuleType.BaseCosts.Length indices, and
        /// will continue scaling for larger sizeIndex, using the equation:
        /// `baseCost = round(0.9 * exp(0.5 * sizeIndex))`. This equation roughly models the
        /// base game values.
        ///
        /// This (and ModuleType.BaseCosts) are used for evaluating metal and bioplastic costs
        /// of a new module (in the ModuleType.calculateCost function), and a "size factor"
        /// (in the Module.getSizeFactor function) that is used to determine resource
        /// generation multipliers.
        /// </summary>
        /// <param name="sizeIndex">The module size index to look up the BaseCost value for</param>
        public static int GetBaseCost(int sizeIndex)
        {
            if (sizeIndex < global::Planetbase.ModuleType.BaseCosts.Length - 1)
                return global::Planetbase.ModuleType.BaseCosts[sizeIndex];

            return Mathf.RoundToInt(0.9f * Mathf.Exp(0.5f * sizeIndex));
        }
    }
}