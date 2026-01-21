using System;
using UnityEngine;

namespace CizaEntry
{
    [DisallowMultipleComponent]
    internal class EntryMono : MonoBehaviour
    {
        //private variable
        private Action<float> _updateCallback;
        private Action<float> _fixedUpdateCallback;
        private Action<float> _lateUpdateCallback;
        private Action _onDisableCallback;

        

        //unity callback
        private void FixedUpdate() =>
            _fixedUpdateCallback?.Invoke(Time.fixedDeltaTime);
        
        private void Update() =>
            _updateCallback?.Invoke(Time.deltaTime);

        private void LateUpdate() =>
            _lateUpdateCallback?.Invoke(Time.deltaTime);

        private void OnDisable() =>
            _onDisableCallback?.Invoke();
        


        //public method
        public void SetUpdateCallback(Action<float> updateCallback) =>
            _updateCallback = updateCallback;

        public void SetFixedUpdateCallback(Action<float> fixedUpdateCallback) =>
            _fixedUpdateCallback = fixedUpdateCallback;
        
        public void SetLateUpdateCallback(Action<float> lateUpdateCallback) =>
            _lateUpdateCallback = lateUpdateCallback;
        
        public void SeOnDisableCallback(Action onDisableCallback) =>
            _onDisableCallback = onDisableCallback;
        

        public void RemoveCallback()
        {
            _updateCallback = null;
            _fixedUpdateCallback = null;
        }
    }
}