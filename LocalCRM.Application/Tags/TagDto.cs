namespace LocalCRM.Application.Tags;

public class TagDto
{
    public int TagId { get; set; }
    public string TagGroup { get; set; } = string.Empty;
    public string TagName { get; set; } = string.Empty;
    public string TagValue { get; set; } = string.Empty;
    public int? TagOrder { get; set; }
}
