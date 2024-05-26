using ClosedXML.Excel;
using Examensarbete.Data;
using Examensarbete.Models;
using System.Globalization;
using static Examensarbete.Pages.Uploads.UploadProductsModel;

namespace Examensarbete.Services
{
    public class OrderDataService : IOrderDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public OrderDataService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<ImportResult> ImportOrderDataAsync(IFormFile fileUpload)
        {
            if (fileUpload == null || fileUpload.Length == 0)
                throw new ArgumentException("Ingen fil valdes för uppladdningen, eller så är den tom.");

            var filePath = await SaveUploadedFile(fileUpload);
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheet(1);
                var orders = new List<OrderData>();

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    var order = CreateOrderDataFromRow(row);
                    if (order != null)
                        orders.Add(order);
                }
                _context.OrderData.AddRange(orders);
                int saved = await _context.SaveChangesAsync();
                return new ImportResult { RecordsAdded = saved };
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private async Task<string> SaveUploadedFile(IFormFile fileUpload)
        {
            var folderPath = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileUpload.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileUpload.CopyToAsync(stream);
            }
            return filePath;
        }

        private OrderData CreateOrderDataFromRow(IXLRow row)
        {
            DateTime orderDate;
            var dateString = row.Cell(1).GetValue<string>();
            if (!DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out orderDate))
                return null;

            return new OrderData
            {
                OrderDate = orderDate,
                OrderGroup = row.Cell(2).GetValue<string>(),
                SubArea = row.Cell(3).GetValue<string>(),
                ItemDescription = row.Cell(4).GetValue<string>(),
                ArticleNumber = row.Cell(5).GetValue<string>(),
                SupplierName = row.Cell(6).GetValue<string>(),
                UnitType = row.Cell(7).GetValue<string>(),
                ConfirmedQuantity = row.Cell(8).GetValue<int>(),
                Price = row.Cell(9).GetValue<decimal>(),
                ConfirmedNetAmount = row.Cell(10).GetValue<decimal>(),
                Account = row.Cell(11).GetValue<string>(),
                CostCenter = row.Cell(12).GetValue<string>(),
                Organization = row.Cell(13).GetValue<string>(),
            };
        }
    }
}
