using UnityEngine;

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
            _container.RemoveAll();
            _entryUpdateComponent.RemoveCallback();

            _container = null;

            var entryUpdateComponent = _entryUpdateComponent;
            _entryUpdateComponent = null;

            Object.Destroy(entryUpdateComponent.gameObject);
        }


        public static TObject Bind<TObject>(params object[] parameters) where TObject : class
        {
            if (CheckIsNotInitialized())
                return null;

            return _container.Bind<TObject>(parameters);
        }

        public static TKey Bind<TKey, TObject>(params object[] parameters) where TObject : class where TKey : class
        {
            if (CheckIsNotInitialized())
                return null;

            return _container.Bind<TKey, TObject>(parameters);
        }

        public static bool TryResolve<TKey>(out TKey obj) where TKey : class
        {
            obj = null;
            if (CheckIsNotInitialized())
                return false;

            return _container.TryResolve<TKey>(out obj);
        }

        public static void Remove<TKey>() where TKey : class
        {
            if (CheckIsNotInitialized())
                return;

            _container.Remove<TKey>();
        }

        public static void RemoveAll()
        {
            if (CheckIsNotInitialized())
                return;

            _container.RemoveAll();
        }


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