using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Perper.Model;

namespace Apocryph.Dao.Bot.Tests.Fixtures
{
    public class InMemoryState : IState
    {
        private readonly ConcurrentDictionary<string, object> _state = new();
        
        public Task<(bool, T)> TryGetAsync<T>(string key)
        {
            if (_state.TryGetValue(key, out var value))
            {
                return Task.FromResult((true, (T)value));
            }
            
            return Task.FromResult((false, default(T)));
        }

        public Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> defaultValueFactory)
        {
            throw new NotImplementedException();
        }

        public Task SetAsync<T>(string key, T value)
        {
            _state[key] = value;
            
            return Task.CompletedTask;
        }
    }
}