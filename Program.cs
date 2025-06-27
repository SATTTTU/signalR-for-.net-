using Microsoft.AspNetCore.SignalR;
 using ChatBackend.Services;
using ChatBackend.Hubs;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Define CORS policy name ---
var MyCorsPolicy = "_myCorsPolicy";

// --- 2. Read multiple React URLs (local + production) from config ---
var reactAppUrlDev = builder.Configuration["CorsPolicy:ReactAppUrl"];
var reactAppUrlProd = builder.Configuration["CorsPolicy:ReactAppUrlProd"];

// --- 3. Configure Services ---

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCorsPolicy, policy =>
    {
        policy.WithOrigins(reactAppUrlDev, reactAppUrlProd)
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

// Set up SignalR hub endpoint
app.MapHub<ChatHub>("/chathub");

// Set up controllers
app.MapControllers();

// --- 6. Bind to Render's provided port ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

// --- 7. Run the App ---
app.Run();
