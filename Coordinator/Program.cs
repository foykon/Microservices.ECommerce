using Coordinator.Models.Context;
using Coordinator.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TwoPhaseCommitContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

builder.Services.AddHttpClient("Order.API", client => client.BaseAddress = new Uri(builder.Configuration["Services:Order.API"]));
builder.Services.AddHttpClient("Stock.API", client => client.BaseAddress = new Uri(builder.Configuration["Services:Stock.API"]));
builder.Services.AddHttpClient("Payment.API", client => client.BaseAddress = new Uri(builder.Configuration["Services:Payment.API"]));

builder.Services.AddTransient<Coordinator.Services.Abstraction.ITransactionService, Coordinator.Services.Concrete.TransactionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/create-order-transaction", async (ITransactionService transactionService) =>
{
    //Phase 1 - Prepare
    var transactionId = await transactionService.CreateTransactionAsync();
    await transactionService.PrepareServicesAsync(transactionId);
    bool transactionState = await transactionService.CheckReadyServicesAsync(transactionId);

    if (transactionState)
    {
        //Phase 2 - Commit
        await transactionService.CommitAsync(transactionId);
        transactionState = await transactionService.CheckTransactionStateAsync(transactionId);
    }

    if (!transactionState)
        await transactionService.RollbackAsync(transactionId);
});

app.Run();
