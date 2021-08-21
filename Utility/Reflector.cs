using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Voxel_Engine
{

    using Utility;
    static class Reflector
    {
        public static void Init()
        {
            SortedList<int, MethodInfo> initFunctionsToRun = new(new DuplicateKeyComparer<int>());

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                    MethodInfo[] methods = type.GetMethods(flags);
                    foreach (MethodInfo method in methods)
                    {
                        if (method is object)
                        {
                            if (method.CustomAttributes.ToArray().Length > 0)
                            {
                                InitFunctionAttribute? attribute = method.GetCustomAttribute<InitFunctionAttribute>();
                                if (attribute is object)
                                {
                                    initFunctionsToRun.Add(attribute.priority, method);
                                    
                                }
                            }
                        }
                    }
                }
            }

            List<object?> returns = new();
            foreach (MethodInfo mi in initFunctionsToRun.Values)
            {
                returns.Add(mi.Invoke(null, null));
            }
            int i = 0;
            returns.ForEach((x) =>
            {
                i++;
                if (x is Exception || x is bool)
                {
                    throw (Exception?)x ?? new ApplicationException("initialization returned false: " + initFunctionsToRun[i].DeclaringType);
                }
            });
        }
        //public static void Init()
        //{
        //    SortedList<int, MethodInfo> initFunctionsToRun = new(new DuplicateKeyComparer<int>());
        //
        //    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        //    foreach (Assembly assembly in assemblies)
        //    {
        //        Type[] types = assembly.GetTypes();
        //        foreach (Type type in types)
        //        {
        //            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        //            MethodInfo[] methods = type.GetMethods(flags);
        //            foreach (MethodInfo method in methods)
        //            {
        //                if (method is object)
        //                {
        //                    if (method.CustomAttributes.ToArray().Length > 0)
        //                    {
        //                        InitFunctionAttribute? attribute = method.GetCustomAttribute<InitFunctionAttribute>();
        //                        if (attribute is object)
        //                        {
        //                            initFunctionsToRun.Add(attribute.priority, method);
        //                            
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //
        //    List<object?> returns = new();
        //    foreach (MethodInfo mi in initFunctionsToRun.Values)
        //    {
        //        returns.Add(mi.Invoke(null, null));
        //    }
        //    int i = 0;
        //    returns.ForEach((x) =>
        //    {
        //        i++;
        //        if (x is Exception || x is bool)
        //        {
        //            throw (Exception?)x ?? new ApplicationException("initialization returned false: " + initFunctionsToRun[i].DeclaringType);
        //        }
        //    });
        //}
    }
}
namespace Voxel_Engine
{
    
    [AttributeUsage(AttributeTargets.Method)]
    sealed public class InitFunctionAttribute : Attribute
    {
        public int priority = 0;

                    
    }
}
