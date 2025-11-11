using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using Api.Middleware;
using Api.Hubs;
using Api.Services;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Default") ?? "Data Source=app.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(cs));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketNotifier, SignalRNotifier>();
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IAgentService, AgentService>();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("db");

builder.Services.AddSignalR();

builder.Services.AddCors(o => o.AddPolicy("dev", p =>
    p.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true)));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors("dev");

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        var payload = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.ToString()
            }),
            totalDuration = report.TotalDuration.ToString()
        };
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
});
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = _ => true });

app.MapHub<SupportHub>("/rt");


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Infrastructure.Data.AppDbContext>();
    db.Database.Migrate();

    if (!db.Tickets.Any())
    {
        db.Tickets.AddRange(
            new Domain.Entities.Ticket
            {
                Title = "Credit transaction failed – unable to authorize payment",
                Description = "A customer reported that credit payments are being declined without any clear error message. Happens mostly with Visa cards.",
                CustomerEmail = "customer1@example.com",
                Priority = Domain.Entities.TicketPriority.High,
                Status = Domain.Entities.TicketStatus.Open,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow.AddDays(-7)
            },

            new Domain.Entities.Ticket
            {
                Title = "Timeout error while connecting to payment gateway",
                Description = "Multiple merchants experienced timeout errors when processing payments via the gateway API. Needs investigation of load or connectivity issues.",
                CustomerEmail = "merchant2@example.com",
                Priority = Domain.Entities.TicketPriority.Critical,
                Status = Domain.Entities.TicketStatus.InProgress,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Domain.Entities.Ticket
            {
                Title = "Credit report emails not sent automatically",
                Description = "Daily transaction reports are not being delivered to merchants’ emails. Appears to be related to the mail scheduler service.",
                CustomerEmail = "support@merchant3.com",
                Priority = Domain.Entities.TicketPriority.Medium,
                Status = Domain.Entities.TicketStatus.Open,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Domain.Entities.Ticket
            {
                Title = "Duplicate charge detected on customer account",
                Description = "A client was charged twice for the same order due to a retry logic issue. Refund requested and needs investigation.",
                CustomerEmail = "customer4@example.com",
                Priority = Domain.Entities.TicketPriority.High,
                Status = Domain.Entities.TicketStatus.Resolved,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },

            new Domain.Entities.Ticket
            {
                Title = "Payment authorization returns 500 Internal Server Error",
                Description = "Backend logs show unhandled exceptions during card authorization for certain BIN ranges. Possible issue with new provider integration.",
                CustomerEmail = "qa-team@example.com",
                Priority = Domain.Entities.TicketPriority.Critical,
                Status = Domain.Entities.TicketStatus.Open,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Ticket
            {
                Title = "Customer unable to update expired credit card details",
                Description = "Frontend validation prevents updating an expired card, even after entering a valid new expiration date. Affects Safari users.",
                CustomerEmail = "customer5@example.com",
                Priority = Domain.Entities.TicketPriority.Medium,
                Status = Domain.Entities.TicketStatus.Open,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
        db.SaveChanges();
    }

    if (!db.Agents.Any())
    {
        db.Agents.AddRange(
            new Domain.Entities.Agent { Name = "Alice Levi", Email = "alice@example.com" },
            new Domain.Entities.Agent { Name = "Efraim Cohen", Email = "efraim@example.com" },
            new Domain.Entities.Agent { Name = "John Miller", Email = "john.miller@example.com" }
        );
        db.SaveChanges();
    }
}



app.Run();




