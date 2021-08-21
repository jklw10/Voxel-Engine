using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Voxel_Engine.DataHandling
{

    public struct Entity : IComponent
    {
        public Guid id;
        public Entity(params IComponent[] components)
        {
            id = Vessel.BindIntoEntity(components);
        }
        public IComponent? GetComponent<T>()
        {
            return Vessel.GetComponent(typeof(T), id);
        }
        public IComponent AddComponent(IComponent component)
        {
            Vessel.AddComponent(id, component);
            return component;
        }
    }

    /// <summary>
    /// entity component container and handler
    /// </summary>
    public static class Vessel
    {
        static readonly List<Dictionary<Guid, IComponent>> EntityDataList = new();
        

        //TODO? guid to index queue + generation based on destruction.
        public static Guid BindIntoEntity(IComponent[] entity)
        {
            Guid id = Guid.NewGuid();
            foreach(IComponent component in entity)
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
            foreach (Dictionary<Guid, IComponent> componentSection in EntityDataList)
            {
                if (componentSection.GetType().GetGenericArguments()[1] == component.GetType()) //compares dictionary value type to added component type
                {
                    //if a dictionary with that type of value is found add the component with that id to that dictionary.
                    componentSection[id] = component;
                    return;
                }
            }
            //if the iteration above doesn't find a dictionary that hase the component type it must create a new dictionary
            EntityDataList.Add(new Dictionary<Guid, IComponent>());
            //then add the item to the last dictionary.
            EntityDataList[^1][id] = component;
        }
        public static IEnumerable<IComponent>? GetComponents(Type type)
        {
            try
            {
                //find a dictionary with that type as value's type
                return EntityDataList.Find(x => x.GetType().GetGenericArguments()[1] == type)?.Values;
            }
            catch
            {
                return null;
            }
        }
        public static IComponent? GetComponent(Type type, Guid id)
        {
            try
            {
                //find a dictionary with that type as value's type and find the one with ID as key
                return EntityDataList.Find(x => x.GetType().GetGenericArguments()[1] == type)?[id];
            }
            catch
            {
                return null;
            }
        }
        public static bool DestroyEntity(Guid id)
        {
            try
            {
                foreach (var comps in EntityDataList)
                {
                    //removes entity's components from dictionaries
                    comps.Remove(id);
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
}
