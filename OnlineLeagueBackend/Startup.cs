﻿using System;
using Adapters.Framework.EventStores;
using Adapters.Json.ObjectPersistences;
using Adapters.WebApi.Seasons;
using Application.Framework;
using Application.Seasons;
using Application.Seasons.Querries;
using DependencyInjection.Framework;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace OnlineLeagueBackend
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<SeasonController>();

            services.AddTransient<SeasonCommandHandler>();
            services.AddTransient<AllSeasonsQuery>();
            services.AddTransient<SubscribedEventTypes<AllSeasonsQuery>>();
            services.AddTransient<AllSeasonsQueryEventHandler>();
            services.AddTransient<AllSeasonsCounterQuery>();
            services.AddTransient<SubscribedEventTypes<AllSeasonsCounterQuery>>();
            services.AddTransient<AllSeasonsCounterQueryHandler>();

            services.AddMyEventStoreDependencies(typeof(AllSeasonsCounterQuery).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}