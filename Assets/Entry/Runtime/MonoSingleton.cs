using UnityEngine;

namespace Entry
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static bool HasInstance => _instance != null;

        private static T _instance;
        private static object _lock = new object();

        protected static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (!Application.isPlaying)
                    {
                        Debug.LogWarning(
                            $"[MonoSingleton::Instance] Instance already destroyed on application quit. Won't create again returning null. Type : {typeof(T)}");
                        return null;
                    }

                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError(
                                $"[MonoSingleton::Instance] Something went really wrong there should never be more than 1 singleton! Reopening the scene might fix it. Type : {typeof(T)}");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = $"[MonoSingleton::Instance:{typeof(T).Name}]";

                            if (Application.isPlaying)
                                DontDestroyOnLoad(singleton);

                            Debug.Log(
                                $"[MonoSingleton::Instance] An instance is created in the scene. Type : {typeof(T)}");
                        }
                    }

                    return _instance;
                }
            }
        }

        private void OnApplicationQuit()
        {
            _instance = null;
        }
    }
}