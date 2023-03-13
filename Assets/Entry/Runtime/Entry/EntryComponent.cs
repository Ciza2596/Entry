using System;
using UnityEngine;

namespace Cilix
{
    [DisallowMultipleComponent]
    internal class EntryComponent : MonoBehaviour
    {
        //private variable
        private Action<float> _updateCallback;
        private Action<float> _fixedUpdateCallback;
        private Action<float> _lateUpdateCallback;
        private Action _applicationQuit;

        

        //unity callback
        private void FixedUpdate() =>
            _fixedUpdateCallback?.Invoke(Time.fixedDeltaTime);
        
        private void Update() =>
            _updateCallback?.Invoke(Time.deltaTime);

        private void LateUpdate() =>
            _lateUpdateCallback?.Invoke(Time.deltaTime);

        private void OnApplicationQuit() =>
            _applicationQuit?.Invoke();
        


        //public method
        public void SetUpdateCallback(Action<float> updateCallback) =>
            _updateCallback = updateCallback;

        public void SetFixedUpdateCallback(Action<float> fixedUpdateCallback) =>
            _fixedUpdateCallback = fixedUpdateCallback;
        
        public void SetLateUpdateCallback(Action<float> lateUpdateCallback) =>
            _lateUpdateCallback = lateUpdateCallback;
        
        public void SetApplicationQuit(Action applicationQuit) =>
            _applicationQuit = applicationQuit;
        

        public void RemoveCallback()
        {
            _updateCallback = null;
            _fixedUpdateCallback = null;
        }
    }
}