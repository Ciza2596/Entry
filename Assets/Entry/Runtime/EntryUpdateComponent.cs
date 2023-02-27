using System;
using UnityEngine;

namespace Entry
{
    internal class EntryUpdateComponent : MonoBehaviour
    {
        //private variable
        private Action<float> _updateCallback;
        private Action<float> _fixedUpdateCallback;

        

        //unity callback
        private void Update() =>
            _updateCallback?.Invoke(Time.deltaTime);
        
        private void FixedUpdate() =>
            _fixedUpdateCallback?.Invoke(Time.fixedDeltaTime);

        

        //public method
        public void SetUpdateCallback(Action<float> updateCallback) =>
            _updateCallback = updateCallback;

        public void SetFixedUpdateCallback(Action<float> fixedUpdateCallback) =>
            _fixedUpdateCallback = fixedUpdateCallback;

        public void RemoveCallback()
        {
            _updateCallback = null;
            _fixedUpdateCallback = null;
        }
    }
}