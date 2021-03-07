﻿using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodexr.Shared;
using Nodexr.Shared.Services;
using Blazored;
using Blazored.Modal;
using Blazored.Toast;
using Blazored.Toast.Services;
using Blazored.Modal.Services;

namespace Nodexr
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBlazoredToast();
            builder.Services.AddTransient(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<INodeDragService, NodeDragService>();
            builder.Services.AddScoped<INoodleDragService, NoodleDragService>();
            builder.Services.AddScoped<INodeHandler, NodeHandler>();
            builder.Services.AddScoped<RegexReplaceHandler>();
            builder.Services.AddBlazoredModal();

            await builder.Build().RunAsync().ConfigureAwait(false);
        }
    }
}
