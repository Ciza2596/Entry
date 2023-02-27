using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Entry
{
    public class Container
    {
        //private variable
        private Action<float> _updateHandle;
        private Action<float> _fixedUpdateHandle;


        private List<Type> _cantBeKeys = new List<Type>(4) 
            { typeof(IInitializable), typeof(IUpdatable), typeof(IFixedUpdatable), typeof(IReleasable) };

        private readonly Dictionary<Type, object> _container = new Dictionary<Type, object>();


        //Unity callback
        public void Update(float deltaTime) =>
            _updateHandle(deltaTime);


        public void FixedUpdate(float fixedDeltaTime) =>
            _fixedUpdateHandle(fixedDeltaTime);


        //public method
        public TObject Bind<TObject>(params object[] parameters) where TObject : class =>
            Bind<TObject, TObject>(parameters);

        public TKey Bind<TKey, TObject>(params object[] parameters) where TObject : class where TKey : class
        {
            var key = typeof(TKey);
            var objType = typeof(TObject);

            if (_cantBeKeys.Contains(key))
            {
                Debug.LogError($"[Entry::Bind] Cant use key: {key}.");
                return null;
            }
            
            if (_container.ContainsKey(key))
            {
                Debug.LogError($"[Entry::Bind] Already has key: {key}.");
                return null;
            }

            var obj = (TObject)Activator.CreateInstance(objType);

            if (obj is IInitializable initializable)
                initializable.Initialize(parameters);

            AddUpdateAndFixedUpdate(obj);

            _container.Add(key, obj);

            return obj as TKey;
        }

        public bool TryResolve<TKey>(out TKey obj) where TKey : class
        {
            obj = null;

            var key = typeof(TKey);
            
            if (_cantBeKeys.Contains(key))
            {
                Debug.LogError($"[Entry::TryResolve] Cant use key: {key}.");
                return false;
            }
            
            if (!_container.TryGetValue(key, out var value))
            {
                Debug.LogWarning($"[Entry::TryResolve] Not find object by key: {key}.");
                return false;
            }

            obj = value as TKey;
            return true;
        }


        public void Remove<TKey>() where TKey : class
        {
            var key = typeof(TKey);
            Remove(key);
        }

        public void RemoveAll()
        {
            var keys = _container.Keys.ToArray();

            foreach (var key in keys)
                Remove(key);
        }


        //private method
        private void AddUpdateAndFixedUpdate(object obj)
        {
            if (obj is IUpdatable updatable)
                _updateHandle += updatable.Update;

            if (obj is IFixedUpdatable fixedUpdatable)
                _fixedUpdateHandle += fixedUpdatable.FixedUpdate;
        }

        private void RemoveUpdateAndFixedUpdate(object obj)
        {
            if (obj is IUpdatable updatable)
                _updateHandle -= updatable.Update;

            if (obj is IFixedUpdatable fixedUpdatable)
                _fixedUpdateHandle -= fixedUpdatable.FixedUpdate;
        }


        private void Remove(Type key)
        {
            if (!_container.TryGetValue(key, out var obj))
            {
                Debug.LogError($"[Entry::Remove] Not find object by key: {key}.");
                return;
            }

            RemoveUpdateAndFixedUpdate(obj);

            if (obj is IReleasable releasable)
                releasable.Release();

            _container.Remove(key);
        }
    }
}