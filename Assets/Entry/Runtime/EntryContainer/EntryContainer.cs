using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Object = System.Object;


namespace CizaEntry
{
    public class EntryContainer
    {
        //private variable
        private readonly IReadOnlyList<Type> _entryPointTypes = new List<Type>(3)
            { typeof(IFixedTickable),typeof(ITickable), typeof(ILateTickable), typeof(IReleasable) };


        private readonly Dictionary<Type, RootObjectData> _rootObjectDataMap = new Dictionary<Type, RootObjectData>();
        private readonly Dictionary<Type, Type> _rootObjectTypeMap = new Dictionary<Type, Type>();
        
        private Action<float> _fixedTickHandle;
        private Action<float> _tickHandle;
        private Action<float> _lateTickHandle;


        //Unity callback
        public void Tick(float deltaTime) =>
            _tickHandle?.Invoke(deltaTime);

        public void FixedTick(float fixedDeltaTime) =>
            _fixedTickHandle?.Invoke(fixedDeltaTime);

        public void LateTick(float deltaTime) =>
            _lateTickHandle?.Invoke(deltaTime);


        //public variable
        public Type[] RootObjectTypes => _rootObjectDataMap.Keys.ToArray();
        public Type[] RegisteredObjectTypes => _rootObjectTypeMap.Keys.ToArray();


        //public method
        public bool TryGetRootObjectType(Type registeredObjectType, out Type rootObjectType) =>
            _rootObjectTypeMap.TryGetValue(registeredObjectType, out rootObjectType);

        public bool TryGetRegisteredObjectTypes(Type rootObjectType, out Type[] registeredTypes)
        {
            registeredTypes = null;

            if (!_rootObjectDataMap.TryGetValue(rootObjectType, out var rootObjectData))
                return false;

            registeredTypes = rootObjectData.RegisteredTypes;
            return true;
        }

        public bool TryGetEntryPointTypes(Type rootObjectType, out Type[] entryPointTypes)
        {
            entryPointTypes = null;
            if (!_rootObjectDataMap.TryGetValue(rootObjectType, out var rootObjectData))
                return false;

            entryPointTypes = rootObjectData.EntryPointTypes;
            return entryPointTypes != null && entryPointTypes.Length > 0;
        }


        public void Bind<TRootObject>(TRootObject rootObject) where TRootObject : class =>
            Bind<TRootObject, TRootObject>(rootObject);


        public void Bind<TRegisteredObject, TRootObject>(TRootObject rootObject)
            where TRegisteredObject : class where TRootObject : class =>
            Bind(typeof(TRegisteredObject), rootObject);

        public void BindAndSelf<TRegisteredObject, TRootObject>(TRootObject rootObject)
            where TRegisteredObject : class where TRootObject : class
        {
            var registeredTypes = new[] { typeof(TRegisteredObject), typeof(TRootObject) };
            foreach (var canBeRegisteredType in registeredTypes)
                Bind(canBeRegisteredType, rootObject);
        }

        public void BindInheritances<TRootObject>(TRootObject rootObject) where TRootObject : class
        {
            var rootObjectType = typeof(TRootObject);
            var canBeRegisteredTypes = GetCanBeRegisteredTypes(rootObjectType);

            foreach (var canBeRegisteredType in canBeRegisteredTypes)
                Bind(canBeRegisteredType, rootObject);
        }

        public void BindInheritancesAndSelf<TRootObject>(TRootObject rootObject) where TRootObject : class
        {
            var rootObjectType = typeof(TRootObject);
            var canBeRegisteredTypes = GetCanBeRegisteredTypes(rootObjectType, true);

            foreach (var canBeRegisteredType in canBeRegisteredTypes)
                Bind(canBeRegisteredType, rootObject);
        }


        public bool TryResolve<TRegisteredObject>(out TRegisteredObject registeredObject)
            where TRegisteredObject : class
        {
            registeredObject = null;

            var registeredObjectType = typeof(TRegisteredObject);
            Assert.IsTrue(!_entryPointTypes.Contains(registeredObjectType),
                $"[EntryContainer::TryResolve] Cant use registeredObjectType: {registeredObjectType}.");

            if (!_rootObjectTypeMap.TryGetValue(registeredObjectType, out var rootObjectType))
            {
                Debug.LogWarning(
                    $"[EntryContainer::TryResolve] Not find registeredObject by registeredObjectType: {registeredObjectType}.");
                return false;
            }

            var rootObjectData = _rootObjectDataMap[rootObjectType];
            var rootObject = rootObjectData.RootObject;

            registeredObject = rootObject as TRegisteredObject;

            return true;
        }


        public void Remove<TRegisteredType>() where TRegisteredType : class =>
            Remove(typeof(TRegisteredType));

        public void Remove(Type registeredType)
        {
            if (!_rootObjectTypeMap.TryGetValue(registeredType, out var rootObjectType))
            {
                Debug.LogError($"[EntryContainer::Remove] RegisteredType: {registeredType} is already removed.");
                return;
            }

            _rootObjectTypeMap.Remove(registeredType);

            var rootObjectData = _rootObjectDataMap[rootObjectType];
            rootObjectData.RemoveRegisteredType(registeredType);

            if (rootObjectData.RegisteredTypeCount > 0)
                return;

            RemoveRootObjectByRootObjectType(rootObjectType);
        }

        public void RemoveRootObject<TRegisteredType>() where TRegisteredType : class =>
            RemoveRootObject(typeof(TRegisteredType));

