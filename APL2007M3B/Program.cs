using System;
using System.Collections.Generic;
using System.Linq;

public class QuarterlyIncomeReport
{
    public static void Main(string[] args)
    {
        // create a new instance of the class
        var report = new QuarterlyIncomeReport();

        // call the GenerateSalesData method
        var salesData = report.GenerateSalesData();

        // call the QuarterlySalesReport method
        report.QuarterlySalesReport(salesData);
    }

    public class SalesData
    {
        public DateTime DateSold { get; set; }
        public string DepartmentName { get; set; }
        public string ProductId { get; set; }
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal BaseCost { get; set; }
        public int VolumeDiscount { get; set; }

        public SalesData(DateTime dateSold, string departmentName, string productId, int quantitySold, decimal unitPrice, decimal baseCost, int volumeDiscount)
        {
            DateSold = dateSold;
            DepartmentName = departmentName;
            ProductId = productId;
            QuantitySold = quantitySold;
            UnitPrice = unitPrice;
            BaseCost = baseCost;
            VolumeDiscount = volumeDiscount;
        }
    }

    public class ProdDepartments
    {
        public static readonly string[] DepartmentNames = { "Men's Clothing", "Women's Clothing", "Children's Clothing", "Accessories", "Footwear", "Outerwear", "Sportswear", "Undergarments" };
        public static readonly string[] DepartmentAbbreviations = { "MENS", "WOMN", "CHLD", "ACCS", "FOOT", "OUTR", "SPRT", "UNDR" };
    }

    public class ManufacturingSites
    {
        public static readonly string[] ManufacturingSitesList = { "US1", "US2", "US3", "UK1", "UK2", "UK3", "JP1", "JP2", "JP3", "CA1" };
    }

    public List<SalesData> GenerateSalesData()
    {
        var salesData = new List<SalesData>();
        var random = new Random();

        for (int i = 0; i < 1000; i++)
        {
            var dateSold = new DateTime(2023, random.Next(1, 13), random.Next(1, 29));
            var departmentName = ProdDepartments.DepartmentNames[random.Next(ProdDepartments.DepartmentNames.Length)];
            var departmentIndex = Array.IndexOf(ProdDepartments.DepartmentNames, departmentName);
            var departmentAbbreviation = ProdDepartments.DepartmentAbbreviations[departmentIndex];
            var firstDigit = (departmentIndex + 1).ToString();
            var nextTwoDigits = random.Next(1, 100).ToString("D2");
            var sizeCode = new[] { "XS", "S", "M", "L", "XL" }[random.Next(5)];
            var colorCode = new[] { "BK", "BL", "GR", "RD", "YL", "OR", "WT", "GY" }[random.Next(8)];
            var manufacturingSite = ManufacturingSites.ManufacturingSitesList[random.Next(ManufacturingSites.ManufacturingSitesList.Length)];
            var productId = $"{departmentAbbreviation}-{firstDigit}{nextTwoDigits}-{sizeCode}-{colorCode}-{manufacturingSite}";
            var quantitySold = random.Next(1, 101);
            var unitPrice = random.Next(25, 300) + random.Next(0, 100) / 100m;
            var baseCost = unitPrice * (1 - random.Next(5, 21) / 100m);
            var volumeDiscount = (int)(quantitySold * 0.1);
            salesData.Add(new SalesData(dateSold, departmentName, productId, quantitySold, unitPrice, baseCost, volumeDiscount));
        }

        return salesData;
    }

