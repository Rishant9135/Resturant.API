namespace DataServiceAPI.Services.Interfaces
{
    public interface ISessionService
    {
        void SaveOrUpdateSession(int userId, string username, string token);
    }
}
