# Entry
```
https://github.com/Ciza2596/Entry.git?path=Assets/Entry
```

## Info
Entry is an object manager. It supports unity callback methods including FixedUpdate, Update and  LateUpdate.


## Methods

1 - **Initialize**

```csharp
public class InitializeExample : MonoBehaviour
{
    private void Awake() =>
        Entry.Initialize();
}
```
Initialzes Entry and instantiates an EntryComponent on DontDestroyOnLoad scene. The EntryComponent is reponsible for processing Unity callbacks. 

2 - **Release**
```csharp
public class ReleaseExample : MonoBehaviour
{
    private void Awake() 
    {
        Entry.Initialize();
        Entry.Release();
    }
}
```
Releases Entry and Destroies the EntryComponent on DontDestroyOnLoad scene. When application is quitted, Entry will be released automatically.

3 - **Bind**
```csharp
public class Foo { }

public class BindExample : MonoBehaviour
{
    private void Awake()
    {
        Entry.Initialize();                 // Entry is initialized.
        Entry.Bind(new Foo());              // Bind Foo into Entry.
        Entry.TryResolve<Foo>(out var foo); // Get Foo from Entry.
    }
}
```
or

```csharp
public class Foo: IFoo { }

public class BindExample : MonoBehaviour
{
    private void Awake()
    {
        Entry.Initialize();            
        Entry.Bind<IFoo>(new Foo());         // Bind Foo with key IFoo into Entry.
        Entry.TryResolve<IFoo>(out var foo); // Get IFoo from instance Foo in Entry.
        
        // Since instance Foo is registered with key IFoo, getting Foo by using key Foo will not work.
        Entry.TryResolve<Foo>(out var foo);
    }
}
```
Binding multiple keys for a single object is available. Here are some methods that support it.
```csharp
public class Foo: IFoo { }

public class BindExample : MonoBehaviour
{
    private void Awake()
    {
        Entry.Initialize();            
        Entry.BindAndSelf<IFoo, Foo>(new Foo());  // Bind Foo with key IFoo and Foo into Entry.
        
        Entry.TryResolve<IFoo>(out var foo1);     // Get IFoo from instance Foo in Entry.
        Entry.TryResolve<Foo>(out var foo2);      // Get Foo from instance Foo in Entry.
        
        // The object reference of foo1 and foo2 is the same.
    }
}
```
or
```csharp
public class Foo: BaseFoo, IFoo { }

public class BindExample : MonoBehaviour
{
    private void Awake()
    {
        Entry.Initialize();            
        Entry.BindInheritances(new Foo());        // Bind Foo with key BaseFoo and IFoo into Entry.
        
        Entry.TryResolve<BaseFoo>(out var foo1);  // Get BaseFoo from instance Foo in Entry.
        Entry.TryResolve<IFoo>(out var foo2);     // Get IFoo from instance Foo in Entry.
        
        // The object reference of foo1 and foo2 is the same.
    }
}
```
4 - **Remove**
```csharp
public class Foo: IFoo { }

public class RemoveExample : MonoBehaviour
{
    private void Awake()
    {
        Entry.Initialize();
        Entry.BindAndSelf(new Foo());  // Instance Foo is registered with two keys, Foo and IFoo.
        
        Entry.RemoveKey<Foo>();        // Remove key Foo from instance Foo. Instance Foo won't be removed from Entry, because it still has a key, IFoo.
        Entry.RemoveKey<IFoo>();       // Instance Foo and key IFoo are both removed.
    }
}
```
If you want to remove an instance and all keys at once, here are some methods that support it.
```csharp
public class Foo: IFoo { }

public class RemoveExample : MonoBehaviour
{
    private void Awake()
    {
        Entry.Initialize();
        Entry.BindAndSelf(new Foo());  // Instance Foo is registered with two keys, Foo and IFoo.
        
        Entry.RemoveInstance<Foo>();   // Instance Foo, key Foo and key IFoo are all removed.
    }
}
```


## EntryPoint

Reference [Unity lifecycle document](https://docs.unity.cn/530/Documentation/Manual/ExecutionOrder.html).

1 - **IFixedTickable**

Timing: Nearly MonoBehaviour.FixedUpdate()
```csharp
public class Foo: IFixedTickable
{
    public void FixedTick(float fixedDeltaTime)
    {
        // Do some thing.
    }
}

public class IFixedTickableExample : MonoBehaviour
{
    private void Awake()
    {
        Entry.Initialize();
        Entry.Bind(new Foo());
    }
}
```

2 - **ITickable**

Time: Nearly MonoBehaviour.Update()
```csharp
public class Foo: ITickable
{
    public void Tick(float deltaTime)
    {
        // Do some thing.
    }
}

public class ITickableExample : MonoBehaviour
{
    private void Awake()
    {
        Entry.Initialize();
        Entry.Bind(new Foo());
    }
}
```

3 - **ILateTickable**

Time: Nearly MonoBehaviour.LateUpdate()
```csharp
public class Foo: ILateTickable
{
    public void LateTick(float deltaTime)
    {
        // Do some thing.
    }
}

public class ILateTickableExample : MonoBehaviour
{
    private void Awake()
    {
        Entry.Initialize();
        Entry.Bind(new Foo());
    }
}
```

4 - **IDisposable**

When an instance that implements IDisposable is removed, the method Dispose() will be called.
```csharp
public class Foo: IDisposable
{
    public void Dispose()
    {
        // Do some thing.
    }
}

public class IDisposableExample : MonoBehaviour
{
    private void Awake()
    {
        Entry.Initialize();
        Entry.Bind(new Foo());
        Entry.Remove<Foo>();    // Foo is removed and its method Dispose() is called.
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

