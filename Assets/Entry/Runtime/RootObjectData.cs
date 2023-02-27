using System;
using System.Collections.Generic;

namespace Entry
{
    public class RootObjectData
    {
        private readonly List<Type> _registeredTypes = new List<Type>();

        //public variable
        public object RootObject { get; }
        public int RegisteredTypeCount => _registeredTypes.Count;
        public Type[] RegisteredTypes => _registeredTypes.ToArray();


        //constructor
        public RootObjectData(object rootObject)
        {
            RootObject = rootObject;
            AddRegisteredType(rootObject.GetType());
        }



        //public method
        public void AddRegisteredType(Type registeredType)
        {
            if (!_registeredTypes.Contains(registeredType))
                _registeredTypes.Add(registeredType);
        }

        public void RemoveRegisteredType(Type registeredType)
        {
            if (_registeredTypes.Contains(registeredType))
                _registeredTypes.Remove(registeredType);
        }
    }
}