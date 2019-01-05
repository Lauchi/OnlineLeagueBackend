<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
**Table of Contents**

- [Microwave](#microwave)
  - [Overview](#overview)
  - [EventStore](#eventstore)
    - [DomainEvents](#domainevents)
    - [Entities](#entities)
  - [Distributed Event Handling](#distributed-event-handling)
    - [EventHandlers](#eventhandlers)
    - [ReadModels](#readmodels)
  - [WebApi](#webapi)
    - [EventStreams](#eventstreams)
  - [Quality of Live Tools](#quality-of-live-tools)
    - [Exceptions](#exceptions)
    - [Result Objects](#result-objects)
    - [WebApi Filters](#webapi-filters)
    - [Identity Handling in WebApi](#identity-handling-in-webapi)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

# Microwave
Microwave is a Framework for Eventsourcing and CQRS that lets you implement your domain and not have you worried about distributing your domain events or storing them.

[![Build status](https://ci.appveyor.com/api/projects/status/sc8x8uaakosryu7c?svg=true)](https://ci.appveyor.com/project/Lauchi/microwave)
[![sonarCubeCoverage](https://sonarcloud.io/api/project_badges/measure?project=Lauchi_Microwave&metric=coverage)](https://sonarcloud.io/dashboard?id=Lauchi_Microwave)


## Overview

### Installation

## EventStore

The `EventStore` is your persistance layer that offers saving DomainEvents and restoring Entities by EventSourcing. There is a SnapShot function for performance reasons, if you need it. Inject the `IEventStore` into your desired class and use the functions to save and load entities. The Eventstore uses optimistic concurrency so you have to give him the version of the entity that you are trying to save.

### DomainEvents and Identities

To append DomainEvents to the `EventStore` you have to implement the `IDomainEvent` interface on your DomainEvents. The interface forces you to implement the property `EntityId` so the eventstore can assign the events to the entity. Everything else is up to your choice. The `EventStore` also generates upcounting versions for the DomainEvents. The Identity class is a base class that has two implementations: `StringIdentity` and `GuidIdentity`. You can create them with a string or an guid on the static create method. Equals and == are overwritten so you can use them just like a string or a guid and the eventstore can handle both of them. I found myself using the `StringIdentity` to give an id in a specific scenario more context.

### IApply/Entities

To Load an entity the Entity has to implement the Interface `IApply` wich takes a list of DomainEvents and forces you to apply them to your entity. There is a class `Entity` that implements the `IApply` method in a way, so the entity applies the DomainEvent to private or public methods that take a single DomainEvent. Reflection is used so you might want to do it on your own, if you run into performance issues. Example:

```
public class User : Entity
{
    public void Apply(UserCreatedEvent domainEvent)
    {
        Id = domainEvent.EntityId;
    }

    private void Apply(UserChangedNameEvent domainEvent)
    {
        Name = domainEvent.Name;
    }

    public Identity Id { get; private set; }
    public string Name { get; private set; }
}

// OR with IApply

public class User2 : IApply
{
    public void Apply(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            switch (domainEvent)
            {
                case UserCreatedEvent ev: Apply(ev);
                case UserChangedNameEvent ev: Apply(ev);

            }
        }
    }

    public void Apply(UserCreatedEvent domainEvent)
    {
        Id = domainEvent.EntityId;
    }

    public void Apply(UserChangedNameEvent domainEvent)
    {
        Name = domainEvent.Name;
    }

    public Identity Id { get; private set; }
    public string Name { get; private set; }
}
```

### Snapshots

The EventStore supports snapshots that lets you save the state of an entity after a certain times of loading it, so the eventstore does not need to apply too much events at once. The eventstore first loads the snapshot and then applies the remaining events on it. The snapshots only get created when the entity is being loaded, so if you never load the entity, the snapshot is not created on the given threshold. To setup an Entity for Snapshots, just add the Attribute over the class like this:

```
[SnapShotAfter(50)] // entity is being snapshotted after 50 events
public class User : Entity
{
    ...
}
```

## Distributed Event Handling

Microwave supports distributed systems and you can subscribe to events from any service with just implementing an interface. Microwave will get the events, put them in the handling classes and saves the location where you left of, in case a service is unavailable or an error occurs during the event handling on the client side.

### Reacting to Events happening in the Domain

If you want to react to DomainEvents in your Domain, to trigger other processes, you can do this by implementing the `IHandleAsync` interface in a class. You will have to implement a method that will receive the event as soon as it happens in the Domain. This all happens asynchronouse, so the emitting thread is not blocked by your handling. A handler could look like this:

```
public class WelcomeMailHandler : IHandleAsync<UserCreatedEvent>
{
    private readonly IMailRepository _mailRepository;

    public WelcomeMailHandler(IMailRepository mailRepository)
    {
        _mailRepository = mailRepository;
    }

    public Task HandleAsync(UserCreatedEvent domainEvent)
    {
        await _mailRepository.SendWelcomeMail(domainEvent.UserId);
    }
}
```

Using the `IHandleAsync` is usually useful when updating your write or read model when something happens in another entity that you do not want to reference directly.

### ReadModels and Querries

Microwave has ReadModels and Querries that can be used to create a querry service that is independent from the write side and therefore can be run in parallel. There are two main differences between Querries and ReadModels: ReadModels can be retrieved with and Identity and Querries are retrieved by their classname. For example you can have a ReadModel that represents a UserReadModel that can be retrieved by the UserId and a Querry could be the Top 10 Most Active Users in the Domain.

To add a Querry, inherit from `Querry` and implement the `IHandle` Interface to register the Querry to the specific DomainEvent. A Querry could look like this:

```
public class UserCounterQuerry : Querry, IHandle<UserCreatedEvent>
{
    public int Count { get; private set; }

    public void Handle(UserCreatedEvent domainEvent)
    {
        Count++;
    }
}
```

To add a ReadModel inherit from `ReadModel` and also implement `IHandle` accordingly. The only difference is that you have to add an attribute to the ReadModel that says on what DomainEvent Microwave should create this ReadModel. Afte the creation Mircowave tracks the ReadModel and updates it when new events emerge. The Readmodel also has a Version that is being updated alongside with the write side, so you can cal the write side with the eventual consistent version. A ReadModel could look like this:

```
[CreateReadmodelOn(typeof(UserCreatedEvent))]
public class UserReadModel : ReadModel, IHandle<UserCreatedEvent>, IHandle<UserChangedNameEvent>
{
    public void Handle(UserCreatedEvent domainEvent)
    {
        Id = domainEvent.EntityId;
    }

    public void Apply(UserChangedNameEvent domainEvent)
    {
        Name = domainEvent.Name;
    }

    public Identity Id { get; private set; }
    public string Name { get; private set; }
}
```

#### Loading Querries and Readmodels
To load the Querries and ReadModels there are two Repositories `IQuerryRepository` and `IReadModelRepository` that offer functionality to load and update Querries/Readmodels. You can use them to update Querries or Readmodels inside a IHandleAsync by yourself, if you need to.

### Setting up the event source

The location for Events can be defined in the appsettings.json, so you can quickly move adjust the source for your events if you move entities aroung. For Microwave it does not matter where the events are stored, it always gets the from the webapi provided and applies them to the local handlers or ReadModels. The appsettings has to contain the following definitions:

```
{
  "DefaultDomainEventLocation": "http://localhost:5000/",  --> this is mandatory (will throw exception when not present)
  "DomainEventLocations": {
    "UserCreatedEvent" : "http://localhost:123/"           --> tells Mircowave to get the event from this location for handlers and Querries
  },
  "DomainEventReadModelLocations": {
    "UserReadModel" : "http://localhost:123/"              --> tells Mircowave to get the all events for the readmodel from this location
  }
}
```

The Locations are optional, if they are not provided, Microwave will try to get the events from the `DefaultDomainEventLocation`

## WebApi

### EventStreams

## Quality of Live Tools

### Exceptions

### Result Objects

### WebApi Filters

### Identity Handling in WebApi

## Known Issues

### String and GuidIdentity in Parameters