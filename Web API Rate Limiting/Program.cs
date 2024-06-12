using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//use fixed window
builder.Services.AddRateLimiter(option =>
{
    option.AddFixedWindowLimiter("FixedWindowPolicy", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(5);
        opt.PermitLimit = 5;
        opt.QueueLimit = 3;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

    }).RejectionStatusCode = 429; //Too Many Request
});


//use sliding Window
builder.Services.AddRateLimiter(option =>
{
    option.AddSlidingWindowLimiter("slidingWindowPolicy", opt => {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 4;
        opt.QueueLimit = 3;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.SegmentsPerWindow = 3;
    }).RejectionStatusCode = 429;
});

//use concurrency
builder.Services.AddRateLimiter(option =>
{
    option.AddConcurrencyLimiter("ConcurrencyPolicy", opt => {
        opt.PermitLimit = 1;
        opt.QueueLimit = 8;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    }).RejectionStatusCode = 429;
});


//use token bucket limiter 
builder.Services.AddRateLimiter(option =>
{
    option.AddTokenBucketLimiter("TokenBucketPolicy", opt => {
        opt.TokenLimit = 4;
        opt.QueueLimit = 2;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.ReplenishmentPeriod=TimeSpan.FromSeconds(10);
        opt.TokensPerPeriod = 4;
        opt.AutoReplenishment = true;
    }).RejectionStatusCode = 429;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
