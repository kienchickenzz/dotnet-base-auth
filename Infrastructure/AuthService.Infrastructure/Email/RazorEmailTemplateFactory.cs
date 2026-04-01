/**
 * Razor-based email template factory.
 *
 * <p>Generates HTML email content from Razor templates.</p>
 */

namespace AuthService.Infrastructure.Email.Template;

using System.Text;

using RazorEngineCore;

using AuthService.Application.Common.ApplicationServices.Email;

/// <summary>
/// Generates email content using Razor templates.
/// </summary>
public class RazorEmailTemplateFactory : IEmailTemplateFactory
{
    /// <inheritdoc />
    public string GenerateEmailTemplate<T>(string templateName, T mailTemplateModel)
    {
        string template = _GetTemplate(templateName);

        IRazorEngine razorEngine = new RazorEngine();
        IRazorEngineCompiledTemplate modifiedTemplate = razorEngine.Compile(template);

        return modifiedTemplate.Run(mailTemplateModel);
    }

    /// <summary>
    /// Reads template content from file.
    /// </summary>
    private static string _GetTemplate(string templateName)
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string tmplFolder = Path.Combine(baseDirectory, "EmailTemplates");
        string filePath = Path.Combine(tmplFolder, $"{templateName}.cshtml");

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.Default);
        string mailText = sr.ReadToEnd();
        sr.Close();

        return mailText;
    }
}
