namespace WebDriveDtimu

#nowarn "20"

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open WebDriveDtimu.Models

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let mutable rootPath = Environment.CurrentDirectory

        let builder = WebApplication.CreateBuilder(args)

        let dirSection = builder.Configuration.GetSection("dir")
        if not (isNull dirSection) then
            let v = dirSection["root"]
            if not (String.IsNullOrEmpty v) then
                rootPath <- v
        try
            let index =
                args
                |>Array.toList
                |>List.findIndex (fun x -> x = "--dir")
            let value = args.[index + 1]
            rootPath <- value
        with
        | _ -> ()

        Scanner.scanBegin rootPath |> Async.Start |> ignore


        builder
            .Services
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation()

        builder.Services.AddRazorPages()

        let app = builder.Build()

        if not (builder.Environment.IsDevelopment()) then
            app.UseExceptionHandler("/Home/Error")
            app.UseHsts() |> ignore // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

        app.UseHttpsRedirection()

        app.UseStaticFiles()
        app.UseRouting()
        app.UseAuthorization()

        app.MapControllerRoute(name = "default", pattern = "{controller=Home}/{action=Index}/{id?}")

        app.MapRazorPages()

        app.Run()

        exitCode
