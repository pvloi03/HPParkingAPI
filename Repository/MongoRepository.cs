using Humanizer;
using LearnApi.Data;
using LearnApi.Repository.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LearnApi.Repository;

public class MongoRepository<T> : IRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(MongoDbContext context)
    {
        var collectionName = typeof(T).Name.Pluralize();
        _collection = context.GetCollection<T>(collectionName);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task InsertAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        var result = await _collection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
}