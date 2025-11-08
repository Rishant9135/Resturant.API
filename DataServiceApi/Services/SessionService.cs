using BSPL.Domain;
using DataModel;
using DataServiceAPI.Services.Interfaces;

namespace DataServiceAPI.Services
{
    public class SessionService : ISessionService
    {
        private readonly IRepository<UserSessionTbl> _sessionRepo;

        public SessionService(IRepository<UserSessionTbl> sessionRepo)
        {
            _sessionRepo = sessionRepo;
        }

        public void SaveOrUpdateSession(int userId, string username, string token)
        {
            var issuedAtIST = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
            );

            var existingSession = _sessionRepo.ListAll().FirstOrDefault(s => s.UserId == userId);
            if (existingSession != null)
            {
                existingSession.JwtToken = token;
                existingSession.IssuedAt = issuedAtIST;
                _sessionRepo.Update(existingSession);
            }
            else
            {
                _sessionRepo.Insert(new UserSessionTbl
                {
                    UserId = userId,
                    Username = username,
                    JwtToken = token,
                    IssuedAt = issuedAtIST
                });
            }
        }
    }
}
