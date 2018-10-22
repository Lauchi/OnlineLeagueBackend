﻿using Adapters.Framework.WebApi;
using Adapters.WebApi.Seasons;
using Application.Framework;
using Application.Seasons;
using Application.Seasons.Querries;
using DependencyInjection.Framework;
using Domain.Seasons.Events;
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
            services.AddTransient<StartEventUpdateController>();
            services.AddTransient<DomainEventController>();

            services.AddTransient<SeasonCommandHandler>();
            services.AddTransient<AllSeasonsQuery>();
            services.AddTransient<SubscribedEventTypes<AllSeasonsQuery>>();
            services.AddTransient<AllSeasonsQueryEventHandler>();
            services.AddTransient<AllSeasonsCounterQuery>();
            services.AddTransient<SubscribedEventTypes<AllSeasonsCounterQuery>>();
            services.AddTransient<AllSeasonsCounterQueryHandler>();

            services.AddTransient<IEventDelegateHandler, EventDelegateHandler<SeasonCreatedEvent>>();
            services.AddTransient<IEventDelegateHandler, EventDelegateHandler<SeasonNameChangedEvent>>();

            services.AddTransient<IHandleAsync<SeasonCreatedEvent>, SeasonCreatedEventHandler>();
            services.AddTransient<IHandleAsync<SeasonNameChangedEvent>, SeasonChangedNamEventHandler>();
            services.AddTransient<IHandleAsync<SeasonNameChangedEvent>, SeasonCreatedEventHandler>();

            services.AddTransient<IPublishedEventStream<SeasonCreatedEvent>, PublishedEventStream<SeasonCreatedEvent>>();
            services.AddTransient<IPublishedEventStream<SeasonNameChangedEvent>, PublishedEventStream<SeasonNameChangedEvent>>();

            services.AddTransient<DomainEventClient<SeasonCreatedEvent>>();
            services.AddTransient<DomainEventClient<SeasonNameChangedEvent>>();


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