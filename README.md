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

Here I've done some wiring up in the web.config so we have a JSON getenabled service, have a look for yourself.

Inside the ```GetWord``` method I'm simply instantiating Needed and calling GetWord so we can ensure everything is wired up.



