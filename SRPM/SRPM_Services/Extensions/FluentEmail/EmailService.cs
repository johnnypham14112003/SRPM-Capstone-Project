using FluentEmail.Core;
using FluentEmail.Core.Models;
using Razor.Templating.Core;
using SRPM_Services.BusinessModels.Others;

namespace SRPM_Services.Extensions.FluentEmail;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailDTO sendEmailDto, bool hasUIBody = true);

    Task<string> RenderPasswordEmail(DTO_PasswordEmail model);
}

public class EmailService : IEmailService
{
    //=============================[ DECLARATION ]=============================
    private readonly IFluentEmailFactory _fluentEmailFactory;
    private readonly IRazorTemplateEngine _razorTemplateEngine;

    //=============================[ CONSTRUCTOR ]=============================
    public EmailService(IFluentEmailFactory fluentEmailFactory, IRazorTemplateEngine razorTemplateEngine)
    {
        _fluentEmailFactory = fluentEmailFactory;
        _razorTemplateEngine = razorTemplateEngine;
    }

    //=============================[ METHODS ]=============================
    public async Task<bool> SendEmailAsync(EmailDTO emailDto, bool hasUIBody = true)
    {
        var email = _fluentEmailFactory.Create();

        email.To(emailDto.ReceiverEmailAddress);
        //CC logic
        if (emailDto.ListAddressToCC is not null)
        {
            var listConverted = emailDto.ListAddressToCC.Select(email => new Address(email));
            email.CC(listConverted);//Visible to all receiver
        }
        
        //BCC logic
        if (emailDto.ListAddressToBCC is not null)
        {
            var listConverted = emailDto.ListAddressToBCC.Select(email => new Address(email));
            email.BCC(listConverted);//Can't see other BCC receiver
        }
        email.Subject(emailDto.Subject);
        email.Body(emailDto.Body, hasUIBody);

        var result = await email.SendAsync();
        return result.Successful;
    }

    //Render Object Date into UI email
    public async Task<string> RenderPasswordEmail(DTO_PasswordEmail model)
    {
        return await _razorTemplateEngine.RenderAsync("/PasswordEmail.cshtml", model);
    }

}