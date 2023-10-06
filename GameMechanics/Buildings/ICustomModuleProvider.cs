using Planetbase;
using System.Xml;
using UnityEngine;

namespace PlanetbaseFramework.GameMechanics.Buildings
{
    /// <summary>
    /// ModuleTypes that implement this interface will be able to inject a custom module type
    /// into the `Create` calls for modules 
    /// </summary>
    public interface ICustomModuleProvider
    {
        Module Create(Vector3 position, int sizeIndex);
        Module Create(XmlNode node);
    }
}