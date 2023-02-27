using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entry
{
    public static class Entry
    {
        //private variable
        private static Container _container;
        private static EntryUpdateComponent _entryUpdateComponent;


        //public method
        public static bool IsInitialized => _container != null && _entryUpdateComponent != null;

        public static void Initialize()
        {
            _container = new Container();

            var entry = new GameObject(nameof(Entry));
            Object.DontDestroyOnLoad(entry);
            _entryUpdateComponent = entry.AddComponent<EntryUpdateComponent>();

            _entryUpdateComponent.SetUpdateCallback(_container.Update);
            _entryUpdateComponent.SetFixedUpdateCallback(_container.FixedUpdate);
        }


        public static void Release()
        {
            _container.RemoveAllRootObjects();
            _entryUpdateComponent.RemoveCallback();

            _container = null;

            var entryUpdateComponent = _entryUpdateComponent;
            _entryUpdateComponent = null;

            Object.Destroy(entryUpdateComponent.gameObject);
        }


        public static void Bind<TRootObject>(TRootObject rootObject) where TRootObject : class
        {
            if (CheckIsNotInitialized())
                return;

            _container.Bind(rootObject);
        }

        public static void Bind<TRegisteredObject, TRootObject>(TRootObject rootObject)
            where TRegisteredObject : class where TRootObject : class
        {
            if (CheckIsNotInitialized())
                return;

            _container.Bind<TRegisteredObject, TRootObject>(rootObject);
        }

        public static bool TryResolve<TRegisteredObject>(out TRegisteredObject registeredObject) where TRegisteredObject : class
        {
            registeredObject = null;
            if (CheckIsNotInitialized())
                return false;

            return _container.TryResolve(out registeredObject);
        }

        public static void Remove(Type registeredType)
        {
            if (CheckIsNotInitialized())
                return;

            _container.Remove(registeredType);
        }

        public static void RemoveAllRootObjects()
        {
            if (CheckIsNotInitialized())
                return;

            _container.RemoveAllRootObjects();
        }


        //private method
        private static bool CheckIsNotInitialized()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[Entry::CheckIsInitialized] Entry is not initialized.");
                return true;
            }

            return false;
        }
    }
}