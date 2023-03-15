using System;

public class FakeDisposable :BaseFake, IDisposable
{
    public void Dispose()
    {
        SetResult();
    }
}