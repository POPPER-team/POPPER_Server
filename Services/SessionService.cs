using MongoDB.Bson;
using MongoDB.Driver;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface ISessionService
{
    public Task<string> CreateNewSession(User user);
    public Task<string> GetSessionGuid(User user);
}
public class SessionService : ISessionService
{
    
    private readonly IMongoDatabase _session;
    public SessionService(IMongoDatabase session)
    {
        _session = session;
    }
    public Task<string> CreateNewSession(User user)
    {
        //TODO add session model
    //    var collection = _session.GetCollection<BsonDocument>(); 
        throw new NotImplementedException();
    }

    public Task<string> GetSessionGuid(User user)
    {
        throw new NotImplementedException();
    }
}