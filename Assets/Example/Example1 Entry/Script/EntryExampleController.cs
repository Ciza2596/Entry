using UnityEngine;

namespace Entry.Example1
{
    public class EntryExampleController : MonoBehaviour
    {
        private void OnEnable()
        {
            Entry.Initialize();
        }

        private void OnDisable()
        {
            Entry.Release();
        }
    }
}
