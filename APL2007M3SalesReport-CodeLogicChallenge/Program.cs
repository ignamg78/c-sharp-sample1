﻿namespace ReportGenerator
{
    class QuarterlyIncomeReport
    {
        static void Main(string[] args)
        {
            // create a new instance of the class
            QuarterlyIncomeReport report = new QuarterlyIncomeReport();

            // call the GenerateSalesData method
            SalesData[] salesData = report.GenerateSalesData();


            // call the QuarterlySalesReport method
            report.QuarterlySalesReport(salesData);

        }

        /* public struct SalesData includes the following fields: date sold, department name, product ID, quantity sold, unit price */
        public struct SalesData
        {
            public DateOnly dateSold;
            public string departmentName;
            public string productID;
            public int quantitySold;
            public double unitPrice;
            public double baseCost;
            public int volumeDiscount;
        }

        public struct ProdDepartments
        {
            public static string[] departmentNames = { "Men's Clothing", "Women's Clothing", "Children's Clothing", "Accessories", "Footwear", "Outerwear", "Sportswear", "Undergarments" };
            public static string[] departmentAbbreviations = { "MENS", "WOMN", "CHLD", "ACCS", "FOOT", "OUTR", "SPRT", "UNDR" };
        }

        public struct ManufacturingSites
        {
            public static string[] manufacturingSites = { "US1", "US2", "US3", "UK1", "UK2", "UK3", "JP1", "JP2", "JP3", "CA1" };
        }

        /* the GenerateSalesData method returns 1000 SalesData records. It assigns random values to each field of the data structure */
        public SalesData[] GenerateSalesData()
        {
            SalesData[] salesData = new SalesData[1000];
            Random random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                salesData[i].dateSold = new DateOnly(2023, random.Next(1, 13), random.Next(1, 29));
                salesData[i].departmentName = ProdDepartments.departmentNames[random.Next(0, ProdDepartments.departmentNames.Length)];
                salesData[i].quantitySold = random.Next(1, 101);
                salesData[i].productID = ConstructProductId(salesData[i]);
                salesData[i].unitPrice = random.Next(25, 300) + random.NextDouble();
                salesData[i].baseCost = salesData[i].unitPrice * (1 - (random.Next(5, 21) / 100.0));
                salesData[i].volumeDiscount = (int)(salesData[i].quantitySold * 0.1);

            }

            return salesData;
        }

        public string ConstructProductId(SalesData salesData)
        {
            Random random = new Random();
            int indexOfDept = Array.IndexOf(ProdDepartments.departmentNames, salesData.departmentName);
            string deptAbb = ProdDepartments.departmentAbbreviations[indexOfDept];
            string firstDigit = (indexOfDept + 1).ToString();
            string nextTwoDigits = random.Next(1, 100).ToString("D2");
            string sizeCode = new string[] { "XS", "S", "M", "L", "XL" }[random.Next(0, 5)];
            string colorCode = new string[] { "BK", "BL", "GR", "RD", "YL", "OR", "WT", "GY" }[random.Next(0, 8)];
            string manufacturingSite = ManufacturingSites.manufacturingSites[random.Next(0, ManufacturingSites.manufacturingSites.Length)];

            return $"{deptAbb}-{firstDigit}{nextTwoDigits}-{sizeCode}-{colorCode}-{manufacturingSite}";
        }

        public string[,] DeconstructProductId(string productId)
        {
            // Asumiendo que el productId tiene el formato "DeptAbbr-Serial-Size-Color-Site"
            string[] components = productId.Split('-');
        
            if (components.Length != 5)
            {
                throw new ArgumentException("El productId no tiene el formato esperado.");
            }
        
            string[,] result = new string[5, 2]
            {
                { "Department Abbreviation", components[0] },
                { "Product Serial Number", components[1] },
                { "Size Code", components[2] },
                { "Color Code", components[3] },
                { "Manufacturing Site", components[4] }
            };
        
            return result;
        }

                public void QuarterlySalesReport(SalesData[] salesData)
        {
            // create a dictionary to store the quarterly sales data
            Dictionary<string, double> quarterlySales = new Dictionary<string, double>();
            Dictionary<string, double> quarterlyProfit = new Dictionary<string, double>();
            Dictionary<string, double> quarterlyProfitPercentage = new Dictionary<string, double>();
        
            // create a dictionary to store the quarterly sales data by department
            Dictionary<string, Dictionary<string, double>> quarterlySalesByDepartment = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> quarterlyProfitByDepartment = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> quarterlyProfitPercentageByDepartment = new Dictionary<string, Dictionary<string, double>>();
        
            // create a dictionary to store the top 3 sales orders by quarter
            Dictionary<string, List<SalesData>> top3SalesOrdersByQuarter = new Dictionary<string, List<SalesData>>();
        
            // create a dictionary to store the most profitable product by quarter
            Dictionary<string, Dictionary<string, (int unitsSold, double totalSales, double avgUnitCost, double totalProfit, double avgProfitPercentage)>> mostProfitableProductByQuarter = new Dictionary<string, Dictionary<string, (int, double, double, double, double)>>();
        
            // iterate through the sales data
            foreach (SalesData data in salesData)
            {
                // calculate the total sales for each quarter
                string quarter = GetQuarter(data.dateSold.Month);
                double totalSales = data.quantitySold * data.unitPrice;
                double totalCost = data.quantitySold * data.baseCost;
                double profit = totalSales - totalCost;
                double profitPercentage = (profit / totalSales) * 100;
        
                // Deconstruct the product ID
                string[,] components = DeconstructProductId(data.productID);
                string uniqueProductSerial = $"{components[0, 1]}-{components[1, 1]}-ss-cc-mmm";
        
                // calculate the total sales, profit, and profit percentage by department
                if (!quarterlySalesByDepartment.ContainsKey(quarter))
                {
                    quarterlySalesByDepartment.Add(quarter, new Dictionary<string, double>());
                    quarterlyProfitByDepartment.Add(quarter, new Dictionary<string, double>());
                    quarterlyProfitPercentageByDepartment.Add(quarter, new Dictionary<string, double>());
                }
        
                if (quarterlySalesByDepartment[quarter].ContainsKey(data.departmentName))
                {
                    quarterlySalesByDepartment[quarter][data.departmentName] += totalSales;
                    quarterlyProfitByDepartment[quarter][data.departmentName] += profit;
                }
                else
                {
                    quarterlySalesByDepartment[quarter].Add(data.departmentName, totalSales);
                    quarterlyProfitByDepartment[quarter].Add(data.departmentName, profit);
                }
        
                if (!quarterlyProfitPercentageByDepartment[quarter].ContainsKey(data.departmentName))
                {
                    quarterlyProfitPercentageByDepartment[quarter].Add(data.departmentName, profitPercentage);
                }
        
                // calculate the total sales and profit for each quarter
                if (quarterlySales.ContainsKey(quarter))
                {
                    quarterlySales[quarter] += totalSales;
                    quarterlyProfit[quarter] += profit;
                }
                else
                {
                    quarterlySales.Add(quarter, totalSales);
                    quarterlyProfit.Add(quarter, profit);
                }
        
                if (!quarterlyProfitPercentage.ContainsKey(quarter))
                {
                    quarterlyProfitPercentage.Add(quarter, profitPercentage);
                }
        
                // add the sales data to the top 3 sales orders by quarter
                if (!top3SalesOrdersByQuarter.ContainsKey(quarter))
                {
                    top3SalesOrdersByQuarter.Add(quarter, new List<SalesData>());
                }
        
                top3SalesOrdersByQuarter[quarter].Add(data);
        
                // update the most profitable product by quarter
                if (!mostProfitableProductByQuarter.ContainsKey(quarter))
                {
                    mostProfitableProductByQuarter.Add(quarter, new Dictionary<string, (int, double, double, double, double)>());
                }
        
                if (mostProfitableProductByQuarter[quarter].ContainsKey(uniqueProductSerial))
                {
                    var currentData = mostProfitableProductByQuarter[quarter][uniqueProductSerial];
                    int newUnitsSold = currentData.unitsSold + data.quantitySold;
                    double newTotalSales = currentData.totalSales + totalSales;
                    double newTotalProfit = currentData.totalProfit + profit;
                    double newAvgUnitCost = (currentData.avgUnitCost * currentData.unitsSold + data.baseCost * data.quantitySold) / newUnitsSold;
                    double newAvgProfitPercentage = (currentData.avgProfitPercentage * currentData.unitsSold + profitPercentage * data.quantitySold) / newUnitsSold;
        
                    mostProfitableProductByQuarter[quarter][uniqueProductSerial] = (newUnitsSold, newTotalSales, newAvgUnitCost, newTotalProfit, newAvgProfitPercentage);
                }
                else
                {
                    mostProfitableProductByQuarter[quarter][uniqueProductSerial] = (data.quantitySold, totalSales, data.baseCost, profit, profitPercentage);
                }
            }
        
            // sort the top 3 sales orders by profit in descending order
            foreach (var quarter in top3SalesOrdersByQuarter.Keys)
            {
                top3SalesOrdersByQuarter[quarter] = top3SalesOrdersByQuarter[quarter]
                    .OrderByDescending(order => (order.quantitySold * order.unitPrice) - (order.quantitySold * order.baseCost))
                    .Take(3)
                    .ToList();
            }
        
            // display the quarterly sales report
            Console.WriteLine("Quarterly Sales Report");
            Console.WriteLine("----------------------");
        
            // sort the quarterly sales by key (quarter)
            var sortedQuarterlySales = quarterlySales.OrderBy(q => q.Key);
        
            foreach (KeyValuePair<string, double> quarter in sortedQuarterlySales)
            {
                // format the sales amount as currency using regional settings
                string formattedSalesAmount = quarter.Value.ToString("C");
                string formattedProfitAmount = quarterlyProfit[quarter.Key].ToString("C");
                string formattedProfitPercentage = quarterlyProfitPercentage[quarter.Key].ToString("F2");
        
                Console.WriteLine("{0}: Sales: {1}, Profit: {2}, Profit Percentage: {3}%", quarter.Key, formattedSalesAmount, formattedProfitAmount, formattedProfitPercentage);
        
                // display the quarterly sales, profit, and profit percentage by department
                Console.WriteLine("By Department:");
                var sortedQuarterlySalesByDepartment = quarterlySalesByDepartment[quarter.Key].OrderBy(d => d.Key);
        
                // Print table headers
                Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┐");
                Console.WriteLine("│      Department       │       Sales       │       Profit      │ Profit Percentage │");
                Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┤");
        
                foreach (KeyValuePair<string, double> department in sortedQuarterlySalesByDepartment)
                {
                    string formattedDepartmentSalesAmount = department.Value.ToString("C");
                    string formattedDepartmentProfitAmount = quarterlyProfitByDepartment[quarter.Key][department.Key].ToString("C");
                    string formattedDepartmentProfitPercentage = quarterlyProfitPercentageByDepartment[quarter.Key][department.Key].ToString("F2");
        
                    Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,17} │", department.Key, formattedDepartmentSalesAmount, formattedDepartmentProfitAmount, formattedDepartmentProfitPercentage);
                }
        
                Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┘");
                Console.WriteLine();
        
                // display the top 3 sales orders for the quarter
                Console.WriteLine("Top 3 Sales Orders:");
                var top3SalesOrders = top3SalesOrdersByQuarter[quarter.Key];
        
                // Print table headers
                Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┐");
                Console.WriteLine("│      Product ID       │   Quantity Sold   │    Unit Price     │   Total Sales     │      Profit       │ Profit Percentage │");
                Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┤");
        
                foreach (SalesData salesOrder in top3SalesOrders)
                {
                    double orderTotalSales = salesOrder.quantitySold * salesOrder.unitPrice;
                    double orderProfit = orderTotalSales - (salesOrder.quantitySold * salesOrder.baseCost);
                    double orderProfitPercentage = (orderProfit / orderTotalSales) * 100;
        
                    Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,17} │ {4,17} │ {5,17} │", salesOrder.productID, salesOrder.quantitySold, salesOrder.unitPrice.ToString("C"), orderTotalSales.ToString("C"), orderProfit.ToString("C"), orderProfitPercentage.ToString("F2"));
                }
        
                Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┘");
                Console.WriteLine();
        
                // display the top 3 most profitable products for the quarter
                Console.WriteLine("Top 3 Most Profitable Products:");
                var top3ProfitableProducts = mostProfitableProductByQuarter[quarter.Key]
                    .OrderByDescending(p => p.Value.totalProfit)
                    .Take(3);
        
                // Print table headers
                Console.WriteLine("┌──────────────────────────────┬───────────────┬───────────────┬───────────────┬───────────────┬────────────────────┐");
                Console.WriteLine("│      Product Serial Number   │   Units Sold  │   Total Sales │ Avg Unit Cost │  Total Profit │ Avg Profit Percent │");
                Console.WriteLine("├──────────────────────────────┼───────────────┼───────────────┼───────────────┼───────────────┼────────────────────┤");
        
                foreach (var product in top3ProfitableProducts)
                {
                    Console.WriteLine("│ {0,-28} │ {1,13} │ {2,13:C} │ {3,13:C} │ {4,13:C} │ {5,18:F2} │",
                        product.Key,
                        product.Value.unitsSold,
                        product.Value.totalSales,
                        product.Value.avgUnitCost,
                        product.Value.totalProfit,
                        product.Value.avgProfitPercentage);
                }
        
                Console.WriteLine("└──────────────────────────────┴───────────────┴───────────────┴───────────────┴───────────────┴────────────────────┘");
                Console.WriteLine();
            }
        }

        public string GetQuarter(int month)
        {
            if (month >= 1 && month <= 3)
            {
                return "Q1";
            }
            else if (month >= 4 && month <= 6)
            {
                return "Q2";
            }
            else if (month >= 7 && month <= 9)
            {
                return "Q3";
            }
            else
            {
                return "Q4";
            }
        }
    }
}
