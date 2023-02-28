using System;
using Entry;
using NUnit.Framework;

public class ContainerLifeScopeTest
{
    private Container _container;

    [SetUp]
    public void SetUp()
    {
        _container = new Container();
    }

    [TearDown]
    public void TearDown()
    {
        _container.RemoveAllRootObjects();
        _container = null;
    }


    [Test]
    public void _01_Updatable()
    {
        //arrange
        var fakeUpdatable = Bind_And_Resolve(new FakeUpdatable());
        Check_LifeScope_Match(fakeUpdatable.GetType(), typeof(IUpdatable));
        
        var deltaTime = 0.02f;

        //act
        _container.Update(deltaTime);

        //assert
        Assert.AreEqual(deltaTime.ToString(), fakeUpdatable.Result, "DeltaTime doesnt match.");
    }

    [Test]
    public void _02_FixedUpdatable()
    {
        //arrange
        var fakeFixedUpdatable = Bind_And_Resolve(new FakeFixedUpdatable());
        Check_LifeScope_Match(fakeFixedUpdatable.GetType(), typeof(IFixedUpdatable));

        var fixedDeltaTime = 0.02f;

        //act
        _container.FixedUpdate(fixedDeltaTime);

        //assert
        Assert.AreEqual(fixedDeltaTime.ToString(), fakeFixedUpdatable.Result, "FixedDeltaTime doesnt match.");
    }

    [Test]
    public void _03_Releasable()
    {
        //arrange
        var fakeReleasable = Bind_And_Resolve(new FakeReleasable());
        Check_LifeScope_Match(fakeReleasable.GetType(), typeof(IReleasable));

        //act
        _container.RemoveRootObject<FakeReleasable>();

        //assert
        Assert.IsTrue(!string.IsNullOrWhiteSpace(fakeReleasable.Result), "The object doesnt release.");
    }


    //private method
    private TRootObject Bind_And_Resolve<TRootObject>(TRootObject rootObject) where TRootObject : class
    {
        _container.Bind(rootObject);
        _container.TryResolve<TRootObject>(out var registeredObject);

        return registeredObject;
    }

    private void Check_LifeScope_Match(Type registeredObjectType, Type exceptedType)
    {
        _container.TryGetLifeScopeTypes(registeredObjectType, out var lifeScopeTypes);
        var lifeScopeType = lifeScopeTypes[0];
        Assert.AreEqual(exceptedType, lifeScopeType,
            $"LifeScope is not match. ExceptedType: {exceptedType}, lifeScopeType: {lifeScopeType}.");
    }
}