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
    public static class MenuReviewEndpoints
    {
        public static void MapMenuReviewEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/menuReview");

            group.MapGet("/", (IReadOnlyRepository<MenuReviewTbl> repo, [FromQuery(Name = "$filter")] string filter) =>
                string.IsNullOrWhiteSpace(filter)
                    ? repo.ListAll()
                    : repo.ListAll(ExpressionEx.ODataFiletrToLinqExpression<MenuReviewTbl>(filter)));

            group.MapPost("/", (IRepository<MenuReviewTbl> repo, MenuReviewTbl menu) => repo.Insert(menu));
            
            group.MapPut("/", (IRepository<MenuReviewTbl> repo, MenuReviewTbl menu) => repo.Update(menu));
            
            group.MapGet("/{id}", (IReadOnlyRepository<MenuReviewTbl> repo, long id) => repo.Get(id));

            group.MapGet("/random", (IReadOnlyRepository<MenuReviewTbl> repo) =>
            {
                var allCategories = repo.ListAll();
                var randomList = allCategories
                                    .OrderBy(x => Guid.NewGuid()) // shuffle
                                    .ToList();
                return randomList;
            });
            group.MapGet("/random/{count}", (IReadOnlyRepository<MenuReviewTbl> repo, int count) =>
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
