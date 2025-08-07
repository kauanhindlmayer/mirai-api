using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using Application.Abstractions;
using Application.WorkItems.Queries.Common;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace Application.WisdomExtractor.Queries.ExtractWisdom;

internal sealed partial class ExtractWisdomQueryHandler
    : IRequestHandler<ExtractWisdomQuery, ErrorOr<WisdomResponse>>
{
    private const double MaxDistance = 0.6;
    private const int MaxItems = 4;
    private readonly INlpService _nlpService;
    private readonly IApplicationDbContext _context;

    public ExtractWisdomQueryHandler(
        INlpService nlpService,
        IApplicationDbContext context)
    {
        _nlpService = nlpService;
        _context = context;
    }

    public async Task<ErrorOr<WisdomResponse>> Handle(
        ExtractWisdomQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _nlpService.GenerateEmbeddingVectorAsync(
            query.Question,
            cancellationToken);

        if (result.IsError)
        {
            return result.Errors;
        }

        var questionVector = new Vector(result.Value);

        var workItems = await _context.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == query.ProjectId && !string.IsNullOrEmpty(wi.Description))
            .Select(WorkItemQueries.ProjectToDtoWithDistance(questionVector))
            .Where(x => x.Distance <= MaxDistance)
            .OrderBy(x => x.Distance)
            .Take(MaxItems)
            .ToListAsync(cancellationToken);

        if (workItems.Count == 0)
        {
            return new WisdomResponse
            {
                Answer = "No relevant work items found.",
                Sources = [],
            };
        }

        var regex = WhitespaceRegex();
        var descriptions = workItems
            .Select(wi => NormalizeText(wi.Description!, regex))
            .ToList();
        var contextText = string.Join("\n", descriptions);

        var answerResult = await _nlpService.AnswerQuestionAsync(
            query.Question,
            contextText,
            cancellationToken);

        if (answerResult.IsError)
        {
            return answerResult.Errors;
        }

        return new WisdomResponse
        {
            Answer = answerResult.Value,
            Sources = workItems,
        };
    }

    private static string NormalizeText(string text, Regex regex)
    {
        return regex.Replace(text.Trim(), " ");
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
