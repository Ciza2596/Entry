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
        private readonly IReadOnlyList<Type> _entryPoints = new List<Type>(3)
            { typeof(IFixedTickable), typeof(ITickable), typeof(ILateTickable), typeof(IDisposable) };


        private readonly Dictionary<Type, InstanceData> _instanceDataMap = new Dictionary<Type, InstanceData>();
        private readonly Dictionary<Type, Type> _instanceTypeMap = new Dictionary<Type, Type>();

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
        public Type[] InstanceTypes => _instanceDataMap.Keys.ToArray();
        public Type[] Keys => _instanceTypeMap.Keys.ToArray();


        //public method
        public bool TryGetInstanceType(Type key, out Type instanceType) =>
            _instanceTypeMap.TryGetValue(key, out instanceType);

        public bool TryGetKeys(Type instanceType, out Type[] keys)
        {
            keys = null;

            if (!_instanceDataMap.TryGetValue(instanceType, out var instanceData))
                return false;

            keys = instanceData.Keys;
            return true;
        }

        public bool TryGetEntryPoints(Type instanceType, out Type[] entryPoints)
        {
            entryPoints = null;
            if (!_instanceDataMap.TryGetValue(instanceType, out var instanceData))
                return false;

            entryPoints = instanceData.EntryPoints;
            return entryPoints != null && entryPoints.Length > 0;
        }


        public void Bind<TInstance>(TInstance instance) where TInstance : class =>
            Bind<TInstance, TInstance>(instance);


        public void Bind<TKey, TInstance>(TInstance instance)
            where TKey : class where TInstance : class =>
            Bind(typeof(TKey), instance);

        public void BindAndSelf<TKey, TInstance>(TInstance instance)
            where TKey : class where TInstance : class
        {
            var keys = new[] { typeof(TKey), typeof(TInstance) };
            foreach (var key in keys)
                Bind(key, instance);
        }

        public void BindInheritances<TInstance>(TInstance instance) where TInstance : class
        {
            var instanceType = typeof(TInstance);
            var canBeKeys = GetCanBeKeys(instanceType);

            foreach (var caneBeKey in canBeKeys)
                Bind(caneBeKey, instance);
        }

        public void BindInheritancesAndSelf<TInstance>(TInstance instance) where TInstance : class
        {
            var instanceType = typeof(TInstance);
            var canBeKeys = GetCanBeKeys(instanceType, true);

            foreach (var caneBeKey in canBeKeys)
                Bind(caneBeKey, instance);
        }


        public bool TryResolve<TKey>(out TKey keyObject)
            where TKey : class
        {
            keyObject = null;

            var key = typeof(TKey);
            Assert.IsTrue(!_entryPoints.Contains(key),
                $"[EntryContainer::TryResolve] Can't use key: {key} to find instance.");

            if (!_instanceTypeMap.TryGetValue(key, out var instanceType))
            {
                Debug.LogWarning(
                    $"[EntryContainer::TryResolve] Not find instance by key: {key}.");
                return false;
            }

            var instanceData = _instanceDataMap[instanceType];
            var instance = instanceData.Instance;

            keyObject = instance as TKey;

            return true;
        }


        public void RemoveKey<TKey>() where TKey : class =>
            RemoveKey(typeof(TKey));

        public void RemoveKey(Type key)
        {
            if (!_instanceTypeMap.TryGetValue(key, out var instanceType))
            {
                Debug.LogError($"[EntryContainer::RemoveKey] Key: {key} is already removed.");
                return;
            }

            _instanceTypeMap.Remove(key);

            var instanceData = _instanceDataMap[instanceType];
            instanceData.RemoveKey(key);

            if (instanceData.KeysCount > 0)
                return;

            RemoveInstanceByInstanceType(instanceType);
        }

        public void RemoveInstance<TKey>() where TKey : class =>
            RemoveInstance(typeof(TKey));

        public void RemoveInstance(Type key) =>
            RemoveInstanceByKey(key);

        public void RemoveAllInstances()
        {
            var instanceTypes = _instanceDataMap.Keys.ToArray();
            foreach (var instanceType in instanceTypes)
                RemoveInstanceByInstanceType(instanceType);
        }


        //private method
        private void Bind<TInstance>(Type key, TInstance instance)
        {
            var instanceType = instance.GetType();

            Assert.IsTrue(CheckCanBeKey(key, instanceType),
                $"[EntryContainer::Bind] Please check instanceType: {instanceType} inherits key: {key}.");

            if (_instanceTypeMap.TryGetValue(key, out var currentInstanceType))
            {
                Debug.LogWarning(
                    $"[EntryContainer::Bind] Key: {key} is already registered. Current matching instanceType: {currentInstanceType}");
                return;
            }


            if (!_instanceDataMap.ContainsKey(instanceType))
                CreateInstanceData(instance);

            var instanceData = _instanceDataMap[instanceType];
            instanceData.AddKey(key);
            _instanceTypeMap.Add(key, instanceType);
        }

        private bool CheckCanBeKey(Type key, Type instanceType)
        {
            var canBeKeys = GetCanBeKeys(instanceType, true);
            return canBeKeys.Contains(key);
        }

        private Type[] GetCanBeKeys(Type instanceType, bool isIncludeSelf = false)
        {
            var instanceInterfaces = instanceType.GetInterfaces().ToList();

            foreach (var instanceInterface in instanceInterfaces.ToArray())
                if (_entryPoints.Contains(instanceInterface))
                    instanceInterfaces.Remove(instanceInterface);

            var instanceBaseType = instanceType.BaseType;
            if (instanceBaseType != typeof(Object))
                instanceInterfaces.Add(instanceBaseType);

            if (isIncludeSelf)
                instanceInterfaces.Add(instanceType);

            return instanceInterfaces.ToArray();
        }

        private Type[] GetentryPoints(Type instanceType)
        {
            var instanceInterfaces = instanceType.GetInterfaces().ToList();
            var entryPoints = new List<Type>();

            foreach (var instanceInterface in instanceInterfaces.ToArray())
                if (_entryPoints.Contains(instanceInterface))
                    entryPoints.Add(instanceInterface);

            return entryPoints.ToArray();
        }

        private void CreateInstanceData<TInstance>(TInstance instance)
        {
            var instanceType = instance.GetType();
            var entryPoints = GetentryPoints(instanceType);
            var instanceData = new InstanceData(instance, entryPoints);
            _instanceDataMap.Add(instanceType, instanceData);

            AddFixedTickAndTickAndLateTickHandle(instance);
        }

        private void AddFixedTickAndTickAndLateTickHandle(object instance)
        {
            if (instance is ITickable updatable)
                _tickHandle += updatable.Tick;

            if (instance is IFixedTickable fixedUpdatable)
                _fixedTickHandle += fixedUpdatable.FixedTick;

            if (instance is ILateTickable lateUpdatable)
                _lateTickHandle += lateUpdatable.LateTick;
        }

        private void RemoveFixedTickAndTickAndLateTickHandle(object instance)
        {
            if (instance is ITickable updatable)
                _tickHandle -= updatable.Tick;

            if (instance is IFixedTickable fixedUpdatable)
                _fixedTickHandle -= fixedUpdatable.FixedTick;

            if (instance is ILateTickable lateUpdatable)
                _lateTickHandle -= lateUpdatable.LateTick;
        }

        private void RemoveInstanceByInstanceType(Type instanceType)
        {
            if (!_instanceDataMap.TryGetValue(instanceType, out var instanceData))
            {
                Debug.LogWarning(
                    $"[EntryContainer::RemoveInstanceByInstanceType] InstanceType: {instanceType} is already removed.");
                return;
            }

            var instance = instanceData.Instance;
            RemoveFixedTickAndTickAndLateTickHandle(instance);

            if (instance is IDisposable disposable)
                disposable.Dispose();

            var keys = instanceData.Keys;
            foreach (var key in keys)
            {
                _instanceTypeMap.Remove(key);
                instanceData.RemoveKey(key);
            }

            _instanceDataMap.Remove(instanceType);
        }

        private void RemoveInstanceByKey(Type key)
        {
            var hasValue = _instanceTypeMap.TryGetValue(key, out var instanceType);
            Assert.IsTrue(hasValue,
                $"[EntryContainer::RemoveInstanceByKey] Key: {key} is already removed.");

            RemoveInstanceByInstanceType(instanceType);
        }
    }
}