using NUnit.Framework;

public class EntryTest
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
        _container.RemoveAll();
        _container = null;
    }

    

    [Test]
    public void _01_Constructor()
    {
        //act
        var fakeConstructor = _container.Bind<FakeConstructor>("Hello");

        //assert
        Assert.IsTrue(!string.IsNullOrWhiteSpace(fakeConstructor.Result), "The object doesnt initialize.");
    }

    [Test]
    public void _02_Updatable()
    {
        //arrange
        var fakeUpdatable = _container.Bind<FakeUpdatable>();
        var deltaTime = 0.02f;
        
        //act
        _container.Update(deltaTime);
        
        //assert
        Assert.AreEqual(fakeUpdatable.Result, deltaTime.ToString(), "DeltaTime doesnt match.");
    }
    
    [Test]
    public void _03_FixedUpdatable()
    {
        //arrange
        var fakeFixedUpdatable = _container.Bind<FakeFixedUpdatable>();
        var fixedDeltaTime = 0.02f;
        
        //act
        _container.FixedUpdate(fixedDeltaTime);
        
        //assert
        Assert.AreEqual(fakeFixedUpdatable.Result, fixedDeltaTime.ToString(), "FixedDeltaTime doesnt match.");
    }

    [Test]
    public void _04_Release()
    {
        //arrange
        var fakeReleasable = _container.Bind<FakeReleasable>();
        
        //act
        _container.Remove<FakeReleasable>();
        
        //assert
        Assert.IsTrue(!string.IsNullOrWhiteSpace(fakeReleasable.Result), "The object doesnt release.");
    }
}