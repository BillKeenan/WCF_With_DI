# Configuring WCF with StructureMap for Dependancy Injection 

I’ve read a lot of articles on how to use Dependancy Injection with WCF, and I found most of them to be pretty inscrutable, it really started to seem like an unscalable problem.

After a lot of futzing, a combination of articles lead me to look closer at servicehostfactory, and how it creates the service. Once I found the solution, it's actually pretty simple, and I don't understand why all the examples are so baffling.

## Create a wcf Project 
of course, nothing fancy

## Create a class / interface that need to be injected 

I've created a class and interface
    
	Needed.cs
	iNeeded.cs

I've put a simple method in Needed so we can see it working

	String GetWord();

Here I've done some wiring up in the web.config so we have a JSON getenabled service, have a look for yourself. Fire up debug, and in your browser go to:

* http://localhost:60203/Service1.svc/GetData

Your port will likely be different of course

Inside the ```GetWord``` method I'm simply instantiating Needed and calling GetWord so we can ensure everything is wired up.

## First Commit

* SHA: 4e70ccb43c017a0cb4bd51f0838460c8b87bab38

## Create a custom Service Host

This is pretty minimal, simply create a class extending ServiceHostFactory

    public class MyServiceFactory : ServiceHostFactory

The only thing we need to do here is override the CreateServiceHost method

     protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
     {
        return new MyServiceHost(serviceType, baseAddresses);
     }

Now of course you need to create MyServiceHost, this things job is basically to register all the factories, we're only going to need 1 provider

      public class MyServiceHost : ServiceHost
    {
        public MyServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            foreach (var cd in this.ImplementedContracts.Values)
            {
                cd.Behaviors.Add(new MyInstanceProvider(serviceType));
            }
        }
    }

Here you could use a different provider for each type, but we don't need that as our provider is going to be generic.

So now we've done this, we need to create the MyInstanceProvider object

    public class MyInstanceProvider : IInstanceProvider, IContractBehavior

There's a number of methods that you need to implement, but the only one that we need to really do anything with is the GetInstance method

    public object GetInstance(InstanceContext instanceContext)
    {
        return new Service1();
    }

Here you can see I've only implemented it for Service1, which of course wont work in the long run, but at this point we can test that our service works, and that our factory works. 

Put a breakpoint on the MyInstanceProvider.GetInstance method and fire it up, the result should be the same as before, but you'll see the GetInstance method get called.

## Second Commit
* SHA: f75dffa4b060fac850077f058528cf1133640551

## Add iNeeded as a dependancy

Simply add it as a constructor parameter and set a field. 

    public Service1(INeeded needed)
    {
        _needed = needed;
    }
At this point the project won't build, because of the constructor parameter, so back in MyInstanceProvider, modify the line to inject needed

     return new Service1(new Needed());

Fire it up, and we should be good.

Now we have real dependancy injection happening with WCF! Now you could write a handler to inspect the type that was passed into MyInstanceProvider and do your own constructors, switching based on type, but we're going to use StructureMap.

## Configure StructureMap
using nuget / Package manager console
   
    
    PM> Install-Package structuremap
    Successfully installed 'structuremap 2.6.4.1'.
    Successfully added 'structuremap 2.6.4.1' to WcfWithDI.
   
Now in MyInstanceProvider we will use StructureMap to get the instance requested

    return ObjectFactory.GetInstance(_serviceType); 

StructureMap is pretty simple to configure, if you run debug at this point it will blow up saying it has no default instance of INeeded configured.

We're going to use the web.config to setup StructureMap
 
```xml
<StructureMap MementoStyle="Attribute">
    <DefaultInstance PluginType="WcfWithDI.Interfaces.INeeded, WcfWithDI" PluggedType="WcfWithDI.Library.Needed, WcfWithDI"/>
  </StructureMap>
```

add it to your configSections of course

```xml
  <configSections>
    <section name="StructureMap" type="StructureMap.Configuration.StructureMapConfigurationSection, StructureMap"/>
  </configSections>
```

The last thing is to initialize structuremap, which we'll do in global.asax in Application_start

     protected void Application_Start(object sender, EventArgs e)
        {
            ObjectFactory.Initialize(x =>
            {
                x.UseDefaultStructureMapConfigFile = false;
                x.PullConfigurationFromAppConfig = true;
            });
        }

And that's it, fire it up.

The web.config is pretty big, If anyone knows lighter ways to configure WCF from there, i'd love to see it.
