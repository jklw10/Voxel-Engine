using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;



namespace Voxel_Engine
{
    public class Entity : IComponent
    {
        readonly Vessel container;
        public Guid id;
        public Entity(params IComponent[] components)
        {
            //#warning not thread safe at the moment
            container = Vessel.Selected ?? throw new ApplicationException("You need to Select a vessel to use when creating entities.");

            id = container.BindIntoEntity(components);
        }
        public IComponent? GetComponent<T>()
        {
            return container.GetComponent(typeof(T), id);
        }
        public IComponent AddComponent(IComponent component)
        {
            container.AddComponent(id, component);
            
            return component;
        }


    }

    /// <summary>
    /// entity component container and handler
    /// </summary>
    public class Vessel
    {
        public static Vessel? Selected;
        List<Dictionary<Guid, IComponent>> EntityDataList = new List<Dictionary<Guid, IComponent>>();
        
        public Guid BindIntoEntity(IComponent[] entity)
        {
            Guid id;
            id = Guid.NewGuid();
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
        public long AddComponent(Guid id,IComponent component)
        {
            
            foreach (Dictionary<Guid, IComponent> componentSection in EntityDataList)
            {
                if (componentSection.GetType().GetGenericArguments()[1] == component.GetType()) //compares dictionary value type to added component type
                {
                    //if a dictionary with that type of value is found add the component with that id to that dictionary.
                    componentSection.Add(id, component);
                    return componentSection.Count-1;
                }
            }
            //if the iteration above doesn't find a dictionary that hase the component type it must create a new dictionary
            EntityDataList.Add(new Dictionary<Guid, IComponent>());
            //then add the item to the last dictionary.
            EntityDataList[^1].Add(id, component);
            return 0;
        }
        public IEnumerable<IComponent>? GetComponents(Type type)
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
        public IComponent? GetComponent(Type type, Guid id)
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
        public bool DestroyEntity(Guid id)
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
            catch(Exception e)
            {
#if !DEBUG
                return false;
#else
                throw e;
#endif
            }
        }
        public Vessel Select()
        {
//#warning not thread safe at the moment

#if !DEBUG
            return Selected = this;
#else
            if (ObjectCount > 0 && !UseAsNonStatic) 
            {
                throw new ApplicationException("this method should only be used if a single vessel is active, if you want to ignore this rule set UseAsNonStatic to true");
            }
            else
            {
                return Selected = this;
            }
#endif
        }
        public static int ObjectCount = 0;
        public static bool UseAsNonStatic = false;
        public Vessel()
        {
            ObjectCount++;
        }
    }
    public interface IComponent
    {

    }
}
