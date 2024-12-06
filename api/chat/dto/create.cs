public class CreateChatDto
{
    public string? TextChat { get; set; }

    // public string? IdRoom { get; set; }
}

public class GeminiResponse
{
    public List<GeminiCandidate> Candidates { get; set; }
    public GeminiUsageMetadata UsageMetadata { get; set; }
}

public class GeminiCandidate
{
    public GeminiContent Content { get; set; }
    public string FinishReason { get; set; }
    public int Index { get; set; }
    public List<GeminiSafetyRating> SafetyRatings { get; set; }
}

public class GeminiContent
{
    public List<GeminiPart> Parts { get; set; }
    public string Role { get; set; }
}

public class GeminiPart
{
    public string Text { get; set; }
}

public class GeminiSafetyRating
{
    public string Category { get; set; }
    public string Probability { get; set; }
}

public class GeminiUsageMetadata
{
    public int PromptTokenCount { get; set; }
    public int CandidatesTokenCount { get; set; }
    public int TotalTokenCount { get; set; }
}