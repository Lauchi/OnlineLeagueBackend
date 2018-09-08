﻿using System.Collections.Generic;
using Application.Framework;
using Domain.Framework;

namespace Adapters.Json.ObjectPersistences
{
    public class DomainEventPersister : JsonFileObjectPersister<IEnumerable<DomainEvent>>, IDomainEventPersister
    {
    }
}