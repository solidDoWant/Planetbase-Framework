using System.Xml;
using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework.GameMechanics.Buildings
{
    public class BaseModule : Module
    {
        public static T Create<T>(Vector3 position, int sizeIndex, ModuleType moduleType, T module) where T : BaseModule
        {
            module.init(position, sizeIndex, moduleType);
            module.postInit();
            return module;
        }

        public static T Create<T>(XmlNode node, T module) where T : BaseModule
        {
            module.deserialize(node);
            module.postInit();
            if (module.mState == BuildableState.Built)
            {
                module.onPlaced();
                module.onBuilt();
            }
            else
                module.onPlaced();
            return module;
        }
    }
}