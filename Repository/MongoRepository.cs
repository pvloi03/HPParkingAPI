using System.Linq.Expressions;
using Humanizer;
using HPParkingAPI.Data;
using HPParkingAPI.Repository.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HPParkingAPI.Repository;

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

    public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(predicate).FirstOrDefaultAsync();
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(predicate).ToListAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(predicate).AnyAsync();
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

    public async Task<bool> UpdateAsync(T entity)
    {
        // Doc Id tu property "Id" cua entity (la string ObjectId)
        var idProp = typeof(T).GetProperty("Id");
        if (idProp is null) return false;

        var idValue = idProp.GetValue(entity)?.ToString();
        if (string.IsNullOrEmpty(idValue)) return false;

        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(idValue));
        var result = await _collection.ReplaceOneAsync(filter, entity);
        return result.ModifiedCount > 0;
    }
}