    public void QuarterlySalesReport(List<SalesData> salesData)
    {
        var quarterlySales = new Dictionary<string, decimal>();
        var quarterlyProfit = new Dictionary<string, decimal>();
        var quarterlyProfitPercentage = new Dictionary<string, decimal>();

        var quarterlySalesByDepartment = new Dictionary<string, Dictionary<string, decimal>>();
        var quarterlyProfitByDepartment = new Dictionary<string, Dictionary<string, decimal>>();
        var quarterlyProfitPercentageByDepartment = new Dictionary<string, Dictionary<string, decimal>>();

        var top3SalesOrdersByQuarter = new Dictionary<string, List<SalesData>>();

        foreach (var data in salesData)
        {
            var quarter = GetQuarter(data.DateSold.Month);
            var totalSales = data.QuantitySold * data.UnitPrice;
            var totalCost = data.QuantitySold * data.BaseCost;
            var profit = totalSales - totalCost;
            var profitPercentage = (profit / totalSales) * 100;

            if (!quarterlySalesByDepartment.ContainsKey(quarter))
            {
                quarterlySalesByDepartment[quarter] = new Dictionary<string, decimal>();
                quarterlyProfitByDepartment[quarter] = new Dictionary<string, decimal>();
                quarterlyProfitPercentageByDepartment[quarter] = new Dictionary<string, decimal>();
            }

            if (!quarterlySalesByDepartment[quarter].ContainsKey(data.DepartmentName))
            {
                quarterlySalesByDepartment[quarter][data.DepartmentName] = 0;
                quarterlyProfitByDepartment[quarter][data.DepartmentName] = 0;
                quarterlyProfitPercentageByDepartment[quarter][data.DepartmentName] = 0;
            }

            quarterlySalesByDepartment[quarter][data.DepartmentName] += totalSales;
            quarterlyProfitByDepartment[quarter][data.DepartmentName] += profit;
            quarterlyProfitPercentageByDepartment[quarter][data.DepartmentName] = profitPercentage;

            if (!quarterlySales.ContainsKey(quarter))
            {
                quarterlySales[quarter] = 0;
                quarterlyProfit[quarter] = 0;
            }

            quarterlySales[quarter] += totalSales;
            quarterlyProfit[quarter] += profit;

            if (!top3SalesOrdersByQuarter.ContainsKey(quarter))
            {
                top3SalesOrdersByQuarter[quarter] = new List<SalesData>();
            }

            top3SalesOrdersByQuarter[quarter].Add(data);
        }

        foreach (var quarter in top3SalesOrdersByQuarter.Keys.ToList())
        {
            top3SalesOrdersByQuarter[quarter] = top3SalesOrdersByQuarter[quarter]
                .OrderByDescending(order => (order.QuantitySold * order.UnitPrice) - (order.QuantitySold * order.BaseCost))
                .Take(3)
                .ToList();
        }

        Console.WriteLine("Quarterly Sales Report");
        Console.WriteLine("----------------------");

        foreach (var kvp in quarterlySales.OrderBy(kvp => kvp.Key))
        {
            var quarter = kvp.Key;
            var salesAmount = kvp.Value;
            var profitAmount = quarterlyProfit[quarter];
            var profitPercentage = quarterlyProfitPercentage[quarter];

            Console.WriteLine($"{quarter}: Sales: {salesAmount:C2}, Profit: {profitAmount:C2}, Profit Percentage: {profitPercentage:F2}%");

            Console.WriteLine("By Department:");
            Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┐");
            Console.WriteLine("│      Department       │       Sales       │       Profit      │ Profit Percentage │");
            Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┤");

            foreach (var deptKvp in quarterlySalesByDepartment[quarter].OrderBy(kvp => kvp.Key))
            {
                var department = deptKvp.Key;
                var departmentSalesAmount = deptKvp.Value;
                var departmentProfitAmount = quarterlyProfitByDepartment[quarter][department];
                var departmentProfitPercentage = quarterlyProfitPercentageByDepartment[quarter][department];

                Console.WriteLine($"│ {department,-22}│ {departmentSalesAmount,17:C2} │ {departmentProfitAmount,17:C2} │ {departmentProfitPercentage,17:F2} │");
            }

            Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┘");
            Console.WriteLine();

            Console.WriteLine("Top 3 Sales Orders:");
            Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┐");
            Console.WriteLine("│      Product ID       │   Quantity Sold   │    Unit Price     │   Total Sales     │      Profit       │ Profit Percentage │");
            Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┤");

            foreach (var salesOrder in top3SalesOrdersByQuarter[quarter])
            {
                var orderTotalSales = salesOrder.QuantitySold * salesOrder.UnitPrice;
                var orderProfit = orderTotalSales - (salesOrder.QuantitySold * salesOrder.BaseCost);
                var orderProfitPercentage = (orderProfit / orderTotalSales) * 100;

                Console.WriteLine($"│ {salesOrder.ProductId,-22}│ {salesOrder.QuantitySold,17} │ {salesOrder.UnitPrice,17:C2} │ {orderTotalSales,17:C2} │ {orderProfit,17:C2} │ {orderProfitPercentage,17:F2} │");
            }

            Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┘");
            Console.WriteLine();
        }
    }

    public string GetQuarter(int month)
    {
        if (1 <= month && month <= 3)
        {
            return "Q1";
        }
        else if (4 <= month && month <= 6)
        {
            return "Q2";
        }
        else if (7 <= month && month <= 9)
        {
            return "Q3";
        }
        else
        {
            return "Q4";
        }
    }
}