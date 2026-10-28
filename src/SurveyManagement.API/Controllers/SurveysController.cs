using MediatR;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.Surveys.Commands.CreateSurvey;
using SurveyManagement.Application.Surveys.Queries.GetSurveyById;
using SurveyManagement.Application.Surveys.Queries.GetSurveys;

namespace SurveyManagement.API.Controllers;

[ApiController]
[Route("api/surveys")]
public class SurveysController : ControllerBase
{
    private readonly IMediator _mediator;

    public SurveysController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateSurveyCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<SurveySummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var surveys = await _mediator.Send(new GetSurveysQuery(), cancellationToken);
        return Ok(surveys);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SurveyDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var survey = await _mediator.Send(new GetSurveyByIdQuery(id), cancellationToken);
        return Ok(survey);
    }
}
