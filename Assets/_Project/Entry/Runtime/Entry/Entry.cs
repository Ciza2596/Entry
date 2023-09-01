using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaEntry
{
	public static class Entry
	{
		//private variable
		private static EntryContainer _entryContainer;
		private static EntryComponent _entryComponent;

		//public variable
		public static bool   IsInitialized => _entryContainer != null && _entryComponent != null;
		public static Type[] InstanceTypes => CheckIsNotInitialized() ? null : _entryContainer.InstanceTypes;
		public static Type[] Keys          => CheckIsNotInitialized() ? null : _entryContainer.Keys;

		//public method
		public static void Initialize()
		{
			if (IsInitialized)
			{
				Debug.LogWarning("[Entry::Initialize] Entry is already Initialized.");
				return;
			}

			_entryContainer = new EntryContainer();

			var entryName = $"[{nameof(Entry)}]";
			var entry     = new GameObject(entryName);
			Object.DontDestroyOnLoad(entry);
			_entryComponent = entry.AddComponent<EntryComponent>();

			_entryComponent.SetUpdateCallback(_entryContainer.Tick);
			_entryComponent.SetFixedUpdateCallback(_entryContainer.FixedTick);
			_entryComponent.SetLateUpdateCallback(_entryContainer.LateTick);
			_entryComponent.SeOnDisableCallback(Release);
		}

		public static void Release()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[Entry::Release] Entry is already released.");
				return;
			}

			_entryContainer.RemoveAllInstances();
			_entryContainer = null;


			_entryComponent.RemoveCallback();

			var entryUpdateComponent = _entryComponent;
			_entryComponent = null;

			Object.Destroy(entryUpdateComponent.gameObject);
		}

		public static bool TryGetInstanceType(Type key, out Type instanceType)
		{
			instanceType = null;
			if (CheckIsNotInitialized())
				return false;

			return _entryContainer.TryGetInstanceType(key, out instanceType);
		}

		public static bool TryGetKeys(Type instanceType, out Type[] keys)
		{
			keys = null;
			if (CheckIsNotInitialized())
				return false;

			return _entryContainer.TryGetKeys(instanceType, out keys);
		}

		public static bool TryGetEntryPoints(Type instanceType, out Type[] entryPoints)
		{
			entryPoints = null;
			if (CheckIsNotInitialized())
				return false;

			return _entryContainer.TryGetEntryPoints(instanceType, out entryPoints);
		}

		public static void Bind<TInstance>(TInstance instance) where TInstance : class
		{
			if (CheckIsNotInitialized())
				return;

			_entryContainer.Bind(instance);
		}

		public static void Bind<TKey, TInstance>(TInstance instance)
			where TKey : class where TInstance : class
		{
			if (CheckIsNotInitialized())
				return;

			_entryContainer.Bind<TKey, TInstance>(instance);
		}

		public static void BindAndSelf<TKey, TInstance>(TInstance instance)
			where TKey : class where TInstance : class
		{
			if (CheckIsNotInitialized())
				return;

			_entryContainer.BindAndSelf<TKey, TInstance>(instance);
		}

		public static void BindInheritances<TInstance>(TInstance instance) where TInstance : class
		{
			if (CheckIsNotInitialized())
				return;

			_entryContainer.BindInheritances(instance);
		}

		public static void BindInheritancesAndSelf<TInstance>(TInstance instance) where TInstance : class
		{
			if (CheckIsNotInitialized())
				return;

			_entryContainer.BindInheritancesAndSelf(instance);
		}

		public static bool TryResolve<TKey>(out TKey keyObject)
			where TKey : class
		{
			keyObject = null;
			if (CheckIsNotInitialized())
				return false;

			return _entryContainer.TryResolve(out keyObject);
		}

		public static void RemoveKey(Type key)
		{
			if (CheckIsNotInitialized())
				return;

			_entryContainer.RemoveKey(key);
		}

		public static void RemoveInstance(Type key)
		{
			if (CheckIsNotInitialized())
				return;

			_entryContainer.RemoveInstance(key);
		}

		public static void RemoveAllInstances()
		{
			if (CheckIsNotInitialized())
				return;

			_entryContainer.RemoveAllInstances();
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
