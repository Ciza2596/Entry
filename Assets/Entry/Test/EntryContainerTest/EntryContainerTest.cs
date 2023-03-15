using System;
using System.Linq;
using CizaEntry;
using NUnit.Framework;

public class EntryContainerTest
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
    public void _01_Bind_Without_Key()
    {
        //arrange
        var fakeInstance = new FakeInstance();

        //act
        _entryContainer.Bind(fakeInstance);

        //assert
        Check_Key_Match(new[] { typeof(FakeInstance) }, fakeInstance.GetType());
    }

    [Test]
    public void _02_Bind()
    {
        //arrange
        var fakeInstance = new FakeInstance();

        //act
        _entryContainer.Bind<IFakeKey1>(fakeInstance);


        //assert
        Check_Key_Match(new[] { typeof(IFakeKey1) }, fakeInstance.GetType());
    }

    [Test]
    public void _03_BindAndSelf()
    {
        //arrange
        var fakeInstance = new FakeInstance();

        //act
        _entryContainer.BindAndSelf<IFakeKey1, FakeInstance>(fakeInstance);

        //assert
        Check_Key_Match(new[] { typeof(IFakeKey1), typeof(FakeInstance) },
            fakeInstance.GetType());
    }

    [Test]
    public void _04_BindInheritances()
    {
        //arrange
        var fakeInstance = new FakeInstance();

        //act
        _entryContainer.BindInheritances(fakeInstance);

        //assert
        Check_Key_Match(new[] { typeof(IFakeKey1), typeof(IFakeKey2) },
            fakeInstance.GetType());
    }

    [Test]
    public void _05_BindInheritancesAndSelf()
    {
        //arrange
        var fakeInstance = new FakeInstance();

        //act
        _entryContainer.BindInheritancesAndSelf(fakeInstance);

        //assert
        Check_Key_Match(
            new[] { typeof(IFakeKey1), typeof(IFakeKey2), typeof(FakeInstance) },
            fakeInstance.GetType());
    }


    [Test]
    public void _06_TryResolve()
    {
        //arrange 
        var fakeInstance = new FakeInstance();
        _entryContainer.Bind<IFakeKey1>(fakeInstance);

        //act
        _entryContainer.TryResolve<IFakeKey1>(out var fakeKeyObject);

        //assert
        Assert.True(fakeKeyObject is FakeInstance, "Resolve fail. Please check keyObject.");
    }

    [Test]
    public void _07_RemoveKey()
    {
        //arrange
        var fakeInstance = new FakeInstance();
        _entryContainer.BindInheritancesAndSelf(fakeInstance);

        //act
        _entryContainer.RemoveKey<IFakeKey1>();

        //assert
        Check_Key_Match(new[] { typeof(IFakeKey2), typeof(FakeInstance) },
            fakeInstance.GetType());
    }
    
    [Test]
    public void _08_RemoveInstance()
    {
        //arrange
        var fakeInstance = new FakeInstance();
        _entryContainer.BindInheritancesAndSelf(fakeInstance);

        //act
        _entryContainer.RemoveInstance<IFakeKey1>();

        //assert
        Assert.AreEqual(0, _entryContainer.InstanceTypes.Length, "Doesnt remove instance.");
    }
    
    
    [Test]
    public void _09_RemoveAllInstances()
    {
        //arrange
        var fakeInstance = new FakeInstance();
        _entryContainer.BindInheritancesAndSelf(fakeInstance);

        //act
        _entryContainer.RemoveAllInstances();

        //assert
        Assert.AreEqual(0, _entryContainer.InstanceTypes.Length, "Doesnt remove instance.");
    }


    //private method
    private void Check_Key_Match(Type[] expectedKeys, Type instanceType)
    {
        _entryContainer.TryGetKeys(instanceType, out var keys);

        foreach (var key in keys)
            Assert.IsTrue(expectedKeys.Contains(key),
                $"Key: {key} is not match any expectedKey in expectedKeys.");
    }
}