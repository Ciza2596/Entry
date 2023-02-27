using Entry;

public class FakeInitializable :BaseFake, IInitializable
{
    public void Initialize(params object[] parameters)
    {
        SetResult(parameters[0] as string);
    }
}