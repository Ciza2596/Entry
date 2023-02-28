using NUnit.Framework;

public class ContainerTest
{
    private Entry.Container _container;

    [SetUp]
    public void SetUp()
    {
        _container = new Entry.Container();
    }

    [TearDown]
    public void TearDown()
    {
        _container.RemoveAllRootObjects();
        _container = null;
    }


    [Test]
    public void _01_Bind_Without_RegisteredObject()
    {
        
        
    }
}
