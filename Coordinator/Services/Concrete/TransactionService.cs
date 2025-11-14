using Coordinator.Models;
using Coordinator.Models.Context;
using Coordinator.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services.Concrete
{
    public class TransactionService(TwoPhaseCommitContext _context, IHttpClientFactory _clientFactory) : ITransactionService
    {
        HttpClient _orderHttpClient = _clientFactory.CreateClient("Order.API");
        HttpClient _stockHttpClient = _clientFactory.CreateClient("Stock.API");
        HttpClient _paymentHttpClient = _clientFactory.CreateClient("Payment.API");

        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();

            var nodes = await _context.Nodes.ToListAsync();
            nodes.ForEach(node =>
            {
                node.NodeStates = new List<NodeState>()
                {
                    new(transactionId)
                    {
                        IsReady = Enums.ReadyType.Pending,
                        TransactionState = Enums.TransactionState.Pending
                    }

                };
            });

            await _context.SaveChangesAsync();
            return transactionId;
        }

        public async Task PrepareServicesAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Include(ns => ns.Node)
                .Where(ns => ns.TransactionId == transactionId)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("ready"),
                        "Stock.API" => _stockHttpClient.GetAsync("ready"),
                        "Payment.API" => _paymentHttpClient.GetAsync("ready"),
                        _ => throw new InvalidOperationException("Unknown service")
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());

                    transactionNode.IsReady = result ? Enums.ReadyType.Ready : Enums.ReadyType.NotReady;

                }
                catch (Exception ex)
                {
                    transactionNode.IsReady = Enums.ReadyType.NotReady;
                }

            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckReadyServicesAsync(Guid transactionId)
            => (await _context.NodeStates.Where(ns => ns.TransactionId == transactionId).ToListAsync())
            .TrueForAll(ns => ns.IsReady == Enums.ReadyType.Ready);
        
        public async Task CommitAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates.Where(ns => ns.TransactionId == transactionId).Include(ns => ns.Node).ToListAsync();
            
            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("commit"),
                        "Stock.API" => _stockHttpClient.GetAsync("commit"),
                        "Payment.API" => _paymentHttpClient.GetAsync("commit"),         
                        _ => throw new InvalidOperationException("Unknown service")
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());

                    transactionNode.TransactionState = result ? Enums.TransactionState.Done : Enums.TransactionState.Aborted;

                }
                catch (Exception ex)
                {
                    transactionNode.TransactionState = Enums.TransactionState.Aborted;

                }

            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckTransactionStateAsync(Guid transactionId) => (await _context.NodeStates.Where(ns => ns.TransactionId == transactionId).ToListAsync())
            .TrueForAll(ns => ns.TransactionState == Enums.TransactionState.Done);

        public async Task RollbackAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates.Where(ns => ns.TransactionId == transactionId).Include(ns => ns.Node).ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    if(transactionNode.TransactionState == Enums.TransactionState.Done)
                    {
                        _ = await (transactionNode.Node.Name switch     
                        {
                            "Order.API" => _orderHttpClient.GetAsync("rollback"),
                            "Stock.API" => _stockHttpClient.GetAsync("rollback"),
                            "Payment.API" => _paymentHttpClient.GetAsync("rollback"),
                            _ => throw new InvalidOperationException("Unknown service")
                        });
                        transactionNode.TransactionState = Enums.TransactionState.Aborted;
                    }
                }
                catch (Exception ex)
                {

                    transactionNode.TransactionState = Enums.TransactionState.Aborted;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
