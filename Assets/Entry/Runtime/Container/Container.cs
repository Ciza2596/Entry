using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


namespace Entry
{
    public class Container
    {
        //private variable
        private readonly IReadOnlyList<Type> _lifeScopeTypes = new List<Type>(3)
            { typeof(IUpdatable), typeof(IFixedUpdatable), typeof(IReleasable) };


        private readonly Dictionary<Type, RootObjectData> _rootObjectDataMap = new Dictionary<Type, RootObjectData>();
        private readonly Dictionary<Type, Type> _rootObjectTypeMap = new Dictionary<Type, Type>();

        private Action<float> _updateHandle;
        private Action<float> _fixedUpdateHandle;


        //Unity callback
        public void Update(float deltaTime) =>
            _updateHandle?.Invoke(deltaTime);
        
        public void FixedUpdate(float fixedDeltaTime) =>
            _fixedUpdateHandle?.Invoke(fixedDeltaTime);

        

        //public method
        public Type[] GetRootObjectTypes() => _rootObjectDataMap.Keys.ToArray();
        public Type[] GetRegisteredObjectTypes() => _rootObjectTypeMap.Keys.ToArray();

        public Type[] GetRegisteredObjectTypes(Type rootObjectType)
        {
            var rootObjectData = _rootObjectDataMap[rootObjectType];
            return rootObjectData.RegisteredTypes;
        }

        public bool TryGetLifeScopeTypes(Type rootObjectType, out Type[] lifeScopeTypes)
        {
            var rootObjectData = _rootObjectDataMap[rootObjectType];
            lifeScopeTypes = rootObjectData.LifeScopeTypes;
            return lifeScopeTypes != null && lifeScopeTypes.Length > 0;
        }

        public void Bind<TRootObject>(TRootObject rootObject) where TRootObject : class =>
            Bind<TRootObject, TRootObject>(rootObject);

        
        public void Bind<TRegisteredObject, TRootObject>(TRootObject rootObject)
            where TRegisteredObject : class where TRootObject : class =>
            Bind(typeof(TRegisteredObject), rootObject);

        public void BindAndSelf<TRegisteredObject, TRootObject>(TRootObject rootObject)
            where TRegisteredObject : class where TRootObject : class =>
            Bind(typeof(TRegisteredObject), rootObject, true);

        
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
            var canBeRegisteredTypes = GetCanBeRegisteredTypes(rootObjectType);

            foreach (var canBeRegisteredType in canBeRegisteredTypes)
                Bind(canBeRegisteredType, rootObject, true);
        }


