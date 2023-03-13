using CizaEntry;

public class FakeFixedTickable : BaseFake, IFixedTickable
{
    public void FixedTick(float fixedDeltaTime)
    {
        SetResult(fixedDeltaTime.ToString());
    }
}