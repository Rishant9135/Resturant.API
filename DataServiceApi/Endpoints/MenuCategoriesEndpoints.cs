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
    public static class MenuCategoriesEndpoints
    {
        public static void MapMenuCategoriesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/category");

            group.MapGet("/", (IReadOnlyRepository<MenuCategoriesTbl> repo, [FromQuery(Name = "$filter")] string filter) =>
                string.IsNullOrWhiteSpace(filter)
                    ? repo.ListAll()
                    : repo.ListAll(ExpressionEx.ODataFiletrToLinqExpression<MenuCategoriesTbl>(filter)));

            group.MapPost("/", (IRepository<MenuCategoriesTbl> repo, MenuCategoriesTbl category) => repo.Insert(category));

            group.MapPut("/", (IRepository<MenuCategoriesTbl> repo, MenuCategoriesTbl category) => repo.Update(category));

            group.MapGet("/{id}", (IReadOnlyRepository<MenuCategoriesTbl> repo, long id) => repo.Get(id));
            
            group.MapGet("/random", (IReadOnlyRepository<MenuCategoriesTbl> repo) =>
            {
                var allCategories = repo.ListAll();
                var randomList = allCategories
                                    .OrderBy(x => Guid.NewGuid()) // shuffle
                                    .ToList();
                return randomList;
            });
            group.MapGet("/random/{count}", (IReadOnlyRepository<MenuCategoriesTbl> repo, int count) =>
            {
                var allCategories = repo.ListAll();
                var randomList = allCategories
                                    .OrderBy(x => Guid.NewGuid()) // shuffle
                                    .Take(count)
                                    .ToList();
                return randomList;
            });
        }
    }
}
