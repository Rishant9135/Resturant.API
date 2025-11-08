using BSPL.Domain;
using ElectionData.Common.Models;
using System;
using System.Collections.Generic;
using BSPL.Facades;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSPL.Domain;
using DataModel;
using Microsoft.AspNetCore.Mvc.Razor;

namespace DataService.Common.Facade
{
    public class MenuCategoryFacade :Facade<MenuCategoriesTbl, MenuCategoriesTbl>
    {
        private readonly IReadOnlyRepository<MenuCategoriesTbl> menuCategoryRepository;
        public MenuCategoryFacade(IReadOnlyRepository<MenuCategoriesTbl> menuCategoryRepository) : base(menuCategoryRepository)
        {
            this.menuCategoryRepository = menuCategoryRepository;
        }
    }
}
