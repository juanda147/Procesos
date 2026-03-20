using ProcesosApi.Data;
using ProcesosApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped<IProcesoService, ProcesoService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<INotaService, NotaService>();
builder.Services.AddScoped<IRecordatorioService, RecordatorioService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<ICampoGlobalService, CampoGlobalService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<RecordatorioNotificacionService>();
builder.Services.AddHostedService<BackupService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ReactDev");

// Serve React frontend from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

// SPA fallback: any non-API, non-file route serves index.html
app.MapFallbackToFile("index.html");

// Create MongoDB indexes
var db = app.Services.GetRequiredService<MongoDbContext>();
await db.CreateIndexesAsync();

app.Run();
