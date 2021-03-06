using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microwave.Domain.EventSourcing;
using MongoDB.Bson.Serialization;

namespace Microwave.Persistence.MongoDb.Eventstores
{
    public static class BsonMapRegistrationHelpers
    {
        public static void AddBsonMapsForMicrowave(Assembly assembly)
        {
            var domainEventTypes = assembly.GetTypes().Where(ev => ev.GetInterfaces().Contains(typeof(IDomainEvent)));
            var addBsonMapGeneric = typeof(BsonMapRegistrationHelpers).GetMethod(nameof(AddBsonMapFor),
                BindingFlags.Public | BindingFlags.Static);

            foreach (var domainEventType in domainEventTypes)
            {
                var addBsonMap = addBsonMapGeneric?.MakeGenericMethod(domainEventType);
                addBsonMap?.Invoke(null, new object[] { });
            }
        }

        public static void AddBsonMapFor<T>() where T : IDomainEvent
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
                BsonClassMap.RegisterClassMap<T>(map =>
                {
                    var eventType = typeof(T);
                    var lambdaParameter = Expression.Parameter(eventType, "domainEvent");

                    var constructorInfo = GetConstructorWithMostMatchingParameters(eventType);
                    MapProperties(eventType.GetProperties(), map);

                    var expressions = CreateExpressionsFromConstructorParameters(constructorInfo, lambdaParameter);

                    var body = Expression.New(constructorInfo, expressions);

                    var funcType = typeof(Func<,>).MakeGenericType(eventType, eventType);
                    var lambda = Expression.Lambda(funcType, body, lambdaParameter);

                    var expressionType = typeof(Expression<>).MakeGenericType(funcType);
                    var genericClassMap = typeof(BsonClassMap<>).MakeGenericType(eventType);
                    var mapCreatorFunction = genericClassMap.GetMethod(nameof(BsonClassMap.MapCreator), new[]
                    {
                        expressionType
                    });
                    mapCreatorFunction?.Invoke(map, new object[] { lambda });
                });
        }

        private static IEnumerable<Expression> CreateExpressionsFromConstructorParameters(
            ConstructorInfo constructorInfo,
            ParameterExpression lambdaParameter)
        {
            return constructorInfo.GetParameters().Select(parameter => ExtractExpression(lambdaParameter, parameter));
        }

        private static void MapProperties<T>(PropertyInfo[] propertyInfos, BsonClassMap<T> c) where T : IDomainEvent
        {
            foreach (var property in propertyInfos) c.MapProperty(property.Name);
        }

        private static Expression ExtractExpression(ParameterExpression lambdaParameter, ParameterInfo parameter)
        {
            return Expression.Property(lambdaParameter, parameter.Name);
        }

        private static ConstructorInfo GetConstructorWithMostMatchingParameters(Type eventType)
        {
            var constructors = eventType.GetConstructors();
            var maxParams = constructors.Max(c => c.GetParameters().Length);
            return constructors.FirstOrDefault(c => c.GetParameters().Length == maxParams);
        }
    }
}