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
        _entryContainer.RemoveAllRootObjects();
        _entryContainer = null;
    }

    [Test]
    public void _01_FixedTickable()
    {
        //arrange
        var fakeFixedTickable = Bind_And_Resolve(new FakeFixedTickable());
        Check_EntryPointType_Match(typeof(IFixedTickable), fakeFixedTickable.GetType());

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
        Check_EntryPointType_Match(typeof(ITickable), fakeTickable.GetType());
        
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
        Check_EntryPointType_Match(typeof(ILateTickable), fakeLateTickable.GetType());
        
        var deltaTime = 0.02f;

        //act
        _entryContainer.LateTick(deltaTime);

        //assert
        Assert.AreEqual(deltaTime.ToString(), fakeLateTickable.Result, "DeltaTime doesnt match.");
    }
    

    [Test]
    public void _03_Releasable()
    {
        //arrange
        var fakeReleasable = Bind_And_Resolve(new FakeReleasable());
        Check_EntryPointType_Match(typeof(IReleasable), fakeReleasable.GetType());

        //act
        _entryContainer.RemoveRootObject<FakeReleasable>();

        //assert
        Assert.IsTrue(!string.IsNullOrWhiteSpace(fakeReleasable.Result), "The object doesnt release.");
    }


    //private method
    private TRootObject Bind_And_Resolve<TRootObject>(TRootObject rootObject) where TRootObject : class
    {
        _entryContainer.Bind(rootObject);
        _entryContainer.TryResolve<TRootObject>(out var registeredObject);

        return registeredObject;
    }

    private void Check_EntryPointType_Match(Type exceptedType, Type rootObjectType)
    {
        _entryContainer.TryGetEntryPointTypes(rootObjectType, out var entryPointTypes);
        var entryPointType = entryPointTypes[0];
        Assert.AreEqual(exceptedType, entryPointType,
            $"EntryPoint is not match. ExceptedType: {exceptedType}, entryPointType: {entryPointType}.");
    }
}