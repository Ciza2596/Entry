using System;
using CizaEntry;
using NUnit.Framework;

public class EntryPointTest
{
    private EntryContainer _entryContainer;

    [SetUp]
    public void SetUp()
    {
        _entryContainer = new EntryContainer();
    }

    [TearDown]
    public void TearDown()
    {
        _entryContainer.RemoveAllInstances();
        _entryContainer = null;
    }

    [Test]
    public void _01_FixedTickable()
    {
        //arrange
        var fakeFixedTickable = Bind_And_Resolve(new FakeFixedTickable());
        Check_EntryPoint_Match(typeof(IFixedTickable), fakeFixedTickable.GetType());

        var fixedDeltaTime = 0.02f;

        //act
        _entryContainer.FixedTick(fixedDeltaTime);

        //assert
        Assert.AreEqual(fixedDeltaTime.ToString(), fakeFixedTickable.Result, "FixedDeltaTime doesnt match.");
    }
    

    [Test]
    public void _02_Tickable()
    {
        //arrange
        var fakeTickable = Bind_And_Resolve(new FakeTickable());
        Check_EntryPoint_Match(typeof(ITickable), fakeTickable.GetType());
        
        var deltaTime = 0.02f;

        //act
        _entryContainer.Tick(deltaTime);

        //assert
        Assert.AreEqual(deltaTime.ToString(), fakeTickable.Result, "DeltaTime doesnt match.");
    }
    
    
    [Test]
    public void _03_LateTickable()
    {
        //arrange
        var fakeLateTickable = Bind_And_Resolve(new FakeLateTickable());
        Check_EntryPoint_Match(typeof(ILateTickable), fakeLateTickable.GetType());
        
        var deltaTime = 0.02f;

        //act
        _entryContainer.LateTick(deltaTime);

        //assert
        Assert.AreEqual(deltaTime.ToString(), fakeLateTickable.Result, "DeltaTime doesnt match.");
    }
    

    [Test]
    public void _03_Disposable()
    {
        //arrange
        var fakeReleasable = Bind_And_Resolve(new FakeDisposable());
        Check_EntryPoint_Match(typeof(IDisposable), fakeReleasable.GetType());

        //act
        _entryContainer.RemoveInstance<FakeDisposable>();

        //assert
        Assert.IsTrue(!string.IsNullOrWhiteSpace(fakeReleasable.Result), "The object doesnt dispose.");
    }


    //private method
    private TInstance Bind_And_Resolve<TInstance>(TInstance instance) where TInstance : class
    {
        _entryContainer.Bind(instance);
        _entryContainer.TryResolve<TInstance>(out var registeredObject);

        return registeredObject;
    }

    private void Check_EntryPoint_Match(Type expectedEntryPoint, Type entryPoint)
    {
        _entryContainer.TryGetEntryPoints(entryPoint, out var entryPoints);
        var entryPointType = entryPoints[0];
        Assert.AreEqual(expectedEntryPoint, entryPointType,
            $"EntryPoint is not match. ExpectedEntryPoint: {expectedEntryPoint}, EntryPoint: {entryPointType}.");
    }
}