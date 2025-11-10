using BSPL.Domain;
using DataModel;
using DataService.Common.Facade;
using ElectionData.Common.Facade;

namespace DataServiceAPI.Services
{
    public static class ServiceRegistration
    {
        public static void AddProjectScopes(this IServiceCollection services)
        {
            // 🧑 User-related repositories
            services.AddScoped<IReadOnlyRepository<UserTable>, ReadOnlyRepository<UserTable>>();
            services.AddScoped<IRepository<UserTable>, Repository<UserTable>>();

            services.AddScoped<IReadOnlyRepository<UserSessionTbl>, ReadOnlyRepository<UserSessionTbl>>();
            services.AddScoped<IRepository<UserSessionTbl>, Repository<UserSessionTbl>>();

            services.AddScoped<IReadOnlyRepository<MenuCategoriesTbl>, ReadOnlyRepository<MenuCategoriesTbl>>();
            services.AddScoped<IRepository<MenuCategoriesTbl>, Repository<MenuCategoriesTbl>>();

            services.AddScoped<IReadOnlyRepository<MenuTbl>, ReadOnlyRepository<MenuTbl>>();
            services.AddScoped<IRepository<MenuTbl>, Repository<MenuTbl>>();

            services.AddScoped<IReadOnlyRepository<MenuReviewTbl>, ReadOnlyRepository<MenuReviewTbl>>();
            services.AddScoped<IRepository<MenuReviewTbl>, Repository<MenuReviewTbl>>();

            services.AddScoped<IReadOnlyRepository<PasswordTbl>, ReadOnlyRepository<PasswordTbl>>();
            services.AddScoped<IRepository<PasswordTbl>, Repository<PasswordTbl>>();

            services.AddScoped<IReadOnlyRepository<LoginHistoryTbl>, ReadOnlyRepository<LoginHistoryTbl>>();
            services.AddScoped<IRepository<LoginHistoryTbl>, Repository<LoginHistoryTbl>>();

            services.AddScoped<UserFacade>();
            services.AddScoped<MenuCategoryFacade>();
            services.AddScoped<MenuFacade>();
            services.AddScoped<JwtTokenService>();


            // 🧩 Add more table-related DI registrations here...
        }
    }
}
