using System.Collections.Concurrent;
using Globomantics.Domain;

namespace Globomantics.Infrastructure.Data.Repositories;

public class TodoInMemoryRepository<T> : IRepository<T>
    where T : Todo
{
    private ConcurrentDictionary<Guid, T> Items { get; } = new();
    
    public Task<T> GetAsync(Guid id) => Task.FromResult(Items[id]);

    public Task<T> FindByAsync(string value)
    {
        var result = Items.Values.First(item => item.Title == value);

        return Task.FromResult(result);
    }

    public Task<IEnumerable<T>> AllAsync()
    {
        var items = Items.Values.ToArray();

        return Task.FromResult<IEnumerable<T>>(items);
    }

    public Task AddAsync(T item)
    {
        Items.TryAdd(item.Id, item);

        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}