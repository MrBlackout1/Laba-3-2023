using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    class Zavd1
    {
        static void Main()
        {
            var csvPath = "transactions.csv";
            var dateFormat = "dd/MM/yyyy";

            Func<string, DateTime> getDate = (line) =>
            {
                var dateStr = line.Split(',')[0];
                return DateTime.ParseExact(dateStr, dateFormat, null);
            };

            Func<string, double> getAmount = (line) =>
            {
                var amountStr = line.Split(',')[1];
                return double.Parse(amountStr);
            };

            Action<DateTime, double> printDailyTotal = (date, total) =>
            {
                Console.WriteLine($"Total for {date.ToString(dateFormat)}: {total}");
            };

            var transactionCount = 0;
            var dailyTotal = 0.0;
            var prevDate = DateTime.MinValue;
            using (var reader = new StreamReader(csvPath))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var date = getDate(line);
                    var amount = getAmount(line);

                    if (date != prevDate && prevDate != DateTime.MinValue)
                    {
                        printDailyTotal(prevDate, dailyTotal);
                        dailyTotal = 0.0;
                        transactionCount = 0;
                    }

                    dailyTotal += amount;
                    transactionCount++;

                    if (transactionCount == 10)
                    {
                        WriteDailyTotalToCsv(prevDate, dailyTotal);
                        dailyTotal = 0.0;
                        transactionCount = 0;
                    }

                    prevDate = date;
                }

                printDailyTotal(prevDate, dailyTotal);

                if (transactionCount > 0)
                {
                    WriteDailyTotalToCsv(prevDate, dailyTotal);
                }
            }
        }

        static void WriteDailyTotalToCsv(DateTime date, double total)
        {
            var csvPath = "daily_totals.csv";
            var dateFormat = "dd/MM/yyyy";
            Func<DateTime, string> formatDate = (d) => d.ToString(dateFormat);
            Func<double, string> formatAmount = (a) => a.ToString("0.00");
            using (var writer = new StreamWriter(csvPath, true))
            {
                writer.WriteLine($"{formatDate(date)}, {formatAmount(total)}");
            }
        }
    }

    class Zavd2
    {
        static void Main()
        {
            string basePath = @"C:\products\";
            List<Product> products = new List<Product>();
            decimal minPrice = 500.0m;
            decimal maxPrice = 1500.0m;
            string category = "electronics";
            Predicate<Product> filterCriteria = p =>
                p.Price >= minPrice && p.Price <= maxPrice && p.Category == category;
            Action<Product> displayProduct = p =>
            Console.WriteLine($"{p.Name}, {p.Price}, {p.Category}");
            for (int i = 1; i <= 10; i++)
            {
                string filePath = Path.Combine(basePath, $"{i}.json");
                string json = File.ReadAllText(filePath);
                List<Product> productsInFile = JsonConvert.DeserializeObject<List<Product>>(json);
                products.AddRange(productsInFile);
            }
            foreach (var product in products)
            {
                if (filterCriteria(product))
                {
                    displayProduct(product);
                }
            }
        }
        class Product
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Category { get; set; }
        }
    }

    class Zavd3
    {
        public static void Third()
        {
            string path = "pictures";
            List<Bitmap> images = LoadImages(path);

            Func<Bitmap, Bitmap> operation1 = InvertColors;
            Func<Bitmap, Bitmap> operation2 = AddBorder;
            Action<Bitmap> displayImage = DisplayImage;

            foreach (Bitmap image in images)
            {
                Bitmap result = operation1(image);
                result = operation2(result);
                displayImage(result);
            }
        }
        static List<Bitmap> LoadImages(string path)
        {
            List<Bitmap> images = new List<Bitmap>();
            List<string> files = Directory.GetFiles(path, "*.jpg").ToList();
            files.AddRange(Directory.GetFiles(path, "*.png"));
            foreach (string file in files)
            {
                Bitmap image = new Bitmap(file);
                images.Add(image);
            }
            return images;
        }
        static Bitmap InvertColors(Bitmap image)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    int red = 255 - color.R;
                    int green = 255 - color.G;
                    int blue = 255 - color.B;
                    result.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            return result;
        }
        static Bitmap AddBorder(Bitmap image)
        {
            Bitmap result = new Bitmap(image.Width + 10, image.Height + 10);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.FillRectangle(Brushes.White, 0, 0, result.Width, result.Height);
                g.DrawImage(image, new Point(5, 5));
            }
            return result;
        }
        static int[] cColors = { 0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF };

        public static void ConsoleWritePixel(Color cValue)
        {
            Color[] cTable = cColors.Select(x => Color.FromArgb(x)).ToArray();
            char[] rList = new char[] { (char)9617, (char)9618, (char)9619, (char)9608 };
            int[] bestHit = new int[] { 0, 0, 4, int.MaxValue };

            for (int rChar = rList.Length; rChar > 0; rChar--)
            {
                for (int cFore = 0; cFore < cTable.Length; cFore++)
                {
                    for (int cBack = 0; cBack < cTable.Length; cBack++)
                    {
                        int R = (cTable[cFore].R * rChar + cTable[cBack].R * (rList.Length - rChar)) / rList.Length;
                        int G = (cTable[cFore].G * rChar + cTable[cBack].G * (rList.Length - rChar)) / rList.Length;
                        int B = (cTable[cFore].B * rChar + cTable[cBack].B * (rList.Length - rChar)) / rList.Length;
                        int iScore = (cValue.R - R) * (cValue.R - R) + (cValue.G - G) * (cValue.G - G) + (cValue.B - B) * (cValue.B - B);
                        if (!(rChar > 1 && rChar < 4 && iScore > 50000))
                        {
                            if (iScore < bestHit[3])
                            {
                                bestHit[3] = iScore;
                                bestHit[0] = cFore;
                                bestHit[1] = cBack;
                                bestHit[2] = rChar;
                            }
                        }
                    }
                }
            }
            Console.ForegroundColor = (ConsoleColor)bestHit[0];
            Console.BackgroundColor = (ConsoleColor)bestHit[1];
            Console.Write(rList[bestHit[2] - 1]);
        }
        public static void DisplayImage(Bitmap source)
        {
            int sMax = 39;
            decimal percent = Math.Min(decimal.Divide(sMax, source.Width), decimal.Divide(sMax, source.Height));
            Size dSize = new Size((int)(source.Width * percent), (int)(source.Height * percent));
            Bitmap bmpMax = new Bitmap(source, dSize.Width * 2, dSize.Height);
            for (int i = 0; i < dSize.Height; i++)
            {
                for (int j = 0; j < dSize.Width; j++)
                {
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2, i));
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2 + 1, i));
                }
                System.Console.WriteLine();
            }
            Console.ResetColor();
        }
    }

    class Zavd4
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(@"C:\TextFiles", "*.txt");
            var tokenFunc = new Func<string, IEnumerable<string>>(TokenizeText);
            var frequencyFunc = new Func<IEnumerable<string>, IDictionary<string, int>>(CalculateWordFrequency);
            var displayAction = new Action<IDictionary<string, int>>(DisplayWordFrequency);

            foreach (var file in files)
            {
                var words = tokenFunc(file);
                var frequency = frequencyFunc(words);

                Console.WriteLine($"File: {file}");
                displayAction(frequency);
                Console.WriteLine();
            }
        }
        static IEnumerable<string> TokenizeText(string text)
        {
            var separators = new[] { ' ', '.', ',', '!', '?', ';', ':', '\r', '\n' };
            return text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }
        static IDictionary<string, int> CalculateWordFrequency(IEnumerable<string> words)
        {
            return words.GroupBy(w => w.ToLower())
                        .ToDictionary(g => g.Key, g => g.Count());
        }
        static void DisplayWordFrequency(IDictionary<string, int> frequency)
        {
            foreach (var word in frequency.OrderByDescending(kv => kv.Value))
            {
                Console.WriteLine($"'{word.Key}': {word.Value}");
            }
        }
    }
}