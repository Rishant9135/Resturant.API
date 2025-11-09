using BSPL.Domain;
using DataModel;
using DataServiceAPI.Common.Models;
using DataServiceAPI.Services;
using ElectionData.Common.Facade;
using ElectionData.Common.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using VmsDataApi.Utils;

namespace DataServiceAPI.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/user");

            group.MapGet("/", (IReadOnlyRepository<UserTable> repo, [FromQuery(Name = "$filter")] string filter) =>
                string.IsNullOrWhiteSpace(filter)
                    ? repo.ListAll()
                    : repo.ListAll(ExpressionEx.ODataFiletrToLinqExpression<UserTable>(filter)));

            group.MapPost("/", (IRepository<UserTable> repo, UserTable user) => repo.Insert(user));
            
            group.MapPut("/", (IRepository<UserTable> repo, UserTable user) => repo.Update(user));
            
            group.MapGet("/{id}", (IReadOnlyRepository<UserTable> repo, long id) => repo.Get(id));
            
            //group.MapPost("/login", (
            //    [FromBody] LoginRequestModel request,
            //    UserFacade userFacade) =>
            //    {
            //        var isAuthenticated = userFacade.AuthenticateUser(request.Username, request.Password);
            //        return Results.Ok(isAuthenticated);
            //    });

            //group.MapPost("/Jwt/login", (
            //    [FromBody] LoginRequestModel request,
            //    UserFacade userFacade,
            //    JwtTokenService tokenService) =>
            //    {
            //        var user = userFacade.AuthenticateUserJwt(request.Username, request.Password);

            //        if (user == null)
            //            return Results.Unauthorized();

            //        var token = tokenService.GenerateToken(user.Username);
            //        return Results.Ok(new { token });
            //    });

            group.MapPost("/Jwt/login", (
                [FromBody] LoginRequestModel request,
                UserFacade userFacade,
                JwtTokenService tokenService,
                IRepository<UserSessionTbl> sessionRepo) =>
                {
                    var user = userFacade.AuthenticateUserJwt(request.Username, request.Password);
                    if (user == null)
                        //return Results.Unauthorized();
                        return Results.Json(new { message = "Invalid username or password" }, statusCode: 401);

                    var token = tokenService.GenerateToken(user.Phone);


                    // Check if session exists
                    var existingSession = sessionRepo.ListAll().FirstOrDefault(s => s.UserId == user.Id);
                    var issuedAtIST = TimeZoneInfo.ConvertTimeFromUtc(
                                            DateTime.UtcNow,
                                            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                                        );
                    if (existingSession != null)
                    {
                        existingSession.JwtToken = token;
                        existingSession.IssuedAt = issuedAtIST;
                        sessionRepo.Update(existingSession);
                    }
                    else
                    {
                        sessionRepo.Insert(new UserSessionTbl
                        {
                            UserId = (int)user.Id,
                            Username = user.Phone,
                            JwtToken = token,
                            IssuedAt = issuedAtIST
                        });
                    }

                    return Results.Ok(new
                    {
                        token,
                        userId = user.Id
                    });
                });

            group.MapPost("/Jwt/validate", (
                [FromBody] TokenValidationRequest request,
                JwtTokenService tokenService,
                IReadOnlyRepository<UserSessionTbl> sessionRepo) =>
                {
                    // 1️⃣ Check if the session exists for the userId
                    var session = sessionRepo.ListAll().FirstOrDefault(s => s.UserId == request.UserId);
                    if (session == null)
                    {
                        return Results.Json(new { isValid = false, message = "Session not found" }, statusCode: 401);
                    }

                    // 2️⃣ Check if the token matches the stored token
                    if (session.JwtToken != request.Token)
                    {
                        return Results.Json(new { isValid = false, message = "Token mismatch" }, statusCode: 401);
                    }

                    // 3️⃣ Verify token cryptographically using JwtTokenService
                    var isTokenValid = tokenService.ValidateToken(request.Token, out var username);
                    if (!isTokenValid)
                    {
                        return Results.Json(new { isValid = false, message = "Invalid or expired token" }, statusCode: 401);
                    }

                    // 4️⃣ Optional: Check if token’s username matches stored username
                    if (!string.Equals(session.Username, username, StringComparison.OrdinalIgnoreCase))
                    {
                        return Results.Json(new { isValid = false, message = "Token does not match user" }, statusCode: 401);
                    }

                    // ✅ Token valid
                    return Results.Ok(new { isValid = true, username });
                });
            group.MapPost("/register", (
                [FromBody] UserRegisterModel request,
                UserFacade userFacade) =>
                    {
                        var user = new UserTable
                        {
                            Phone = request.Phone,
                            Email = request.Email,
                            PasswordHash = request.Password // raw password (will be hashed in Facade)
                        };

                        var result = userFacade.RegisterUser(user);

                        if (!result.IsSuccess)
                            return Results.Json(new { success = false, message = result.Message }, statusCode: 400);

                        return Results.Ok(new { success = true, message = result.Message });
                    });




        }
    }
}
