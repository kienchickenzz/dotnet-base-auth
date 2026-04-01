namespace AuthService.Application.Common.ApplicationServices.Email;


public interface IEmailTemplateFactory
{
    string GenerateEmailTemplate<T>(string templateName, T mailTemplateModel);
}