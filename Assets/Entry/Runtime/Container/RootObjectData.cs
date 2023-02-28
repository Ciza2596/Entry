using System;
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class RootObjectData
    {
        private readonly Type[] _lifeScopeTypes;
        private readonly List<Type> _registeredTypes = new List<Type>();

        //public variable
        public object RootObject { get; }
        public Type[] LifeScopeTypes => _lifeScopeTypes.ToArray();
        public int RegisteredTypeCount => _registeredTypes.Count;
        public Type[] RegisteredTypes => _registeredTypes.ToArray();


        //constructor
        public RootObjectData(object rootObject, Type[] lifeScopeTypes)
        {
            RootObject = rootObject;
            _lifeScopeTypes = lifeScopeTypes;
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