using System;
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class RootObjectData
    {
        private readonly Type[] _entryPointTypes;
        private readonly List<Type> _registeredTypes = new List<Type>();

        //public variable
        public object RootObject { get; }
        public Type[] EntryPointTypes => _entryPointTypes.ToArray();
        public int RegisteredTypeCount => _registeredTypes.Count;
        public Type[] RegisteredTypes => _registeredTypes.ToArray();


        //constructor
        public RootObjectData(object rootObject, Type[] entryPointTypes)
        {
            RootObject = rootObject;
            _entryPointTypes = entryPointTypes;
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