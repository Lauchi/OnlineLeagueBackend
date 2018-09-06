﻿using System;
using System.Threading.Tasks;
using Application.Seasons;
using Application.Seasons.Commands;
using Application.Seasons.Querries;
using Microsoft.AspNetCore.Mvc;

namespace Adapters.WebApi.Seasons
{
    [Route("Api/Seasons")]
    public class SeasonController : Controller
    {
        private readonly SeasonCommandHandler _commandHandler;
        private readonly AllSeasonsQueryHandler _queryHandler;

        public SeasonController(SeasonCommandHandler commandHandler, AllSeasonsQueryHandler queryHandler)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }

        [HttpGet]
        public ActionResult GetSeasons()
        {
            var seasons = _queryHandler.GetAllSeasons();
            return Ok(seasons);
        }

        [HttpGet("{entityId}")]
        public ActionResult GetSeason(Guid id)
        {
            var seasons = _queryHandler.GetSeason(id);
            return Ok(seasons);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateSeason([FromBody] CreateSesonCommand command)
        {
            await _commandHandler.CreateSeason(command);
            return Ok();
        }

        [HttpPut("{entityId}/ChangeDate")]
        public async Task<IActionResult> ChangeDate(Guid entityId,[FromBody] ChangeDateApiCommand command)
        {
            var changeDateCommand = new ChangeDateCommand
            {
                EntityId = entityId,
                StartDate = command.StartDate,
                EndDate = command.EndDate
            };

            await _commandHandler.SetStartAndEndDate(changeDateCommand);
            return Ok();
        }

        [HttpPut("{entityId}/ChangeName")]
        public async Task<IActionResult> ChangeName(Guid entityId,[FromBody] ChangeNameApiCommand command)
        {
            var changeDateCommand = new ChangeNameCommand
            {
                EntityId = entityId,
                Name = command.Name
            };

            await _commandHandler.ChangeName(changeDateCommand);
            return Ok();
        }
    }

    public class ChangeNameApiCommand
    {
        public string Name { get; set; }
    }

    public class ChangeDateApiCommand
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}