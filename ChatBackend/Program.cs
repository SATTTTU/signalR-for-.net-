// --- The Complete and Corrected Program.cs ---


var builder = WebApplication.CreateBuilder(args);

// --- Define the CORS policy name ---
var MyCorsPolicy = "_myCorsPolicy";

// --- 1. Read the React App URL from appsettings.json ---
// This makes your code cleaner and more configurable.
var reactAppUrl = builder.Configuration["CorsPolicy:ReactAppUrl"];

// --- 2. Configure Services ---

// Add CORS services and define our policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCorsPolicy,
                      policy =>
                      {
                          // Use the URL we read from the config file!
                          policy.WithOrigins(reactAppUrl) 
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
});

builder.Services.AddSignalR();
builder.Services.AddHttpClient();

// This line requires IModerationService.cs and NeutrinoApiModerationService.cs to exist
builder.Services.AddSingleton<IModerationService, NeutrinoApiModerationService>();

builder.Services.AddControllers();

// --- 3. Build the App ---
var app = builder.Build();

// --- 4. Configure the HTTP Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// This MUST be in the correct order
app.UseCors(MyCorsPolicy);
app.UseRouting();
app.UseAuthorization();

// This line requires ChatHub.cs and ChatMessage.cs to exist
app.MapHub<ChatHub>("/chathub"); 

app.MapControllers();

// --- 5. Run the App ---
app.Run();