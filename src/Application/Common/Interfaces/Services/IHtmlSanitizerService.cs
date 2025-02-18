namespace Application.Common.Interfaces.Services;

public interface IHtmlSanitizerService
{
    string Sanitize(string html);
}
