using CizaEntry;

public class FakeLateTickable : BaseFake, ILateTickable
{
    public void LateTick(float deltaTime)
    {
        SetResult(deltaTime.ToString());
    }
}