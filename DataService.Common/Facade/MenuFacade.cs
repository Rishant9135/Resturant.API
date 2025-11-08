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
    public class MenuFacade :Facade<MenuTbl, MenuTbl>
    {
        private readonly IReadOnlyRepository<MenuTbl> menuRepository;
        public MenuFacade(IReadOnlyRepository<MenuTbl> menuRepository) : base(menuRepository)
        {
            this.menuRepository = menuRepository;
        }
    }
}
