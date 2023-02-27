using System;
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class RootObjectData
    {
        private readonly Type[] _gameLoopTypes;
        private readonly List<Type> _registeredTypes = new List<Type>();

        //public variable
        public object RootObject { get; }
        public Type[] GameLoopTypes => _gameLoopTypes.ToArray();
        public int RegisteredTypeCount => _registeredTypes.Count;
        public Type[] RegisteredTypes => _registeredTypes.ToArray();


        //constructor
        public RootObjectData(object rootObject, Type[] gameLoopTypes)
        {
            RootObject = rootObject;
            _gameLoopTypes = gameLoopTypes;
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