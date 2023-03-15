using System;

namespace CizaEntry.Example1
{
    public class CombatModule : ITickable, IFixedTickable, IDisposable, ICombatModule
    {
        public void Tick(float deltaTime)
        {
        }

        public void FixedTick(float fixedDeltaTime)
        {
        }

        public void Dispose()
        {
        }
    }
}