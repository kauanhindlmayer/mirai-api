using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using Application.Abstractions;
using Application.WorkItems.Queries;
using Domain.Shared;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pgvector;

namespace Application.WisdomExtractor.Queries.ExtractWisdom;

internal sealed partial class ExtractWisdomQueryHandler
    : IRequestHandler<ExtractWisdomQuery, ErrorOr<WisdomResponse>>
{
    private const double MaxDistance = 0.6;
    private const int MaxItems = 4;
    private readonly ILogger<ExtractWisdomQueryHandler> _logger;
    private readonly IChatClient _chatClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly IApplicationDbContext _context;

    public ExtractWisdomQueryHandler(
        ILogger<ExtractWisdomQueryHandler> logger,
        [FromKeyedServices(ServiceKeys.Chat)] IChatClient chatClient,
        [FromKeyedServices(ServiceKeys.Embedding)] IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IApplicationDbContext context)
    {
        _logger = logger;
        _chatClient = chatClient;
        _embeddingGenerator = embeddingGenerator;
        _context = context;
    }

    public async Task<ErrorOr<WisdomResponse>> Handle(
        ExtractWisdomQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            var embedding = await _embeddingGenerator.GenerateAsync(
                query.Question,
                cancellationToken: cancellationToken);

            var questionVector = new Vector(embedding.Vector.ToArray());

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
                    Answer = "No relevant work items found for your question.",
                    Sources = [],
                };
            }

            var regex = WhitespaceRegex();
            var descriptions = workItems
                .Select(wi => NormalizeText(wi.Description!, regex))
                .ToList();
            var contextText = string.Join("\n\n", descriptions);

            var prompt = BuildAnswerPrompt(query.Question, contextText);
            var response = await _chatClient.GetResponseAsync(
                prompt,
                cancellationToken: cancellationToken);

            return new WisdomResponse
            {
                Answer = response.Text,
                Sources = workItems,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while extracting wisdom");
            return Error.Failure(
                "WisdomExtractor.Failed",
                "An error occurred while extracting wisdom.");
        }
    }

    private static string BuildAnswerPrompt(string question, string context)
    {
        return $"""
            Based on the following work item descriptions from the project, answer the user's question.
            
            CONTEXT (Work Items):
            {context}
            
            QUESTION: {question}
            
            INSTRUCTIONS:
            - Provide a helpful answer based ONLY on the information in the context above
            - If the context doesn't contain enough information, say so clearly
            - Keep your answer concise and relevant to project management
            - Reference specific work item codes when applicable
            - If multiple work items are relevant, summarize the key insights
            """;
    }

    private static string NormalizeText(string text, Regex regex)
    {
        return regex.Replace(text.Trim(), " ");
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}