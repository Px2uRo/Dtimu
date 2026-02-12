namespace WebDriveDtimu.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open System.Diagnostics

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open WebDriveDtimu.Models

open WebDriveDtimu.Models

[<Route("/[controller]")>]
type APIController(logger: ILogger<APIController>) =
    inherit ControllerBase() // ControllerBase 更适合 API

    [<HttpGet("vediolist")>]
    member this.GetVedioList() : IActionResult =
        this.Ok(Scanner.VedioPaths)


