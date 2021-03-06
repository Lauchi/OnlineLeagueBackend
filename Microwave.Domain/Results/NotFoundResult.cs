﻿using System;
using Microwave.Domain.Exceptions;

namespace Microwave.Domain.Results
{
    public class NotFoundResult<T> : Result<T>
    {
        public NotFoundResult(string notFoundId) : base(new NotFound(typeof(T), notFoundId))
        {
        }
    }

    public class NotFound : ResultStatus
    {
        public Type Type { get; }
        public string NotFoundId { get; }

        public NotFound(Type type, string notFoundId)
        {
            Type = type;
            NotFoundId = notFoundId;
        }

        public override void Check()
        {
            throw new NotFoundException(Type, NotFoundId ?? "null");
        }
    }
}