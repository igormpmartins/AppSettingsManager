using AppSettingsManager.Extensions;
using AppSettingsManager.Models;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

ConfigureCustomConfigHierarchy(args, builder);

// Add services to the container.
AddConfigurations(builder);
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void ConfigureCustomConfigHierarchy(string[] args, WebApplicationBuilder builder)
{
    if (!builder.Environment.IsDevelopment())
    {
        var uri = new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/");
        builder.Configuration.AddAzureKeyVault(uri, new DefaultAzureCredential());
    }

    return;

    //Just change the order to change hirarchy. From lowest to highest priority!
    //Note: WebHost start acting weird after change this! I don't recommend...

    builder.Host.ConfigureAppConfiguration((context, builder) =>
    {
        builder.Sources.Clear();

        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);

        if (context.HostingEnvironment.IsDevelopment())
            builder.AddUserSecrets<Program>();

        //adding custom file
        //builder.AddJsonFile($"customSettings.json", optional: true, reloadOnChange: true);
        builder.AddEnvironmentVariables();
        builder.AddCommandLine(args);
    });
}

static void AddConfigurations(WebApplicationBuilder builder)
{
    //this one uses extension method!
    builder.Services.AddConfiguration<TwilioSettings>(builder.Configuration, "Twilio");

    //equivalent to above!
    /*var twilioSettings = new TwilioSettings();
    new ConfigureFromConfigurationOptions<TwilioSettings>(builder.Configuration.GetSection("Twilio")).Configure(twilioSettings);
    builder.Services.AddSingleton(twilioSettings);
    */

    //for a singleton
    //builder.Services.AddConfiguration<SocialLoginSettings>(builder.Configuration, "SocialLoginSettings");
    //or using IOptions
    builder.Services.Configure<SocialLoginSettings>(builder.Configuration.GetSection("SocialLoginSettings"));

    builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
}