using System;
using System.Linq;
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
        //arrange
        var fakeRootObject = new FakeRootObject();

        //act
        _container.Bind(fakeRootObject);

        //assert
        Check_RegisteredObjectTypes_Match(new[] { typeof(FakeRootObject) }, fakeRootObject.GetType());
    }

    [Test]
    public void _02_Bind()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();

        //act
        _container.Bind<IFakeRegisteredObject1>(fakeRootObject);

        //assert
        Check_RegisteredObjectTypes_Match(new[] { typeof(IFakeRegisteredObject1) }, fakeRootObject.GetType());
    }

    [Test]
    public void _03_BindAndSelf()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();

        //act
        _container.BindAndSelf<IFakeRegisteredObject1, FakeRootObject>(fakeRootObject);

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
        _container.BindInheritances(fakeRootObject);

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
        _container.BindInheritancesAndSelf(fakeRootObject);

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
        _container.Bind<IFakeRegisteredObject1>(fakeRootObject);

        //act
        _container.TryResolve<IFakeRegisteredObject1>(out var fakeRegisteredObject);

        //assert
        Assert.True(fakeRegisteredObject is FakeRootObject, "Resolve fail. Please check registerObject.");
    }

    [Test]
    public void _07_Remove()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();
        _container.BindInheritancesAndSelf(fakeRootObject);

        //act
        _container.Remove<IFakeRegisteredObject1>();

        //assert
        Check_RegisteredObjectTypes_Match(new[] { typeof(IFakeRegisteredObject2), typeof(FakeRootObject) },
            fakeRootObject.GetType());
    }
    
    [Test]
    public void _08_RemoveRootObject()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();
        _container.BindInheritancesAndSelf(fakeRootObject);

        //act
        _container.RemoveRootObject<IFakeRegisteredObject1>();

        //assert
        Assert.AreEqual(0, _container.RootObjectTypes.Length, "Doesnt remove rootObject.");
    }
    
    
    [Test]
    public void _09_RemoveAllRootObject()
    {
        //arrange
        var fakeRootObject = new FakeRootObject();
        _container.BindInheritancesAndSelf(fakeRootObject);

        //act
        _container.RemoveAllRootObjects();

        //assert
        Assert.AreEqual(0, _container.RootObjectTypes.Length, "Doesnt remove rootObject.");
    }


    //private method
    private void Check_RegisteredObjectTypes_Match(Type[] exceptedTypes, Type rootObjectType)
    {
        _container.TryGetRegisteredObjectTypes(rootObjectType, out var registeredObjectTypes);

        foreach (var registeredObjectType in registeredObjectTypes)
            Assert.IsTrue(exceptedTypes.Contains(registeredObjectType),
                $"RegisteredObjectType is not match any type in exceptedTypes. RegisteredObjectType: {registeredObjectType}.");
    }
}