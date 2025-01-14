using MDA.Builtins;

namespace MDA;

public static class NativeClassRegistry
{
    public static readonly Dictionary<string, NativeClass> Registry = new();
    
    /*
     * Add all native classes to the registry.
     */
    public static void RegisterAll()
    {
        Register(new ConsoleClass());
        Register(new MathClass());
    }
    
    /*
     * Register a native class with the given name.
     */
    public static void Register(NativeClass nativeClass)
    {
        Registry[nativeClass.Name] = nativeClass;
    }
    
    /*
     * Get a native class by name.
     */
    public static NativeClass? Get(string name)
    {
        if (Registry.ContainsKey(name))
        {
            return Registry[name];
        }
        
        return null;
    }
    
    /*
     * Get all registered native classes.
     */
    public static Dictionary<string, NativeClass> GetAll()
    {
        return Registry;
    }
}