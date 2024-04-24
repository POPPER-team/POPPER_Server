using MongoDB.Bson;
using MongoDB.Driver;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface ISessionService
{
    public Task<Session> CreateNewSession(User user);
    public Task<Session> GetOrCreateSession(User user);
    public Task<Session> GetSession(string sessionGuid);
    public Task<Session> UpdateText(Session session, string text);
}

public class SessionService : ISessionService
{
    private readonly IMongoCollection<Session> _sessions;

    public SessionService(IMongoDatabase session)
    {
        _sessions = session.GetCollection<Session>("Sessions");
    }

    public async Task<Session> CreateNewSession(User user)
    {
        Session newSession = new Session()
        {
            UserGuid = user.Guid
        };
        await _sessions.InsertOneAsync(newSession);
        return newSession;
    }

    public async Task<Session> GetOrCreateSession(User user)
    {
        var filter = Builders<Session>.Filter.Eq(s => s.UserGuid, user.Guid);
        Session session = await _sessions.Find(filter).FirstOrDefaultAsync();
        if (session == null) return await CreateNewSession(user);
        return session;
    }

    public async Task<Session> GetSession(string sessionGuid)
    {
        var filter = Builders<Session>.Filter.Eq(s => s.SessionGuid,sessionGuid);
        Session session = await _sessions.Find(filter).FirstOrDefaultAsync();
        return session;
    }

    //TODO Any new field should have its own method to update 
    public async Task<Session> UpdateText(Session session, string text)
    {
        var filter = Builders<Session>.Filter.Eq(s => s.SessionGuid,session.SessionGuid);
        var update = Builders<Session>.Update.Set(s => s.text ,text);
        await _sessions.UpdateOneAsync(filter,update);
        return session;
    }
}
