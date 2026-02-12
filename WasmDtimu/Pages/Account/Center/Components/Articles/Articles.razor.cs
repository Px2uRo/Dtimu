using System.Collections.Generic;
using WasmDtimu.Models;
using Microsoft.AspNetCore.Components;

namespace WasmDtimu.Pages.Account.Center
{
    public partial class Articles
    {
        [Parameter] public IList<ListItemDataType> List { get; set; }
    }
}