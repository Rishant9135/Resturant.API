using BSPL.Domain;
using DataModel;
using ElectionData.Common.Facade;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc;
using VmsDataApi.Utils;
using DataServiceAPI.Services;
using DataServiceAPI.Endpoints;
using DataServiceAPI.Common.Models;
using DataServiceAPI.Extensions;

namespace VmsDataApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            builder.Services.AddJwtAuthentication(builder.Configuration);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

            builder.Services.AddLinqToDBContext<DataConnection, APIDB>((provider, options) => options.UseSqlServer(connectionString).UseDefaultLogging(provider));
            
            //**********************Add your Project Scope here*********************************
            builder.Services.AddProjectScopes();
            
            var app = builder.Build();
            app.MapGet("/", () => "Hello!!!");

            //**********************Map Your End Points here*********************************
            app.MapUserEndpoints();
            app.MapMenuCategoriesEndpoints();
            app.MapMenuEndPoints();
            app.MapMenuReviewEndpoints();

            app.Run();
        }
    }
}

