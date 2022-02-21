using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace PlanetbaseFramework.Cecil
{
    public class ModuleLoader
    {
        public static LinkedList<string> DllSearchFolders { get; } = new LinkedList<string>();
        
        static ModuleLoader()
        {
            DllSearchFolders.AddLast(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            DllSearchFolders.AddLast(ModBase.BasePath);
        }

        /// <summary>
        /// Loads a DLL by path, resolving dependencies if needed.
        /// </summary>
        /// <param name="filePath">The path to the DLL to load</param>
        /// <returns>The loaded module definition.</returns>
        public static ModuleDefinition LoadByPath(string filePath) =>
            ModuleDefinition.ReadModule(filePath, Resolver.Instance.ReaderParameters);

        /// <summary>
        /// Loads the DLL by searching through the DllSearchFolders and checking the name of each
        /// DLL inside. If the path to the assembly is known, LoadByPath should be used instead.
        /// </summary>
        /// <param name="fullAssemblyName">The fully qualified name of the assembly to search for</param>
        /// <returns>The loaded module definition if found, null otherwise.</returns>
        public static ModuleDefinition LoadByAssemblyName(string fullAssemblyName) =>
            GetFilesToSearch().Select(LoadByPath)
                .FirstOrDefault(dllModule => dllModule.Assembly.FullName == fullAssemblyName);

        /// <summary>
        /// Provides a list of DLLs to search through when looking for modules.
        /// </summary>
        /// <returns>A list of DLLs to search through.</returns>
        protected static IEnumerable<string> GetFilesToSearch() =>
            DllSearchFolders.SelectMany(searchFolder => Directory.GetFiles(searchFolder, "*.dll"));
    }
}