namespace SRPM_Services.Extensions.Utils;
public class StringUtils
{
    public static string DecodeHtmlEntitiesText(string? rawText)
    {
        if (string.IsNullOrEmpty(rawText)) return "";
        var text = HtmlAgilityPack.HtmlEntity.DeEntitize(rawText); // giải mã HTML entities
        text = text.Replace('\u00A0', ' ');                     // NBSP -> space thường
        return text.Trim();
    }
}
