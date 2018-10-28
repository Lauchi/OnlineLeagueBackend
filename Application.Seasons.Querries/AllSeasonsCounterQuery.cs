﻿using Application.Framework;
using Domain.Seasons.Events;

namespace Application.Seasons.Querries
{
    public class AllSeasonsCounterQuery : Query, IHandle<SeasonCreatedEvent>
    {
        public long SeasonCounter { get; private set; }

        public void Handle(SeasonCreatedEvent createdEvent)
        {
            SeasonCounter++;
        }
    }
}