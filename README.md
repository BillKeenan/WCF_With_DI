# Configuring WCF with StructureMap for Dependancy Injection [title]

I’ve read a lot of articles on how to use Dependancy Injection with WCF, and I found most of them to be pretty inscrutable, it really started to seem like an unscalable problem.

After a lot of futzing, a combination of articles lead me to look closer at servicehostfactory, and how it creates the service. Once I found the solution, it's actually pretty simple, and I don't understand why all the examples are so baffling.

## Create a wcf Project [create]
of course, nothing fancy

## Create a class / interface that need to be injected [dependancy]

I've created a class 
    
	Needed.cs

And interface

	iNeeded.cs

I've put a simple method in Needed so we can see it working

	String GetWord();

Here I've done some wiring up in the web.config so we have a JSON getenabled service, have a look for yourself. Fire up debug, and in your browser go to:

*http://localhost:60203/Service1.svc/GetData

Your port will likely be different of course

Inside the ```GetWord``` method I'm simply instantiating Needed and calling GetWord so we can ensure everything is wired up.

## First Commit[firstcommit]

* SHA: 4e70ccb43c017a0cb4bd51f0838460c8b87bab38

## Create a custom Service Host[servicehost]

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

Here you can see I've only implemented it for Service1, which of course wont work in the long run, but at this point we can test that our service works, and that our factory works. Put a breakpoing on the MyInstanceProvider.GetInstance method and fire it up, the result should be the same as before, but you'll see the GetInstance method get called.


