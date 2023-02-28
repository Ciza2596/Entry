using Entry;

public class FakeFixedUpdatable : BaseFake, IFixedUpdatable
{
    public void FixedUpdate(float fixedDeltaTime)
    {
        SetResult(fixedDeltaTime.ToString());
    }
}