        public bool TryResolve<TRegisteredObject>(out TRegisteredObject registeredObject)
            where TRegisteredObject : class
        {
            registeredObject = null;

            var registeredObjectType = typeof(TRegisteredObject);
            Assert.IsTrue(!_lifeScopeTypes.Contains(registeredObjectType),
                $"[Container::TryResolve] Cant use registeredObjectType: {registeredObjectType}.");

            if (!_rootObjectTypeMap.TryGetValue(registeredObjectType, out var rootObjectType))
            {
                Debug.LogWarning(
                    $"[Container::TryResolve] Not find registeredObject by registeredObjectType: {registeredObjectType}.");
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
                Debug.LogError($"[Container::Remove] RegisteredType: {registeredType} is already removed.");
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
        private void Bind<TRootObject>(Type registeredType, TRootObject rootObject, bool isBindSelf = false)
        {
            var rootObjectType = rootObject.GetType();

            Assert.IsTrue(CheckCanBeRegisteredType(registeredType, rootObjectType),
                $"[Container::Bind] Please check rootObjectType: {rootObjectType} inherits registeredType: {registeredType}.");

            if (_rootObjectTypeMap.TryGetValue(registeredType, out var currentRootObjectType))
            {
                Debug.LogWarning(
                    $"[Container::Bind] RegisteredType: {registeredType} is already registered. Current matching rootObjectType: {currentRootObjectType}");
                return;
            }


            if (!_rootObjectDataMap.ContainsKey(rootObjectType))
                CreateRootObjectData(rootObject);

            var rootObjectData = _rootObjectDataMap[rootObjectType];
            rootObjectData.AddRegisteredType(registeredType);
            _rootObjectTypeMap.Add(registeredType, rootObjectType);

            if (isBindSelf && !_rootObjectTypeMap.ContainsKey(rootObjectType))
            {
                rootObjectData.AddRegisteredType(rootObjectType);
                _rootObjectTypeMap.Add(rootObjectType, rootObjectType);
            }
        }

        private bool CheckCanBeRegisteredType(Type registeredType, Type rootObjectType)
        {
            var canBeRegisteredTypes = GetCanBeRegisteredTypes(rootObjectType);
            return canBeRegisteredTypes.Contains(registeredType);
        }
        
        private Type[] GetCanBeRegisteredTypes(Type rootObjectType)
        {
            var rootObjectInterfaces = rootObjectType.GetInterfaces().ToList();

            foreach (var rootObjectInterface in rootObjectInterfaces.ToArray())
                if (_lifeScopeTypes.Contains(rootObjectInterface))
                    rootObjectInterfaces.Remove(rootObjectInterface);

            rootObjectInterfaces.Add(rootObjectType);

            var rootObjectBaseType = rootObjectType.BaseType;
            rootObjectInterfaces.Add(rootObjectBaseType);

            return rootObjectInterfaces.ToArray();
        }

        private Type[] GetLifeScopeTypes(Type rootObjectType)
        {
            var rootObjectInterfaces = rootObjectType.GetInterfaces().ToList();
            var lifeScopeTypes = new List<Type>();

            foreach (var rootObjectInterface in rootObjectInterfaces.ToArray())
                if (_lifeScopeTypes.Contains(rootObjectInterface))
                    lifeScopeTypes.Add(rootObjectInterface);

            return lifeScopeTypes.ToArray();
        }

        private void CreateRootObjectData<TRootObject>(TRootObject rootObject)
        {
            var rootObjectType = rootObject.GetType();
            var lifeScopeTypes = GetLifeScopeTypes(rootObjectType);
            var rootObjectData = new RootObjectData(rootObject, lifeScopeTypes);
            _rootObjectDataMap.Add(rootObjectType, rootObjectData);

            AddUpdateAndFixedUpdate(rootObject);
        }

        private void AddUpdateAndFixedUpdate(object rootObject)
        {
            if (rootObject is IUpdatable updatable)
                _updateHandle += updatable.Update;

            if (rootObject is IFixedUpdatable fixedUpdatable)
                _fixedUpdateHandle += fixedUpdatable.FixedUpdate;
        }

        private void RemoveUpdateAndFixedUpdate(object rootObject)
        {
            if (rootObject is IUpdatable updatable)
                _updateHandle -= updatable.Update;

            if (rootObject is IFixedUpdatable fixedUpdatable)
                _fixedUpdateHandle -= fixedUpdatable.FixedUpdate;
        }
        
        private void RemoveRootObjectByRootObjectType(Type rootObjectType)
        {
            if (!_rootObjectDataMap.TryGetValue(rootObjectType, out var rootObjectData))
            {
                Debug.LogWarning(
                    $"[Container::RemoveRootObjectByRootObjectType] RootObjectType: {rootObjectType} is already removed.");
                return;
            }

            var rootObject = rootObjectData.RootObject;
            RemoveUpdateAndFixedUpdate(rootObject);

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
            Assert.IsTrue(_rootObjectTypeMap.TryGetValue(registeredObjectType, out var rootObjectType),
                $"[Container::RemoveRootObjectByRegisteredObjectType] RegisteredObjectType: {registeredObjectType} is already removed.");

            RemoveRootObjectByRootObjectType(rootObjectType);
        }
    }
}