        public void RemoveRootObject(Type registeredType) =>
            RemoveRootObjectByRegisteredObjectType(registeredType);

        public void RemoveAllRootObjects()
        {
            var rootObjectTypes = _rootObjectDataMap.Keys.ToArray();
            foreach (var rootObjectType in rootObjectTypes)
                RemoveRootObjectByRootObjectType(rootObjectType);
        }


        //private method
        private void Bind<TRootObject>(Type registeredType, TRootObject rootObject)
        {
            var rootObjectType = rootObject.GetType();

            Assert.IsTrue(CheckCanBeRegisteredType(registeredType, rootObjectType),
                $"[EntryContainer::Bind] Please check rootObjectType: {rootObjectType} inherits registeredType: {registeredType}.");

            if (_rootObjectTypeMap.TryGetValue(registeredType, out var currentRootObjectType))
            {
                Debug.LogWarning(
                    $"[EntryContainer::Bind] RegisteredType: {registeredType} is already registered. Current matching rootObjectType: {currentRootObjectType}");
                return;
            }


            if (!_rootObjectDataMap.ContainsKey(rootObjectType))
                CreateRootObjectData(rootObject);

            var rootObjectData = _rootObjectDataMap[rootObjectType];
            rootObjectData.AddRegisteredType(registeredType);
            _rootObjectTypeMap.Add(registeredType, rootObjectType);
        }

        private bool CheckCanBeRegisteredType(Type registeredType, Type rootObjectType)
        {
            var canBeRegisteredTypes = GetCanBeRegisteredTypes(rootObjectType, true);
            return canBeRegisteredTypes.Contains(registeredType);
        }

        private Type[] GetCanBeRegisteredTypes(Type rootObjectType, bool isIncludeSelf = false)
        {
            var rootObjectInterfaces = rootObjectType.GetInterfaces().ToList();

            foreach (var rootObjectInterface in rootObjectInterfaces.ToArray())
                if (_entryPointTypes.Contains(rootObjectInterface))
                    rootObjectInterfaces.Remove(rootObjectInterface);

            var rootObjectBaseType = rootObjectType.BaseType;
            if (rootObjectBaseType != typeof(Object))
                rootObjectInterfaces.Add(rootObjectBaseType);

            if (isIncludeSelf)
                rootObjectInterfaces.Add(rootObjectType);

            return rootObjectInterfaces.ToArray();
        }

        private Type[] GetEntryPointTypes(Type rootObjectType)
        {
            var rootObjectInterfaces = rootObjectType.GetInterfaces().ToList();
            var entryPointTypes = new List<Type>();

            foreach (var rootObjectInterface in rootObjectInterfaces.ToArray())
                if (_entryPointTypes.Contains(rootObjectInterface))
                    entryPointTypes.Add(rootObjectInterface);

            return entryPointTypes.ToArray();
        }

        private void CreateRootObjectData<TRootObject>(TRootObject rootObject)
        {
            var rootObjectType = rootObject.GetType();
            var entryPointTypes = GetEntryPointTypes(rootObjectType);
            var rootObjectData = new RootObjectData(rootObject, entryPointTypes);
            _rootObjectDataMap.Add(rootObjectType, rootObjectData);

            AddFixedTickAndTickAndLateTickHandle(rootObject);
        }

        private void AddFixedTickAndTickAndLateTickHandle(object rootObject)
        {
            if (rootObject is ITickable updatable)
                _tickHandle += updatable.Tick;

            if (rootObject is IFixedTickable fixedUpdatable)
                _fixedTickHandle += fixedUpdatable.FixedTick;

            if (rootObject is ILateTickable lateUpdatable)
                _lateTickHandle += lateUpdatable.LateTick;
        }

        private void RemoveFixedTickAndTickAndLateTickHandle(object rootObject)
        {
            if (rootObject is ITickable updatable)
                _tickHandle -= updatable.Tick;

            if (rootObject is IFixedTickable fixedUpdatable)
                _fixedTickHandle -= fixedUpdatable.FixedTick;

            if (rootObject is ILateTickable lateUpdatable)
                _lateTickHandle -= lateUpdatable.LateTick;
        }

        private void RemoveRootObjectByRootObjectType(Type rootObjectType)
        {
            if (!_rootObjectDataMap.TryGetValue(rootObjectType, out var rootObjectData))
            {
                Debug.LogWarning(
                    $"[EntryContainer::RemoveRootObjectByRootObjectType] RootObjectType: {rootObjectType} is already removed.");
                return;
            }

            var rootObject = rootObjectData.RootObject;
            RemoveFixedTickAndTickAndLateTickHandle(rootObject);

            if (rootObject is IReleasable releasable)
                releasable.Release();

            var registeredTypes = rootObjectData.RegisteredTypes;
            foreach (var registeredType in registeredTypes)
            {
                _rootObjectTypeMap.Remove(registeredType);
                rootObjectData.RemoveRegisteredType(registeredType);
            }

            _rootObjectDataMap.Remove(rootObjectType);
        }

        private void RemoveRootObjectByRegisteredObjectType(Type registeredObjectType)
        {
            var hasValue = _rootObjectTypeMap.TryGetValue(registeredObjectType, out var rootObjectType);
            Assert.IsTrue(hasValue,
                $"[EntryContainer::RemoveRootObjectByRegisteredObjectType] RegisteredObjectType: {registeredObjectType} is already removed.");

            RemoveRootObjectByRootObjectType(rootObjectType);
        }
    }
}