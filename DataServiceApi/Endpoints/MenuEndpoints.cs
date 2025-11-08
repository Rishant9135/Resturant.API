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
    public static class MenuEndPoints
    {
        public static void MapMenuEndPoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/menu");

            group.MapGet("/", (IReadOnlyRepository<MenuTbl> repo, [FromQuery(Name = "$filter")] string filter) =>
                string.IsNullOrWhiteSpace(filter)
                    ? repo.ListAll()
                    : repo.ListAll(ExpressionEx.ODataFiletrToLinqExpression<MenuTbl>(filter)));

            group.MapPost("/", (IRepository<MenuTbl> repo, MenuTbl menu) => repo.Insert(menu));
            
            group.MapPut("/", (IRepository<MenuTbl> repo, MenuTbl menu) => repo.Update(menu));
            
            group.MapGet("/{id}", (IReadOnlyRepository<MenuTbl> repo, long id) => repo.Get(id));

            group.MapGet("/random", (IReadOnlyRepository<MenuTbl> repo) =>
            {
                var allCategories = repo.ListAll();
                var randomList = allCategories
                                    .OrderBy(x => Guid.NewGuid()) // shuffle
                                    .ToList();
                return randomList;
            });
            group.MapGet("/random/{count}", (IReadOnlyRepository<MenuTbl> repo, int count) =>
            {
                var allCategories = repo.ListAll();
                var randomList = allCategories
                                    .OrderBy(x => Guid.NewGuid()) // shuffle
                                    .Take(count)
                                    .ToList();
                return randomList;
            });
            group.MapGet("/menuList", (IReadOnlyRepository<MenuTbl> repo) =>
            {
                var allCategories = repo.ListAll();
                var randomList = allCategories
                                    .OrderBy(x => Guid.NewGuid()) // shuffle
                                    .ToList();
                return randomList;
            });

            group.MapGet("/withReviews", (
                IReadOnlyRepository<MenuTbl> menuRepo,
                IReadOnlyRepository<MenuCategoriesTbl> categoryRepo,
                IReadOnlyRepository<MenuReviewTbl> reviewRepo) =>
            {
                var menus = menuRepo.ListAll().ToList();
                var categories = categoryRepo.ListAll().ToList();
                var reviews = reviewRepo.ListAll().ToList();

                var result = menus.Select(menu => new MenuWithReviewsModel
                {
                    MenuId = menu.Id,
                    EnglishName = menu.EnglishName,
                    HindiName = menu.HindiName,
                    CategoryEnglishName = categories.FirstOrDefault(c => c.Id == menu.CategoryId)?.EnglishName ?? "",
                    CategoryHindiName = categories.FirstOrDefault(c => c.Id == menu.CategoryId)?.HindiName ?? "",
                    Price = menu.Price,
                    Image = menu.Image,
                    IsAvailable = menu.IsAvailable,
                    IsVeg = menu.IsVeg,

                    AverageRating = reviews.Where(r => r.MenuId == menu.Id).Any()
                        ? reviews.Where(r => r.MenuId == menu.Id).Average(r => r.Rating)
                        : 0,
                    ReviewCount = reviews.Count(r => r.MenuId == menu.Id),
                    Reviews = reviews.Where(r => r.MenuId == menu.Id).ToList()
                }).ToList();

                return Results.Ok(result);
            });
            group.MapGet("/menuListwithPagination", (IReadOnlyRepository<MenuTbl> repo, int pageNumber = 1, int pageSize = 20) =>
            {
                var allMenus = repo.ListAll();

                // Apply pagination
                var pagedData = allMenus
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

                // Return both data and pagination info
                return Results.Ok(new
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = allMenus.Count,
                    TotalPages = (int)Math.Ceiling(allMenus.Count / (double)pageSize),
                    Data = pagedData
                });
            });

        }
    }
}
