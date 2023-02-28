using Entry;

public class FakeUpdatable : BaseFake, IUpdatable
{
    public void Update(float deltaTime)
    {
        SetResult(deltaTime.ToString());
    }
}