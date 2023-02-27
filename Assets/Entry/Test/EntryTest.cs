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
        _container.RemoveAllRootObjects();
        _container = null;
    }

    

    [Test]
    public void _01_Constructor()
    {
        //act
        _container.Bind<FakeConstructor>("Hello");
        _container.TryResolve<FakeConstructor>(out var fakeConstructor);

        //assert
        Assert.IsTrue(!string.IsNullOrWhiteSpace(fakeConstructor.Result), "The object doesnt initialize.");
    }

    [Test]
    public void _02_Updatable()
    {
        //arrange
        _container.Bind<FakeUpdatable>();
        _container.TryResolve<FakeUpdatable>(out var fakeUpdatable);
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
        _container.Bind<FakeFixedUpdatable>();
        _container.TryResolve<FakeFixedUpdatable>(out var fakeFixedUpdatable);
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
        _container.Bind<FakeReleasable>();
        _container.TryResolve<FakeReleasable>(out var fakeReleasable);
        
        //act
        _container.Remove<FakeReleasable>();
        
        //assert
        Assert.IsTrue(!string.IsNullOrWhiteSpace(fakeReleasable.Result), "The object doesnt release.");
    }
}