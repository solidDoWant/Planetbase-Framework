using Mono.Cecil;

namespace PlanetbaseFramework.Cecil
{
    /// <summary>
    /// Uses the ModuleLoader class to find and resolve AssemblyDefinitions for Mono.Cecil.
    /// </summary>
    public class Resolver : BaseAssemblyResolver
    {
        protected static Resolver BackingInstance { get; set; }
        public static Resolver Instance => BackingInstance ?? (BackingInstance = new Resolver());
     
        public ReaderParameters ReaderParameters { get; }

        protected Resolver()
        {
            ReaderParameters = new ReaderParameters { AssemblyResolver = this };
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            try
            {
                return base.Resolve(name);
            }
            catch (AssemblyResolutionException)
            {
                return ModuleLoader.LoadByAssemblyName(name.FullName).Assembly;
            }
        }
    }
}