using MongoDB.Bson;
using MongoDB.Driver;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface ISessionService
{
    public Task<Session> CreateNewSession(User user);
    public Task<Session> GetSession(User user);
    public Task<Session> GetSession(string sessionGuid);
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

    public async Task<Session> GetSession(User user)
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
}
