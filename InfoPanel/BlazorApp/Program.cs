using RejseplanWebsite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.InjectConfig();

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Named HttpClient with dynamic BaseAddress and optional cert bypass (DEV only)
builder.Services.AddHttpClient("Default", (serviceProvider, client) =>
{
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;

    if (request == null)
        throw new InvalidOperationException("HttpContext is not available during HttpClient configuration.");

    // Dynamically set base address with correct scheme
    client.BaseAddress = new Uri($"{request.Scheme}://{request.Host}/");
}).ConfigurePrimaryHttpMessageHandler(() =>
new HttpClientHandler
{
    // Bypass cert validation
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});


// Register default HttpClient for DI
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Default"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<BlazorApp.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
