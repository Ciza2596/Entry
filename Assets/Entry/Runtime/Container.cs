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
        private readonly IReadOnlyList<Type> _cantBeRegisteredTypes = new List<Type>(3)
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
        //new instance by container
        public void Bind<TRootObject>(params object[] parameters) where TRootObject : class =>
            Bind<TRootObject, TRootObject>(parameters);

        public void Bind<TRegisteredObject, TRootObject>(params object[] parameters)
            where TRegisteredObject : class where TRootObject : class
        {
            var rootObject = Activator.CreateInstance(typeof(TRootObject), parameters);
            BindFromInstance(typeof(TRegisteredObject), rootObject);
        }

        public void BindInheritances<TRootObject>(params object[] parameters) where TRootObject : class
        {
            var rootObjectType = typeof(TRootObject);
            var rootObject = Activator.CreateInstance(rootObjectType, parameters);
            var canBeRegisteredTypes = GetCanBeRegisteredTypes(rootObjectType);

            foreach (var canBeRegisteredType in canBeRegisteredTypes)
                BindFromInstance(canBeRegisteredType, rootObject);
        }


        //instance form outside
        public void BindFromInstance<TRootObject>(TRootObject rootObject) where TRootObject : class =>
            Bind<TRootObject, TRootObject>(rootObject);

        public void BindFromInstance<TRegisteredObject, TRootObject>(TRootObject rootObject)
            where TRegisteredObject : class where TRootObject : class =>
            BindFromInstance(typeof(TRegisteredObject), rootObject);

        public void BindInheritancesFromInstance<TRootObject>(TRootObject rootObject) where TRootObject : class
        {
            var rootObjectType = typeof(TRootObject);
            var canBeRegisteredTypes = GetCanBeRegisteredTypes(rootObjectType);

            foreach (var canBeRegisteredType in canBeRegisteredTypes)
                BindFromInstance(canBeRegisteredType, rootObject);
        }


        public bool TryResolve<TRegisteredObject>(out TRegisteredObject registeredObject)
            where TRegisteredObject : class
        {
            registeredObject = null;

            var registeredObjectType = typeof(TRegisteredObject);
            Assert.IsTrue(!_cantBeRegisteredTypes.Contains(registeredObjectType), $"[Container::TryResolve] Cant use registeredObjectType: {registeredObjectType}.");

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


        public void Remove<TRegisteredObject>() where TRegisteredObject : class =>
            Remove(typeof(TRegisteredObject));


        public void RemoveRootObject<TRootObject>() where TRootObject : class =>
            RemoveRootObject(typeof(TRootObject));


        public void RemoveAllRootObjects()
        {
            var rootObjectTypes = _rootObjectDataMap.Keys.ToArray();
            foreach (var rootObjectType in rootObjectTypes)
                RemoveRootObject(rootObjectType);
        }


        //private method
        private void BindFromInstance<TRootObject>(Type registeredType, TRootObject rootObject)
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
                CreateRootObjectData(rootObjectType);

            var rootObjectData = _rootObjectDataMap[rootObjectType];
            rootObjectData.AddRegisteredType(registeredType);

            _rootObjectTypeMap.Add(registeredType, rootObjectType);
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
                if (_cantBeRegisteredTypes.Contains(rootObjectInterface))
                    rootObjectInterfaces.Remove(rootObjectInterface);

            rootObjectInterfaces.Add(rootObjectType);

            var rootObjectBaseType = rootObjectType.BaseType;
            rootObjectInterfaces.Add(rootObjectBaseType);

            return rootObjectInterfaces.ToArray();
        }

        private void CreateRootObjectData<TRootObject>(TRootObject rootObject)
        {
            var rootObjectType = rootObject.GetType();
            var rootObjectData = new RootObjectData(rootObject);
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


        private void Remove(Type registeredType)
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

            RemoveRootObject(rootObjectType);
        }


        private void RemoveRootObject(Type rootObjectType)
        {
            if (!_rootObjectDataMap.TryGetValue(rootObjectType, out var rootObjectData))
            {
                Debug.LogWarning($"[Container::RemoveRootObject] RootObjectType: {rootObjectType} is already removed.");
                return;
            }

            var rootObject = rootObjectData.RootObject;
            RemoveUpdateAndFixedUpdate(rootObject);

            var registeredTypes = rootObjectData.RegisteredTypes;
            foreach (var registeredType in registeredTypes)
            {
                _rootObjectTypeMap.Remove(registeredType);
                rootObjectData.RemoveRegisteredType(registeredType);
            }

            _rootObjectDataMap.Remove(rootObjectType);
        }
    }
}