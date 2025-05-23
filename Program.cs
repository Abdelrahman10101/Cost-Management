using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CostManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    // Database simulation with dummy data
    public static class DummyDatabase
    {
        public static List<CostEntry> CostEntries = new List<CostEntry>
        {
            new CostEntry { Id = 1, Category = "Office Supplies", Amount = 150.00m, Date = "2023-05-15", Description = "Printer paper" },
            new CostEntry { Id = 2, Category = "Software", Amount = 499.99m, Date = "2023-05-18", Description = "Project management tool subscription" }
        };

        public static List<Invoice> Invoices = new List<Invoice>
        {
            new Invoice
            {
                Id = 1001,
                ClientId = "CL001",
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { Name = "Web Design", Quantity = 10, UnitPrice = 75.00m },
                    new InvoiceItem { Name = "Hosting", Quantity = 1, UnitPrice = 120.00m }
                },
                TaxRate = 0.08m,
                Discounts = 0.05m,
                DueDate = "2023-06-15",
                Status = "pending",
                ClientContact = new ClientContact { Email = "client1@example.com", Phone = "+1234567890" }
            }
        };

        public static List<Client> Clients = new List<Client>
        {
            new Client { Id = "CL001", Name = "Acme Corp", Region = "US-CA" },
            new Client { Id = "CL002", Name = "Globex Inc", Region = "US-NY" }
        };

        public static Dictionary<string, decimal> TaxRates = new Dictionary<string, decimal>
        {
            { "US-CA", 0.0825m },
            { "US-NY", 0.08875m },
            { "EU-DE", 0.19m },
            { "default", 0.05m }
        };
    }

    // Data Models
    public class CostEntry
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
    }

    public class InvoiceItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class ClientContact
    {
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class Invoice
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discounts { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
        public ClientContact ClientContact { get; set; }
        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        public string UpdatedAt { get; set; }
    }

    public class Client
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
    }

    public class Notification
    {
        public int InvoiceId { get; set; }
        public string Recipient { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
        public string SentAt { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        public string Status { get; set; } = "sent";
    }

    public class TaxCalculationResult
    {
        public decimal Subtotal { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
    }

    // Controllers
    [ApiController]
    [Route("api/[controller]")]
    public class CostEntriesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(DummyDatabase.CostEntries);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CostEntryRequest request)
        {
            if (string.IsNullOrEmpty(request.Category) || request.Amount <= 0 || 
                string.IsNullOrEmpty(request.Date) || string.IsNullOrEmpty(request.Description))
            {
                return BadRequest("All parameters are required");
            }

            var costRecord = new CostEntry
            {
                Id = GenerateId(),
                Category = request.Category,
                Amount = request.Amount,
                Date = request.Date,
                Description = request.Description
            };

            DummyDatabase.CostEntries.Add(costRecord);
            return CreatedAtAction(nameof(GetById), new { id = costRecord.Id }, costRecord);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var entry = DummyDatabase.CostEntries.FirstOrDefault(c => c.Id == id);
            if (entry == null) return NotFound();
            return Ok(entry);
        }

        private int GenerateId()
        {
            return new Random().Next(1000, 9999);
        }
    }

    public class CostEntryRequest
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(DummyDatabase.Invoices);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var invoice = DummyDatabase.Invoices.FirstOrDefault(i => i.Id == id);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        [HttpPost]
        public IActionResult Create([FromBody] InvoiceRequest request)
        {
            if (string.IsNullOrEmpty(request.ClientId)) return BadRequest("Client ID is required");
            if (request.Items == null || request.Items.Count == 0) return BadRequest("Items are required");

            var client = DummyDatabase.Clients.FirstOrDefault(c => c.Id == request.ClientId);
            if (client == null) return BadRequest("Client not found");

            // Calculate subtotal
            var subtotal = request.Items.Sum(item => item.Quantity * item.UnitPrice);

            // Use provided tax or calculate based on region
            var effectiveTaxRate = request.TaxRate ?? 
                (DummyDatabase.TaxRates.ContainsKey(client.Region) ? DummyDatabase.TaxRates[client.Region] : DummyDatabase.TaxRates["default"]);

            // Calculate total
            var discountedAmount = subtotal * (1 - request.Discounts);
            var taxAmount = discountedAmount * effectiveTaxRate;
            var total = discountedAmount + taxAmount;

            var invoice = new Invoice
            {
                Id = GenerateId(),
                ClientId = request.ClientId,
                Items = new List<InvoiceItem>(request.Items),
                Subtotal = subtotal,
                Discounts = request.Discounts,
                TaxRate = effectiveTaxRate,
                TaxAmount = taxAmount,
                Total = total,
                DueDate = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"), // 30 days from now
                Status = "pending",
                ClientContact = new ClientContact { Email = client.Id + "@example.com", Phone = "+1234567890" }
            };

            DummyDatabase.Invoices.Add(invoice);
            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] InvoiceUpdateRequest request)
        {
            var invoice = DummyDatabase.Invoices.FirstOrDefault(i => i.Id == id);
            if (invoice == null) return NotFound();

            // Update items if provided
            if (request.Items != null && request.Items.Count > 0)
            {
                invoice.Items = new List<InvoiceItem>(request.Items);
            }

            // Update tax if provided
            if (request.TaxRate.HasValue)
            {
                invoice.TaxRate = request.TaxRate.Value;
            }

            // Update discounts if provided
            if (request.Discounts.HasValue)
            {
                invoice.Discounts = request.Discounts.Value;
            }

            // Recalculate invoice totals
            invoice.Subtotal = invoice.Items.Sum(item => item.Quantity * item.UnitPrice);
            var discountedAmount = invoice.Subtotal * (1 - invoice.Discounts);
            invoice.TaxAmount = discountedAmount * invoice.TaxRate;
            invoice.Total = discountedAmount + invoice.TaxAmount;
            invoice.UpdatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            return Ok(invoice);
        }

        [HttpPost("{id}/reminders")]
        public IActionResult SendReminder(int id, [FromBody] ReminderRequest request = null)
        {
            var invoice = DummyDatabase.Invoices.FirstOrDefault(i => i.Id == id);
            if (invoice == null) return NotFound();

            var effectiveDueDate = request?.DueDate ?? invoice.DueDate;
            var contact = request?.ClientContact ?? invoice.ClientContact;

            var dueDateTime = DateTime.Parse(effectiveDueDate);
            var daysUntilDue = (dueDateTime - DateTime.Now).Days;

            string message;
            if (daysUntilDue < 0)
            {
                message = $"Urgent: Your invoice #{id} is overdue by {Math.Abs(daysUntilDue)} days.";
            }
            else if (daysUntilDue == 0)
            {
                message = $"Reminder: Your invoice #{id} is due today.";
            }
            else
            {
                message = $"Friendly reminder: Your invoice #{id} is due in {daysUntilDue} days.";
            }

            var notification = new Notification
            {
                InvoiceId = id,
                Recipient = !string.IsNullOrEmpty(contact.Email) ? contact.Email : contact.Phone,
                Method = !string.IsNullOrEmpty(contact.Email) ? "email" : "sms",
                Message = message
            };

            // In a real system, we would actually send the notification here
            Console.WriteLine($"Notification sent via {notification.Method} to {notification.Recipient}: {notification.Message}");

            return Ok(notification);
        }

        private int GenerateId()
        {
            return new Random().Next(1000, 9999);
        }
    }

    public class InvoiceRequest
    {
        public string ClientId { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal Discounts { get; set; } = 0;
    }

    public class InvoiceUpdateRequest
    {
        public List<InvoiceItem> Items { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? Discounts { get; set; }
    }

    public class ReminderRequest
    {
        public string DueDate { get; set; }
        public ClientContact ClientContact { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TaxController : ControllerBase
    {
        [HttpGet("calculate")]
        public IActionResult Calculate([FromQuery] TaxCalculationRequest request)
        {
            if (request.Subtotal <= 0) return BadRequest("Subtotal must be positive");
            if (string.IsNullOrEmpty(request.Region)) return BadRequest("Region is required");

            var effectiveTaxRate = request.TaxRate ?? 
                (DummyDatabase.TaxRates.ContainsKey(request.Region) ? DummyDatabase.TaxRates[request.Region] : DummyDatabase.TaxRates["default"]);

            var taxAmount = request.Subtotal * effectiveTaxRate;
            var total = request.Subtotal + taxAmount;

            return Ok(new TaxCalculationResult
            {
                Subtotal = request.Subtotal,
                TaxRate = effectiveTaxRate,
                TaxAmount = taxAmount,
                Total = total
            });
        }

        [HttpGet("rates/{region}")]
        public IActionResult GetRate(string region)
        {
            var rate = DummyDatabase.TaxRates.ContainsKey(region) ? DummyDatabase.TaxRates[region] : DummyDatabase.TaxRates["default"];
            return Ok(new { Region = region, TaxRate = rate });
        }
    }

    public class TaxCalculationRequest
    {
        public decimal Subtotal { get; set; }
        public string Region { get; set; }
        public decimal? TaxRate { get; set; }
    }
}