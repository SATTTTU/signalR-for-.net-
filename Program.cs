using Microsoft.AspNetCore.SignalR;
using ChatBackend.Services;
using ChatBackend.Hubs;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Define CORS policy name ---
var MyCorsPolicy = "_myCorsPolicy";

// --- 2. Read Production Origin from appsettings.json ---
var reactAppUrlProd = builder.Configuration["CorsPolicy:ReactAppUrlProd"];

// --- 3. Configure Services ---
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCorsPolicy, policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrEmpty(origin)) return false;

                try
                {
                    var uri = new Uri(origin);
                    return uri.Host == "localhost" || origin == reactAppUrlProd;
                }
                catch
                {
                    return false;
                }
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IModerationService, NeutrinoApiModerationService>();
builder.Services.AddControllers();

// --- 4. Build the App ---
var app = builder.Build();

// --- 5. Configure HTTP Request Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Enable CORS before routing
app.UseCors(MyCorsPolicy);

app.UseRouting();
app.UseAuthorization();

// --- 6. Set up SignalR hub endpoint ---
app.MapHub<ChatHub>("/chathub");

// --- 7. Set up controllers ---
app.MapControllers();

// --- 8. Bind to Render's provided port ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

// --- 9. Run the App ---
app.Run();
