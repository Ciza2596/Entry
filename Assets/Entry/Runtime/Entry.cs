using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entry
{
    public static class Entry
    {
        //private variable
        private static Container _container;
        private static EntryComponent _entryComponent;


        //public variable
        public static bool IsInitialized => _container != null && _entryComponent != null;
        public static Type[] RootObjectTypes => CheckIsNotInitialized() ? null : _container.RootObjectTypes;
        public static Type[] RegisteredObjectTypes => CheckIsNotInitialized() ? null : _container.RegisteredObjectTypes;

        
        //public method
        public static void Initialize()
        {
            if (IsInitialized)
            {
                Debug.LogWarning("[Entry::Release] Entry is already Initialized.");
                return;
            }

            _container = new Container();

            var entry = new GameObject(nameof(Entry));
            Object.DontDestroyOnLoad(entry);
            _entryComponent = entry.AddComponent<EntryComponent>();

            _entryComponent.SetUpdateCallback(_container.Tick);
            _entryComponent.SetFixedUpdateCallback(_container.FixedTick);
            _entryComponent.SetApplicationQuit(Release);
        }

        public static void Release()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[Entry::Release] Entry is already released.");
                return;   
            }

            _container.RemoveAllRootObjects();
            _container = null;


            _entryComponent.RemoveCallback();

            var entryUpdateComponent = _entryComponent;
            _entryComponent = null;

            Object.Destroy(entryUpdateComponent.gameObject);
        }

        public static bool TryGetRootObjectType(Type registeredObjectType, out Type rootObjectType)
        {
            rootObjectType = null;
            if (CheckIsNotInitialized())
                return false;
            
           return _container.TryGetRootObjectType(registeredObjectType, out rootObjectType);
        }

        public static bool TryGetRegisteredObjectTypes(Type rootObjectType, out Type[] registeredTypes)
        {
            registeredTypes = null;
            if (CheckIsNotInitialized())
                return false;
            
            return _container.TryGetRegisteredObjectTypes(rootObjectType, out registeredTypes);
        }

        public static bool TryGetEntryPointTypes(Type rootObjectType, out Type[] entryPointTypes)
        {
            entryPointTypes = null;
            if (CheckIsNotInitialized())
                return false;
            
            return _container.TryGetEntryPointTypes(rootObjectType, out entryPointTypes);
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

        public static void BindAndSelf<TRegisteredObject, TRootObject>(TRootObject rootObject)
            where TRegisteredObject : class where TRootObject : class
        {
            if (CheckIsNotInitialized())
                return;

            _container.BindAndSelf<TRegisteredObject, TRootObject>(rootObject);
        }


        public static void BindInheritances<TRootObject>(TRootObject rootObject) where TRootObject : class
        {
            if (CheckIsNotInitialized())
                return;

            _container.BindInheritances(rootObject);
        }

        public static void BindInheritancesAndSelf<TRootObject>(TRootObject rootObject) where TRootObject : class
        {
            if (CheckIsNotInitialized())
                return;

            _container.BindInheritancesAndSelf(rootObject);
        }


        public static bool TryResolve<TRegisteredObject>(out TRegisteredObject registeredObject)
            where TRegisteredObject : class
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

        public static void RemoveRootObject(Type registeredType)
        {
            if (CheckIsNotInitialized())
                return;

            _container.RemoveRootObject(registeredType);
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