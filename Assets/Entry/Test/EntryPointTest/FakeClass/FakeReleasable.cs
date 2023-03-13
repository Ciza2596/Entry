using Cilix;

public class FakeReleasable :BaseFake, IReleasable
{
    public void Release()
    {
        SetResult();
    }
}