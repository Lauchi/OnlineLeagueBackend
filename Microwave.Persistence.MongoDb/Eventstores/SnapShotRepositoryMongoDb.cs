﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microwave.EventStores.Ports;
using MongoDB.Driver;

namespace Microwave.Persistence.MongoDb.Eventstores
{
    public class SnapShotRepositoryMongoDb : ISnapShotRepository
    {
        private readonly IMongoDatabase _context;
        private readonly string _snapShotCollectionName = "SnapShotDbos";

        public SnapShotRepositoryMongoDb(MicrowaveMongoDb context)
        {
            _context = context.Database;
        }

        public async Task<SnapShotResult<T>> LoadSnapShot<T>(string entityId) where T : new()
        {
            if (entityId == null) return SnapShotResult<T>.NotFound(null);
            var mongoCollection = _context.GetCollection<SnapShotDbo<T>>(_snapShotCollectionName);
            var asyncCursor = await mongoCollection.FindAsync(r => r.Id == entityId);
            var snapShot = asyncCursor.ToList().FirstOrDefault();

            if (snapShot == null) return SnapShotResult<T>.Default();
            return new SnapShotResult<T>(snapShot.Payload, snapShot.Version);
        }

        public async Task SaveSnapShot<T>(SnapShotWrapper<T> snapShot)
        {
            var mongoCollection = _context.GetCollection<SnapShotDbo<T>>(_snapShotCollectionName);

            var findOneAndReplaceOptions = new FindOneAndReplaceOptions<SnapShotDbo<T>>();
            findOneAndReplaceOptions.IsUpsert = true;
            await mongoCollection.FindOneAndReplaceAsync(
                (Expression<Func<SnapShotDbo<T>, bool>>) (e => e.Id == snapShot.Id),
                new SnapShotDbo<T>
                {
                    Id = snapShot.Id,
                    Version = snapShot.Version,
                    Payload = snapShot.Entity
                }, findOneAndReplaceOptions);
        }
    }
}