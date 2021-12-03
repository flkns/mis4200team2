using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using mis4200team2.Models;

namespace mis4200team2
{
  public class EmailService : IIdentityMessageService
  {
    public async Task SendAsync(IdentityMessage message)
    {
      MailMessage mailMessage = new MailMessage();

      mailMessage.From = new MailAddress("ecentrictest@gmail.com", "eCentric Email Service", System.Text.Encoding.UTF8);
      mailMessage.To.Add(message.Destination);

      mailMessage.Subject = message.Subject;
      mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

      mailMessage.IsBodyHtml = true;
      mailMessage.Body = message.Body;
      mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

      SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
      client.SendCompleted += (s, e) =>
      {
        client.Dispose();
      };
      client.EnableSsl = true;
      client.DeliveryMethod = SmtpDeliveryMethod.Network;

      NetworkCredential credentials = new NetworkCredential(
          ConfigurationManager.AppSettings["mailAccount"],
          ConfigurationManager.AppSettings["mailPassword"]
      );
      client.Credentials = credentials;
      try
      {
        await client.SendMailAsync(mailMessage);
      }
      catch (Exception ex)
      {
        Trace.TraceError(ex.Message);
        await Task.FromResult(0);
      }
    }
  }

  public class SmsService : IIdentityMessageService
  {
    public Task SendAsync(IdentityMessage message)
    {
      // Plug in your SMS service here to send a text message.
      return Task.FromResult(0);
    }
  }

  // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
  public class ApplicationUserManager : UserManager<ApplicationUser>
  {
    public ApplicationUserManager(IUserStore<ApplicationUser> store)
        : base(store)
    {
    }

    public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
    {
      var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
      // Configure validation logic for usernames
      manager.UserValidator = new UserValidator<ApplicationUser>(manager)
      {
        AllowOnlyAlphanumericUserNames = true,
        RequireUniqueEmail = true
      };

      //var ben = manager.FindByEmail("bf853817@ohio.edu");
      //if (ben.Roles.Count() == 0)
      //{
      //  manager.AddToRoles(ben.Id, new string[] { "Admin" });
      //}

      // Configure validation logic for passwords
      manager.PasswordValidator = new PasswordValidator
      {
        RequiredLength = 6,
        RequireNonLetterOrDigit = true,
        RequireDigit = true,
        RequireLowercase = true,
        RequireUppercase = true,
      };

      // Configure user lockout defaults
      manager.UserLockoutEnabledByDefault = true;
      manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
      manager.MaxFailedAccessAttemptsBeforeLockout = 5;

      // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
      // You can write your own provider and plug it in here.
      manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
      {
        MessageFormat = "Your security code is {0}"
      });
      manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
      {
        Subject = "Security Code",
        BodyFormat = "Your security code is {0}"
      });

      manager.EmailService = new EmailService();
      manager.SmsService = new SmsService();

      var dataProtectionProvider = options.DataProtectionProvider;
      if (dataProtectionProvider != null)
      {
        manager.UserTokenProvider =
            new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
      }

      return manager;
    }
  }


  // Configure the application sign-in manager which is used in this application.
  public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
  {
    public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
        : base(userManager, authenticationManager)
    {
    }

    public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
    {
      return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
    }

    public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
    {
      return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
    }
  }
}
