using System;

namespace PlanetbaseFramework
{
    //Simple attribute that keeps a mod from being loaded. Useful if you are dynamically generating mods at runtime.
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ModLoaderIgnoreAttribute : Attribute
    {
    }
}