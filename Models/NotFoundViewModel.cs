namespace Muhasebe.Models;

public class NotFoundViewModel
{
    public string EntityName { get; set; } = "Kayıt";
    public string? Message { get; set; }
    public int? RequestedId { get; set; }
    public string ListController { get; set; } = "Home";
    public string ListAction { get; set; } = "Index";
    public string? ListLabel { get; set; }
}
