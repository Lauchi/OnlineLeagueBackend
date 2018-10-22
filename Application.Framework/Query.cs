﻿using System;
using System.Linq;
using Domain.Framework;

namespace Application.Framework
{
    public class CustomQuery
    {
        public long Version { get; private set;  }

        public void Handle(DomainEvent domainEvent)
        {
            var type = domainEvent.GetType();
            var currentEntityType = GetType();
            var methodInfos = currentEntityType.GetMethods().Where(method => method.Name == nameof(Handle));
            var methodToExecute = methodInfos.FirstOrDefault(method => method.GetParameters().FirstOrDefault()?.ParameterType == type);
            if (methodToExecute == null || methodToExecute.GetParameters().Length != 1) return;
            methodToExecute.Invoke(this, new object[] {domainEvent});
            Version = Version + 1;
        }
    }

    public class Query : CustomQuery
    {
    }

    public class IdentifiableQuery : Query
    {
        public Guid Id { get; }
    }
}