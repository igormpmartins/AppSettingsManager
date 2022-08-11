using AppSettingsManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AppSettingsManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;
        private readonly IOptions<TwilioSettings> twilioOptions;
        private readonly TwilioSettings twilioSettings;
        private readonly IOptions<SocialLoginSettings> socialLoginSettings;

        //private readonly TwilioSettings twilio;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration,
            IOptions<TwilioSettings> twilioOptions,
            TwilioSettings twilioSettings,
            IOptions<SocialLoginSettings> socialLoginSettings)
        {
            _logger = logger;
            this.configuration = configuration;
            this.twilioOptions = twilioOptions;
            this.twilioSettings = twilioSettings;
            this.socialLoginSettings = socialLoginSettings;

            //manual biding
            //twilio = new TwilioSettings();
            //configuration.GetSection("Twilio").Bind(twilio);
        }

        public IActionResult Index()
        {
            ViewBag.SendGridKey = configuration.GetValue<string>("SendGridKey");
            
            //ViewBag.TwilioAuthToken = configuration.GetValue<string>("Twilio:AuthToken");
            //ViewBag.TwilioPhoneNumber = twilio.PhoneNumber;

            //Using IOptions
            //ViewBag.TwilioAuthToken = twilioOptions.Value.AuthToken;
            //ViewBag.TwilioPhoneNumber = twilioOptions.Value.PhoneNumber;
            //ViewBag.TwilioAccountSid = twilioOptions.Value.AccountSid;

            //Using DI
            ViewBag.TwilioAuthToken = twilioSettings.AuthToken;
            ViewBag.TwilioPhoneNumber = twilioSettings.PhoneNumber;
            ViewBag.TwilioAccountSid = twilioSettings.AccountSid;

            //the commands below have the same result, just different approaches
            //ViewBag.TwilioAccountSid = configuration.GetValue<string>("Twilio:AccountSid");
            //ViewBag.TwilioAccountSid = configuration.GetSection("Twilio").GetValue<string>("AccountSid");

            ViewBag.BottomLevelSetting = configuration.GetValue<string>("FirstLevelSettings:SecondLevelSetting:BottomLevelSetting");
            //or
            //ViewBag.BottomLevelSetting = configuration.GetSection("FirstLevelSettings").GetSection("SecondLevelSetting").GetValue<string>("BottomLevelSetting");
            //or
            //ViewBag.BottomLevelSetting = configuration.GetSection("FirstLevelSettings").GetSection("SecondLevelSetting").GetSection("BottomLevelSetting").Value;

            ViewBag.SocialLoginEnabled = socialLoginSettings.Value.SocialLoginEnabled;
            ViewBag.FacebookSettingsKey = socialLoginSettings.Value.FacebookSettings.Key;
            ViewBag.GoogleSettingsKey = socialLoginSettings.Value.GoogleSettings.Key;

            ViewBag.ConnectionStrings = configuration.GetConnectionString("AppSettingsManagerDb");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}