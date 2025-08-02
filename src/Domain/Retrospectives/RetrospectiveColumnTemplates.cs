using Domain.Retrospectives.Enums;

namespace Domain.Retrospectives;

public static class RetrospectiveColumnTemplates
{
    public static readonly Dictionary<RetrospectiveTemplate, List<string>> Templates = new()
    {
        {
            RetrospectiveTemplate.Classic, new List<string>
            {
                "What went well?",
                "What didn't go well?",
                "What can be improved?",
            }
        },
        {
            RetrospectiveTemplate.StartStopContinue, new List<string>
            {
                "Start",
                "Stop",
                "Continue",
            }
        },
        {
            RetrospectiveTemplate.MadSadGlad, new List<string>
            {
                "Mad",
                "Sad",
                "Glad",
            }
        },
        {
            RetrospectiveTemplate.LikedLearnedLackedLongedFor, new List<string>
            {
                "Liked",
                "Learned",
                "Lacked",
                "Longed For",
            }
        },
        {
            RetrospectiveTemplate.Sailboat, new List<string>
            {
                "Anchors (Slowing us down)",
                "Winds (Pushing us forward)",
                "Rocks (Risks ahead)",
                "Destination (Goals)",
            }
        },
    };
}
