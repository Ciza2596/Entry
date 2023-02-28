using UnityEngine;

namespace Entry.Example1
{
    public class EntryExampleController : MonoBehaviour
    {
        private void OnEnable()
        {
            Entry.Initialize();
            Entry.BindInheritancesAndSelf(new CombatModule());
            Entry.BindInheritancesAndSelf(new DungeonModule());
            Entry.BindInheritancesAndSelf(new MemoryModule());
        }

        private void OnDisable()
        {
            Entry.Release();
        }
    }
}
