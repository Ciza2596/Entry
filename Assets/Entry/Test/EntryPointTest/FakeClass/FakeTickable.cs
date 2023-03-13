using Cilix;

public class FakeTickable : BaseFake, ITickable
{
    public void Tick(float deltaTime)
    {
        SetResult(deltaTime.ToString());
    }
}