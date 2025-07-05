namespace SRPM_Services.BusinessModels.Others;

//for strong-typed config
public class FluentEmailOptionModel
{
    public string Address { get; set; } = default!;
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string AppPassword { get; set; } = default!;
}