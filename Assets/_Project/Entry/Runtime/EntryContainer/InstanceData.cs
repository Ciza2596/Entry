using System;
using System.Collections.Generic;
using System.Linq;

namespace CizaEntry
{
    internal class InstanceData
    {
        private readonly Type[] _entryPoints;
        private readonly List<Type> _keys = new List<Type>();

        //public variable
        public object Instance { get; }
        public Type[] EntryPoints => _entryPoints.ToArray();
        public int KeysCount => _keys.Count;
        public Type[] Keys => _keys.ToArray();


        //constructor
        public InstanceData(object instance, Type[] entryPoints)
        {
            Instance = instance;
            _entryPoints = entryPoints;
        }



        //public method
        public void AddKey(Type key)
        {
            if (!_keys.Contains(key))
                _keys.Add(key);
        }

        public void RemoveKey(Type key)
        {
            if (_keys.Contains(key))
                _keys.Remove(key);
        }
    }
}