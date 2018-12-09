﻿namespace Microwave.Application.Results
{
    public abstract class Result
    {
        protected ResultStatus Status { get; }

        protected Result(ResultStatus status)
        {
            Status = status;
        }

        public static Result ConcurrencyResult(long expectedVersion, long actualVersion)
        {
            return new ConcurrencyErrorResult(expectedVersion, actualVersion);
        }

        public static Result Ok()
        {
            return new OkResult();
        }

        public bool Is<T>() where T : Result
        {
            return typeof(T) == Status.GetType();
        }

        public void Check()
        {
            Status.Check();
        }
    }

    public abstract class ResultStatus
    {
        public abstract void Check();
    }

    public abstract class Result<T>
    {
        private T _value;
        protected ResultStatus Status { get; }

        protected Result(ResultStatus status)
        {
            Status = status;
        }

        public T Value
        {
            get
            {
                Status.Check();
                return _value;
            }
            protected set => _value = value;
        }

        public static Result<T> Ok(T value)
        {
            return new Ok<T>(value);
        }

        public bool Is<TCheck>() where TCheck : ResultStatus
        {
            return typeof(TCheck) == Status.GetType();
        }

        public static Result<T> NotFound(string notFoundId)
        {
            return new NotFoundResult<T>(notFoundId);
        }
    }


}