namespace Application.Abstractions;

public interface IHtmlSanitizerService
{
    string Sanitize(string html);
}
