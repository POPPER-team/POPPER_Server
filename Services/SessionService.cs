using MongoDB.Driver;
using POPPER_Server.Helpers;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface ISessionService
{
    public Task<string> GetOrCreateSession(User user);
    public Task<Session> GetSessionAsync(string sessionGuid);
    public Task<string> UpdateText(string sessionGuid, string text);
}

public class SessionService : ISessionService
{
    private readonly IMongoDatabase _mongoDB;
    private const string SessionDatabaseName ="Sessions";
    public SessionService(IMongoDatabase session)
    {
        _mongoDB = session;
    }

   private async Task<Session> CreateNewSession(User user)
    {
        Session newSession = new Session()
        {
            UserGuid = user.Guid
        };
        await _mongoDB.GetCollection<Session>(SessionDatabaseName).InsertOneAsync(newSession);
        return newSession;
    }

    public async Task<string> GetOrCreateSession(User user)
    {
        FilterDefinition<Session> filter = Builders<Session>.Filter.Eq(s => s.UserGuid, user.Guid);
        Session session = await _mongoDB.GetCollection<Session>(SessionDatabaseName).Find(filter).FirstOrDefaultAsync();
        if (session == null) return (await CreateNewSession(user)).SessionGuid;
        return session.SessionGuid;
    }

    public async Task<Session> GetSessionAsync(string sessionGuid)
    {
        FilterDefinition<Session> filter = Builders<Session>.Filter.Eq(s => s.SessionGuid,sessionGuid);
        Session session = await _mongoDB.GetCollection<Session>(SessionDatabaseName).Find(filter).FirstOrDefaultAsync();
        return session;
    }

    //TODO Any new field should have its own method to update 
    public async Task<string> UpdateText(string sessionGuid, string text)
    {
        FilterDefinition<Session> filter = Builders<Session>.Filter.Eq(s => s.SessionGuid,sessionGuid);
        UpdateDefinition<Session> update = Builders<Session>.Update.Set(s => s.text ,text);
        await _mongoDB.GetCollection<Session>(SessionDatabaseName).UpdateOneAsync(filter,update);
        Session session = await GetSessionAsync(sessionGuid);
        return session.text;
    }
}
