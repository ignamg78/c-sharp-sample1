using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerator
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
        //I need a public struct ProdDepartments that contains a static string array for 8 clothing industry departments. Create separate string array containing 4-character abbreviations for each department name. The abbreviation must be unique. The department names should represent real department names for the clothing industry.

        public struct ProdDepartments
        {
            public static string[] departments = { "Menswear", "Womenswear", "Children", "Outerwear", "Footwear", "Accessories", "Underwear", "Sportswear" };
            public static string[] abbreviations = { "MENS", "WMNS", "CHLD", "OUTR", "FTWR", "ACCS", "UNDR", "SPRT" };
        }
        public struct ManufacturingSites
        {
            public static string[] manSites = { "US1", "US2", "US3", "CA1", "CA2", "CA3", "MX1", "MX2", "MX3", "MX4" };
        }

        /* the GenerateSalesData method returns 1000 SalesData records. It assigns random values to each field of the data structure */
        public SalesData[] GenerateSalesData()
        {
            SalesData[] salesData = new SalesData[1000];
            Random random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                salesData[i].dateSold = new DateOnly(2023, random.Next(1, 13), random.Next(1, 29));
                salesData[i].departmentName = ProdDepartments.departments[random.Next(ProdDepartments.departments.Length)];

                int indexOfDept = Array.IndexOf(ProdDepartments.departments, salesData[i].departmentName);
                string deptAbb = ProdDepartments.abbreviations[indexOfDept];
                string firstDigit = (indexOfDept + 1).ToString();
                string nextTwoDigits = "";
                nextTwoDigits = random.Next(1, 100).ToString("D2");                 
                string[] sizes = { "XS", "S", "M", "L", "XL" };
                string sizeCode = sizes[random.Next(sizes.Length)];                
                string[] colors = { "BK", "BL", "GR", "RD", "YL", "OR", "WT", "GY" };
                string colorCode = colors[random.Next(colors.Length)];
                string manufacturingSite = ManufacturingSites.manSites[random.Next(ManufacturingSites.manSites.Length)];

                
                salesData[i].productID = $"{deptAbb}-{firstDigit}{nextTwoDigits}-{sizeCode}-{colorCode}-{manufacturingSite}";
                salesData[i].quantitySold = random.Next(1, 101);
                salesData[i].unitPrice = random.Next(25, 300) + random.NextDouble();
                double discountPercentage = random.Next(5, 21) / 100.0;
                salesData[i].baseCost = salesData[i].unitPrice * (1 - discountPercentage);
                salesData[i].volumeDiscount = (int)(salesData[i].quantitySold * 0.1);
            }

            return salesData;
        }

                public void QuarterlySalesReport(SalesData[] salesData)
        {
            // Crear diccionarios para almacenar las ventas y ganancias trimestrales
            Dictionary<string, double> quarterlySales = new Dictionary<string, double>();
            Dictionary<string, double> quarterlyProfit = new Dictionary<string, double>();
            Dictionary<string, Dictionary<string, double>> quarterlySalesByDept = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> quarterlyProfitByDept = new Dictionary<string, Dictionary<string, double>>();
        
            // Iterar a través de los datos de ventas
            foreach (SalesData data in salesData)
            {
                // Calcular las ventas y ganancias totales para cada trimestre
                string quarter = GetQuarter(data.dateSold.Month);
                double totalSales = data.quantitySold * data.unitPrice;
                double totalProfit = (data.unitPrice - data.baseCost) * data.quantitySold;
        
                // Actualizar las ventas y ganancias trimestrales
                if (quarterlySales.ContainsKey(quarter))
                {
                    quarterlySales[quarter] += totalSales;
                    quarterlyProfit[quarter] += totalProfit;
                }
                else
                {
                    quarterlySales.Add(quarter, totalSales);
                    quarterlyProfit.Add(quarter, totalProfit);
                }
        
                // Inicializar diccionarios para ventas y ganancias por departamento si no existen
                if (!quarterlySalesByDept.ContainsKey(quarter))
                {
                    quarterlySalesByDept[quarter] = new Dictionary<string, double>();
                    quarterlyProfitByDept[quarter] = new Dictionary<string, double>();
                }
        
                // Actualizar las ventas y ganancias trimestrales por departamento
                if (quarterlySalesByDept[quarter].ContainsKey(data.departmentName))
                {
                    quarterlySalesByDept[quarter][data.departmentName] += totalSales;
                    quarterlyProfitByDept[quarter][data.departmentName] += totalProfit;
                }
                else
                {
                    quarterlySalesByDept[quarter].Add(data.departmentName, totalSales);
                    quarterlyProfitByDept[quarter].Add(data.departmentName, totalProfit);
                }
            }
        
            // Mostrar el informe de ventas trimestrales
            Console.WriteLine("Quarterly Sales Report");
            Console.WriteLine("----------------------");
        
            // Ordenar los trimestres en orden
            string[] quarters = { "Q1", "Q2", "Q3", "Q4" };
            foreach (string quarter in quarters)
            {
                if (quarterlySales.TryGetValue(quarter, out double sales) && quarterlyProfit.TryGetValue(quarter, out double profit))
                {
                    double profitPercentage = (profit / sales) * 100;
                    Console.WriteLine("{0}: Sales: ${1:N2}, Profit: ${2:N2}, Profit Percentage: {3:N2}%", quarter, sales, profit, profitPercentage);
        
                    // Mostrar las ventas y ganancias por departamento
                    Console.WriteLine("  By Department:");
                    foreach (var dept in quarterlySalesByDept[quarter].OrderBy(d => d.Key))
                    {
                        double deptSales = dept.Value;
                        double deptProfit = quarterlyProfitByDept[quarter][dept.Key];
                        double deptProfitPercentage = (deptProfit / deptSales) * 100;
                        Console.WriteLine("    {0}: Sales: ${1:N2}, Profit: ${2:N2}, Profit Percentage: {3:N2}%", dept.Key, deptSales, deptProfit, deptProfitPercentage);
                    }
                }
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
