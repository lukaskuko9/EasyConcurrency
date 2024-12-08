namespace Web;

public class RefundRequest
{
    public required string AccountKey { get; set; }
    public required decimal Amount { get; set; }
}