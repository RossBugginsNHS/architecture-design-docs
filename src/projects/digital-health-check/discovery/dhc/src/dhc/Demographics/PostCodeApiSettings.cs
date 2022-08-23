namespace dhc;

public class PostCodeApiSettings
{
    public static readonly string Location = "PostCodeApi";
    public string BaseUrl { get; set; }

    public TimeSpan SlidingCacheLength{get;set;}
    public TimeSpan AbsoluteCacheLength {get;set;}
}
