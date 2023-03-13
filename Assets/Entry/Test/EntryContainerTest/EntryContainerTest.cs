using System;
using System.Linq;
using NUnit.Framework;

public class EntryContainerTest
{
    private Cilix.EntryContainer _entryContainer;

    [SetUp]
    public void SetUp()
    {
        _entryContainer = new Cilix.EntryContainer();
    }

    [TearDown]
    public void TearDown()
    {
        _entryContainer.RemoveAllRootObjects();
        _entryContainer = null;
    }


    [Test]
    public void _01_Bind_Without_RegisteredObject()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();

        //act
        _entryContainer.Bind(fakeRootObject);

        //assert
        Check_RegisteredObjectTypes_Match(new[] { typeof(FakeRootObject) }, fakeRootObject.GetType());
    }

    [Test]
    public void _02_Bind()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();

        //act
        _entryContainer.Bind<IFakeRegisteredObject1>(fakeRootObject);


        //assert
        Check_RegisteredObjectTypes_Match(new[] { typeof(IFakeRegisteredObject1) }, fakeRootObject.GetType());
    }

    [Test]
    public void _03_BindAndSelf()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();

        //act
        _entryContainer.BindAndSelf<IFakeRegisteredObject1, FakeRootObject>(fakeRootObject);

        //assert
        Check_RegisteredObjectTypes_Match(new[] { typeof(IFakeRegisteredObject1), typeof(FakeRootObject) },
            fakeRootObject.GetType());
    }

    [Test]
    public void _04_BindInheritances()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();

        //act
        _entryContainer.BindInheritances(fakeRootObject);

        //assert
        Check_RegisteredObjectTypes_Match(new[] { typeof(IFakeRegisteredObject1), typeof(IFakeRegisteredObject2) },
            fakeRootObject.GetType());
    }

    [Test]
    public void _05_BindInheritancesAndSelf()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();

        //act
        _entryContainer.BindInheritancesAndSelf(fakeRootObject);

        //assert
        Check_RegisteredObjectTypes_Match(
            new[] { typeof(IFakeRegisteredObject1), typeof(IFakeRegisteredObject2), typeof(FakeRootObject) },
            fakeRootObject.GetType());
    }


    [Test]
    public void _06_TryResolve()
    {
        //arrange 
        var fakeRootObject = new FakeRootObject();
        _entryContainer.Bind<IFakeRegisteredObject1>(fakeRootObject);

        //act
        _entryContainer.TryResolve<IFakeRegisteredObject1>(out var fakeRegisteredObject);

        //assert
        Assert.True(fakeRegisteredObject is FakeRootObject, "Resolve fail. Please check registerObject.");
    }

    [Test]
    public void _07_Remove()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();
        _entryContainer.BindInheritancesAndSelf(fakeRootObject);

        //act
        _entryContainer.Remove<IFakeRegisteredObject1>();

        //assert
        Check_RegisteredObjectTypes_Match(new[] { typeof(IFakeRegisteredObject2), typeof(FakeRootObject) },
            fakeRootObject.GetType());
    }
    
    [Test]
    public void _08_RemoveRootObject()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();
        _entryContainer.BindInheritancesAndSelf(fakeRootObject);

        //act
        _entryContainer.RemoveRootObject<IFakeRegisteredObject1>();

        //assert
        Assert.AreEqual(0, _entryContainer.RootObjectTypes.Length, "Doesnt remove rootObject.");
    }
    
    
    [Test]
    public void _09_RemoveAllRootObject()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();
        _entryContainer.BindInheritancesAndSelf(fakeRootObject);

        //act
        _entryContainer.RemoveAllRootObjects();

        //assert
        Assert.AreEqual(0, _entryContainer.RootObjectTypes.Length, "Doesnt remove rootObject.");
    }


    //private method
    private void Check_RegisteredObjectTypes_Match(Type[] exceptedTypes, Type rootObjectType)
    {
        _entryContainer.TryGetRegisteredObjectTypes(rootObjectType, out var registeredObjectTypes);

        foreach (var registeredObjectType in registeredObjectTypes)
            Assert.IsTrue(exceptedTypes.Contains(registeredObjectType),
                $"RegisteredObjectType is not match any type in exceptedTypes. RegisteredObjectType: {registeredObjectType}.");
    }
}