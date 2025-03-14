namespace ReportGenerator
{
    class QuarterlyIncomeReport
    {
        private bool isAuthenticated;
        private string currentUser;

        public QuarterlyIncomeReport()
        {
            isAuthenticated = false;
            currentUser = string.Empty;
        }

        public bool Login(string username, string password)
        {
            try
            {
                // Aquí deberías implementar tu lógica real de autenticación
                // Este es solo un ejemplo simplificado
                if (username == "admin" && password == "password123")
                {
                    isAuthenticated = true;
                    currentUser = username;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la autenticación: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            isAuthenticated = false;
            currentUser = string.Empty;
        }

        private void CheckAuthentication()
        {
            if (!isAuthenticated)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado. Por favor, inicie sesión primero.");
            }
        }

        static void Main(string[] args)
        {
            try
            {
                // create a new instance of the class
                QuarterlyIncomeReport report = new QuarterlyIncomeReport();

                // Autenticar al usuario
                if (!report.Login("admin", "password123"))
                {
                    Console.WriteLine("Error de autenticación. No se puede generar el informe.");
                    return;
                }

                // call the GenerateSalesData method
                SalesData[] salesData = report.GenerateSalesData();

                // call the QuarterlySalesReport method
                report.QuarterlySalesReport(salesData);

                // Cerrar sesión
                report.Logout();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error de acceso: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
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
            try
            {
                // Verificar autenticación antes de proceder
                CheckAuthentication();

                SalesData[] salesData = new SalesData[100000];
                Random random = new Random();

                for (int i = 0; i < 100000; i++)
                {
                    salesData[i].dateSold = new DateOnly(2023, random.Next(1, 13), random.Next(1, 29));
                    salesData[i].departmentName = ProdDepartments.departmentNames[random.Next(0, ProdDepartments.departmentNames.Length)];
                    salesData[i].productID = ConstructProductId(salesData[i], random);
                    salesData[i].quantitySold = random.Next(1, 101);
                    salesData[i].unitPrice = random.Next(25, 300) + random.NextDouble();
                    salesData[i].baseCost = salesData[i].unitPrice * (1 - (random.Next(5, 21) / 100.0));
                    salesData[i].volumeDiscount = (int)(salesData[i].quantitySold * 0.1);
                }

                return salesData;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error de acceso: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando datos de ventas: {ex.Message}");
                throw;
            }
        }

        public static string ConstructProductId(SalesData salesDataItem, Random random)
        {
            int indexOfDept = Array.IndexOf(ProdDepartments.departmentNames, salesDataItem.departmentName);
            string deptAbb = ProdDepartments.departmentAbbreviations[indexOfDept];
            string firstDigit = (indexOfDept + 1).ToString();
            string nextTwoDigits = random.Next(1, 100).ToString("D2");
            string sizeCode = new string[] { "XS", "S", "M", "L", "XL" }[random.Next(0, 5)];
            string colorCode = new string[] { "BK", "BL", "GR", "RD", "YL", "OR", "WT", "GY" }[random.Next(0, 8)];
            string manufacturingSite = ManufacturingSites.manufacturingSites[random.Next(0, ManufacturingSites.manufacturingSites.Length)];
        
            return $"{deptAbb}-{firstDigit}{nextTwoDigits}-{sizeCode}-{colorCode}-{manufacturingSite}";
        }

        public static string[,] DeconstructProductId(string productID)
        {
            try
            {
                if (string.IsNullOrEmpty(productID))
                    throw new ArgumentNullException(nameof(productID), "El ID del producto no puede ser nulo o vacío");

                string[] parts = productID.Split('-');
                if (parts.Length != 5)
                    throw new ArgumentException("Formato de ID de producto inválido", nameof(productID));

                string[,] deconstructedProduct = new string[5, 2];

                deconstructedProduct[0, 0] = "Department Abbreviation";
                deconstructedProduct[0, 1] = parts[0];

                deconstructedProduct[1, 0] = "Product Serial Number";
                deconstructedProduct[1, 1] = parts[1];

                deconstructedProduct[2, 0] = "Size Code";
                deconstructedProduct[2, 1] = parts[2];

                deconstructedProduct[3, 0] = "Color Code";
                deconstructedProduct[3, 1] = parts[3];

                deconstructedProduct[4, 0] = "Manufacturing Site";
                deconstructedProduct[4, 1] = parts[4];

                return deconstructedProduct;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decodificando ID de producto: {ex.Message}");
                throw;
            }
        }

        public void QuarterlySalesReport(SalesData[] salesData)
        {
            try
            {
                // Verificar autenticación antes de proceder
                CheckAuthentication();

                // Registrar el acceso
                Console.WriteLine($"Generando informe para el usuario: {currentUser}");
                Console.WriteLine("Fecha y hora: " + DateTime.Now);
                Console.WriteLine();

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

                // create a dictionary to store the quarterly top profit for product numbers
                Dictionary<string, Dictionary<string, (int, double, double, double, double)>> quarterlyTopProfitForProductNumbers = new Dictionary<string, Dictionary<string, (int, double, double, double, double)>>();

                // iterate through the sales data
                foreach (SalesData data in salesData)
                {
                    // calculate the total sales for each quarter
                    string quarter = GetQuarter(data.dateSold.Month);
                    double totalSales = data.quantitySold * data.unitPrice;
                    double totalCost = data.quantitySold * data.baseCost;
                    double profit = totalSales - totalCost;
                    double profitPercentage = (profit / totalSales) * 100;

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

                    // update the quarterly top profit for product numbers
                    string[,] deconstructedProductId = DeconstructProductId(data.productID);
                    string productSerialNumber = deconstructedProductId[0, 1] + "-" + deconstructedProductId[1, 1] + "-ss-cc-mmm";

                    if (!quarterlyTopProfitForProductNumbers.ContainsKey(quarter))
                    {
                        quarterlyTopProfitForProductNumbers.Add(quarter, new Dictionary<string, (int, double, double, double, double)>());
                    }

                    if (quarterlyTopProfitForProductNumbers[quarter].ContainsKey(productSerialNumber))
                    {
                        var (unitsSold, totalSalesAmount, unitCost, totalProfit, innerProfitPercentage) = quarterlyTopProfitForProductNumbers[quarter][productSerialNumber];
                        unitsSold += data.quantitySold;
                        totalSalesAmount += totalSales;
                        unitCost = totalSalesAmount / unitsSold;
                        totalProfit += profit;
                        innerProfitPercentage = (totalProfit / totalSalesAmount) * 100;
                        quarterlyTopProfitForProductNumbers[quarter][productSerialNumber] = (unitsSold, totalSalesAmount, unitCost, totalProfit, innerProfitPercentage);
                    }
                    else
                    {
                        quarterlyTopProfitForProductNumbers[quarter].Add(productSerialNumber, (data.quantitySold, totalSales, data.baseCost, profit, profitPercentage));
                    }
                }

                // Call the new method to display the reports
                DisplayReports(quarterlySales, quarterlyProfit, quarterlyProfitPercentage, quarterlySalesByDepartment, quarterlyProfitByDepartment, quarterlyProfitPercentageByDepartment, top3SalesOrdersByQuarter, quarterlyTopProfitForProductNumbers);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error de acceso: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando el informe: {ex.Message}");
                throw;
            }
        }

        public bool DisplayReports(
            Dictionary<string, double> quarterlySales,
            Dictionary<string, double> quarterlyProfit,
            Dictionary<string, double> quarterlyProfitPercentage,
            Dictionary<string, Dictionary<string, double>> quarterlySalesByDepartment,
            Dictionary<string, Dictionary<string, double>> quarterlyProfitByDepartment,
            Dictionary<string, Dictionary<string, double>> quarterlyProfitPercentageByDepartment,
            Dictionary<string, List<SalesData>> top3SalesOrdersByQuarter,
            Dictionary<string, Dictionary<string, (int, double, double, double, double)>> quarterlyTopProfitForProductNumbers)
        {
            try
            {
                // Verificar autenticación antes de proceder
                CheckAuthentication();

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
                    TopThree(top3SalesOrdersByQuarter[quarter.Key]);

                    // display the quarterly top profit for product numbers
                    Console.WriteLine("Quarterly Top Profit for Product Numbers:");
                    var quarterlyTopProfit = quarterlyTopProfitForProductNumbers[quarter.Key];

                    // Sort the quarterly top profit by profit in descending order
                    var sortedQuarterlyTopProfit = quarterlyTopProfit.OrderByDescending(p => p.Value.Item4).Take(3);

                    // Print table headers
                    Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┐");
                    Console.WriteLine("│   Product Serial No   │    Units Sold     │   Total Sales     │    Unit Cost      │     Total Profit  │ Profit Percentage │");
                    Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┤");

                    foreach (KeyValuePair<string, (int, double, double, double, double)> product in sortedQuarterlyTopProfit)
                    {
                        var (unitsSold, totalSalesAmount, unitCost, totalProfit, profitPercentage) = product.Value;

                        Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,17} │ {4,17} │ {5,17} │", product.Key, unitsSold, totalSalesAmount.ToString("C"), unitCost.ToString("C"), totalProfit.ToString("C"), profitPercentage.ToString("F2"));
                    }

                    Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┘");
                    Console.WriteLine();
                }

                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error de acceso: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying reports: {ex.Message}");
                return false;
            }
        }

        public void TopThree(List<SalesData> salesOrders)
        {
            try
            {
                // Verificar autenticación antes de proceder                
                CheckAuthentication();

                if (salesOrders == null)
                    throw new ArgumentNullException(nameof(salesOrders), "La lista de órdenes de venta no puede ser nula");

                if (salesOrders.Count == 0)
                    throw new ArgumentException("La lista de órdenes de venta está vacía", nameof(salesOrders));

                Console.WriteLine("Top 3 Sales Orders:");
                var top3SalesOrders = salesOrders.OrderByDescending(o => o.quantitySold * o.unitPrice).Take(3);

                Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┐");
                Console.WriteLine("│      Product ID       │   Quantity Sold   │    Unit Price     │   Total Sales     │      Profit       │ Profit Percentage │");
                Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┤");

                foreach (SalesData salesOrder in top3SalesOrders)
                {
                    if (salesOrder.quantitySold <= 0)
                        throw new ArgumentException($"Cantidad inválida para el producto {salesOrder.productID}");

                    if (salesOrder.unitPrice <= 0)
                        throw new ArgumentException($"Precio unitario inválido para el producto {salesOrder.productID}");

                    double orderTotalSales = salesOrder.quantitySold * salesOrder.unitPrice;
                    double orderProfit = orderTotalSales - (salesOrder.quantitySold * salesOrder.baseCost);
                    double orderProfitPercentage = (orderProfit / orderTotalSales) * 100;

                    Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,17} │ {4,17} │ {5,17} │", 
                        salesOrder.productID, 
                        salesOrder.quantitySold, 
                        salesOrder.unitPrice.ToString("C"), 
                        orderTotalSales.ToString("C"), 
                        orderProfit.ToString("C"), 
                        orderProfitPercentage.ToString("F2"));
                }

                Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┘");
                Console.WriteLine();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error de acceso: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando el Top 3 de ventas: {ex.Message}");
                throw;
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
