using Microsoft.EntityFrameworkCore;

namespace BookStoreSubscription.Services
{
    public class InvoiceHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public InvoiceHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer  = new Timer(ProcessInvoices, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }


        private void ProcessInvoices(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SetBadPayers(context);
                GenerateInvoice(context);
            }
        }

        private static void SetBadPayers(ApplicationDbContext context) 
        {
            context.Database.ExecuteSqlInterpolated($"exec SetBadPayers");
        }

        private static void GenerateInvoice(ApplicationDbContext context)
        {
            var today = DateTime.Today;
            var comparisonDate = today.AddMonths(-1);
            var invoiceAlreadyIssueds = context.InvoiceIssueds.Any(x =>
                x.Year == comparisonDate.Year && x.Month == comparisonDate.Month
            );
            if (!invoiceAlreadyIssueds)
            {
                var beginDate = new DateTime(comparisonDate.Year, comparisonDate.Month, 1);
                var endDate = comparisonDate.AddMonths(1);
                context.Database.ExecuteSqlInterpolated($"exec InvoiceCreation {beginDate.ToString("yyyy-MM-dd")}, {endDate.ToString("yyyy-MM-dd")}");
            }
        }
    }
}
