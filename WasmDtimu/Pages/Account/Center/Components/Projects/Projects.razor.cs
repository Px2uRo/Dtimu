using System.Collections.Generic;
using WasmDtimu.Models;
using Microsoft.AspNetCore.Components;
using AntDesign;

namespace WasmDtimu.Pages.Account.Center
{
    public partial class Projects
    {
        private readonly ListGridType _listGridType = new ListGridType
        {
            Gutter = 24,
            Column = 4
        };

        [Parameter]
        public IList<ListItemDataType> List { get; set; }
    }
}