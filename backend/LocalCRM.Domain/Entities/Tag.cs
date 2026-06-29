namespace LocalCRM.Domain.Entities;

public class Tag
{
    public int TagId { get; set; }
    public string TagGroup { get; set; } = string.Empty;
    public string TagName { get; set; } = string.Empty;
    public string TagValue { get; set; } = string.Empty;
    public int? TagOrder { get; set; } = 0;
}
