﻿// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Server.Kestrel;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // No scenarios covered by the benchmarks require the HttpContextAccessor so we're replacing it with a
            // no-op version to avoid the cost.
            services.AddSingleton(typeof(IHttpContextAccessor), typeof(InertHttpContextAccessor));
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            var kestrel = app.ServerFeatures[typeof(IKestrelServerInformation)] as IKestrelServerInformation;

            if (kestrel != null)
            {
                // Using an I/O thread for every 2 logical CPUs appears to be a good ratio
                kestrel.ThreadCount = Environment.ProcessorCount >> 1;
                kestrel.NoDelay = true;
            }

            app.UsePlainText();
            app.UseJson();
            app.UseMvc();
        }
        
        public class InertHttpContextAccessor : IHttpContextAccessor
        {
            public HttpContext HttpContext
            {
                get { return null; }
                set { return; }
            }
        }

        public static void Main(string[] args) => Microsoft.AspNet.Hosting.WebApplication.Run(args);
    }
}
