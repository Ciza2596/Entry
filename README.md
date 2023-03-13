# CilixEngine.Entry
```
https://github.com/Ciza2596/Entry.git?path=Assets/Entry
```

## Info
Entry is object controller, like singleton.


## Method

1 - **Initialize**

```csharp
public class InitializeGame : MonoBehaviour
{
    private void Awake() =>
        Entry.Entry.Initialize();
}
```
Entry is initialized then Entry instantiate EntryComponent on DontDestroyOnLoad scene. It is responsible for processing Unity callback.

2 - **Release**
```csharp
public class InitializeGame : MonoBehaviour
{
    private void Awake() 
    {
        Entry.Entry.Initialize();
        Entry.Entry.Release();
    }
}
```

Entry is released then Entry Destroy EntryComponent on DontDestroyOnLoad scene. When Application is quited, entry will auto release.

3 - **Bind**
```csharp
public class Foo { }

public class InitializeGame : MonoBehaviour
{
    private void Awake()
    {
        Entry.Entry.Initialize();
        Entry.Entry.Bind(new Foo());              //Bind Foo into Entry.
        Entry.Entry.TryResolve<Foo>(out var foo); //Get Foo from Entry.
    }
}
```
or

```csharp
public class Foo: IFoo { }

public class InitializeGame : MonoBehaviour
{
    private void Awake()
    {
        Entry.Entry.Initialize();            
        Entry.Entry.Bind<IFoo>(new Foo());         //Bind IFoo into Entry.
        Entry.Entry.TryResolve<IFoo>(out var foo); //Get IFoo from Entry.
        
        //If find object by Foo, it can't find anything.
        Entry.Entry.TryResolve<Foo>(out var foo);
    }
}
```
Want to register more key for one object, some methods support it.
```csharp
public class Foo: IFoo { }

public class InitializeGame : MonoBehaviour
{
    private void Awake()
    {
        Entry.Entry.Initialize();            
        Entry.Entry.BindAndSelf<IFoo, Foo>(new Foo());  //Bind IFoo and Foo into Entry.
        
        Entry.Entry.TryResolve<IFoo>(out var foo1); //Get IFoo from Entry.
        Entry.Entry.TryResolve<Foo>(out var foo2);  //Get Foo from Entry.
        
        //foo1 and foo2 is same object reference.
    }
}
```
or
```csharp
public class Foo: IFoo, IFoo2 { }

public class InitializeGame : MonoBehaviour
{
    private void Awake()
    {
        Entry.Entry.Initialize();            
        Entry.Entry.BindInheritances(new Foo());  //Bind IFoo and Foo into Entry.
        
        Entry.Entry.TryResolve<IFoo>(out var foo);    //Get IFoo from Entry.
        Entry.Entry.TryResolve<IFoo2>(out var foo2);  //Get IFoo2 from Entry.
        
        //foo and foo2 is same object reference.
    }
}
```
4 - **Remove**
```csharp
public class Foo: IFoo { }

public class InitializeGame : MonoBehaviour
{
    private void Awake()
    {
        Entry.Entry.Initialize();
        Entry.Entry.BindAndSelf(new Foo());  //RootObject: Foo is registered two key Foo and IFoo.
        
        Entry.Entry.Remove<Foo>();  //RootObject isn't removed, only Foo registered key is removed.
        Entry.Entry.Remove<IFoo>(); //RootObject is removed, Foo and IFoo registered keys are removed.
    }
}
```
Want to remove RootObject once.
```csharp
public class Foo: IFoo { }

public class InitializeGame : MonoBehaviour
{
    private void Awake()
    {
        Entry.Entry.Initialize();
        Entry.Entry.BindAndSelf(new Foo());  //RootObject: Foo is registered two key Foo and IFoo.
        
        Entry.Entry.RemoveRootObject<Foo>(); //RootObject is removed, including Foo and IFoo registered key is removed.
    }
}
```


## EntryPoint

Reference [Unity lifecycle document](https://docs.unity.cn/530/Documentation/Manual/ExecutionOrder.html).

1 - **IFixedTickable**
Nearly MonoBehaviour.FixedUpdate()
```csharp
public class Foo: IFixedTickable
{
    public void FixedTick(float fixedDeltaTime)
    {
        //Do some thing.
    }
}

public class InitializeGame : MonoBehaviour
{
    private void Awake()
    {
        Entry.Entry.Initialize();
        Entry.Entry.Bind(new Foo());
    }
}
```

2 - **ITickable**
Nearly MonoBehaviour.Update()
```csharp
public class Foo: ITickable
{
    public void Tick(float deltaTime)
    {
        //Do some thing.
    }
}

public class InitializeGame : MonoBehaviour
{
    private void Awake()
    {
        Entry.Entry.Initialize();
        Entry.Entry.Bind(new Foo());
    }
}
```

3 - **IReasable**
```csharp
public class Foo: IReleasable
{
    public void Release()
    {
        //Do some thing.
    }
}

public class InitializeGame : MonoBehaviour
{
    private void Awake()
    {
        Entry.Entry.Initialize();
        Entry.Entry.Bind(new Foo());
        Entry.Entry.Remove<Foo>();  //Foo's Release will be trigger.
    }
}
```


## Editor
1 - **Open Entry window**

<img src="Document/Image/EntryWindowPath.png?"/>

2 - **Entry isn't initialized**
          
<img src="Document/Image/EntryWindowIsntInitialized.png?"/>

3 - **Entry is initialized**

<img src="Document/Image/EntryWindowIsInitialized.png?"/>

