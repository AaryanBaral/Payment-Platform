using Microsoft.EntityFrameworkCore;
using PaymentPlatform.Application.Payouts.Commands.GeneratePayoutRequest;
using PaymentPlatform.Application.Payouts.Services;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Infrastructure.Payouts;
using PaymentPlatform.Infrastructure.Persistence;
using PaymentPlatform.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IMerchantRepository, MerchantRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPayoutRepository, PayoutRepository>();
builder.Services.AddScoped<ILedgerEntryRepository, LedgerEntryRepository>();

builder.Services.AddScoped<IMerchantBalanceService, MerchantBalanceService>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

// Command handler
builder.Services.AddScoped<GeneratePayoutRequestHandler>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
        using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DbSeeders.SeedAsync(db);
    }
}
app.UseHttpsRedirection();

app.MapControllers(); 



app.MapGet("/", () =>"Working fine");


app.Run();

