public class CreateTranscribeDto
{
    public string? Name { get; set; }

}

public class DeepgramResponse
{
    public DeepgramResult Results { get; set; }
}

public class DeepgramResult
{
    public DeepgramChannel[] Channels { get; set; }
}

public class DeepgramChannel
{
    public DeepgramAlternative[] Alternatives { get; set; }
}

public class DeepgramAlternative
{
    public string Transcript { get; set; }
}