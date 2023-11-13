using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Voxel_Engine.DataHandling;

public struct Entity
{
    public Guid id;
    public Entity(params IComponent[] components)
    {
        id = EntityContainer.CreateEntity(components);
    }
    public bool TryGetComponent<T>(out IComponent? value)
    {
        EntityContainer.TryGetComponent(typeof(T), id, out value);
        return value is not null;
    }
    public IComponent AddComponent(IComponent component)
    {
        EntityContainer.AddComponent(id, component);
        return component;
    }
}

/// <summary>
/// entity component container and handler
/// </summary>
public static class EntityContainer
{
    static readonly Dictionary<Type, Dictionary<Guid, IComponent>> EntityDataList = new();



    //TODO? guid to index queue + generation based on destruction.
    public static Guid CreateEntity(IComponent[] entity)
    {
        Guid id = Guid.NewGuid();
        foreach (IComponent component in entity)
        {
            AddComponent(id, component);
        }
        return id;
    }
    /// <summary>
    /// adds component to the vessel and returns what location it was added to.
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public static void AddComponent(Guid id, IComponent component)
    {
        var type = component.GetType();
        if (EntityDataList.TryGetValue(type, out var componentSection))
        {
            //if a dictionary with that type of value is found add the component with that id to that dictionary.
            componentSection[id] = component;
            return;
        }
        //if the iteration above doesn't find a dictionary that hase the component type it must create a new dictionary
        EntityDataList.Add(component.GetType(), new Dictionary<Guid, IComponent>());
        //then add the item to the last dictionary.
        EntityDataList[type][id] = component;
    }
    public static bool TryGetComponents(Type type, out IEnumerable<IComponent>? value)
    {   //find a dictionary with that type as value's type
        EntityDataList.TryGetValue(type, out var section);
        value = section?.Values;
        return value is not null;
    }
    public static bool TryGetComponent(Type type, Guid id, out IComponent? value)
    {
        value = null;
        //find a single entity's component
        EntityDataList.TryGetValue(type, out var section);
        section?.TryGetValue(id, out value);
        return value is not null;
    }
    public static bool DestroyEntity(Guid id)
    {
        try
        {
            foreach (var comps in EntityDataList)
            {
                //removes entity's components from dictionaries
                comps.Value.Remove(id);
            }
            return true;
        }
        catch
        {
            Console.WriteLine("Entity destruction failed severely");
#if !DEBUG
                return false;
#else
            throw;
#endif
        }
    }
}
public interface IComponent
{

}

