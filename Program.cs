using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
//Test
namespace Budgeteer
{
    static class CSB
    {
        public static class Column
        {
            public static int Number = 0;
            public static int Date = 1;
            public static int Description = 2;
            public static int Memo = 3;
            public static int Debit = 4;
            public static int Credit = 5;
        };
    }
    static class CapitalOne
    {
        public static class Column
        {
            public static int Date = 0;
            public static int PostedDate = 1;
            public static int Card = 2;
            public static int Description = 3;
            public static int Category = 4;
            public static int Debit = 5;
            public static int Credit = 6;
        };
    }
    static class CitiBank
    {
        public static class Column
        {
            public static int Status = 0;
            public static int Date = 1;
            public static int Description = 2;
            public static int Debit = 3;
            public static int Credit = 4;
        };
    }
    static class PayPal
    {
        public static class Column
        {
            public static int Date = 0;
            public static int Time = 1;
            public static int Timezone = 2;
            public static int Description = 3;
            public static int Type = 4;
            public static int Status = 5;
            public static int Currency = 6;
            public static int Amount = 7;
            public static int ReceiptId = 8;
            public static int Balance = 9;
        };
    }
    class Program
    {
        const string CSBEXPORT_LOCATION = "Exports/CSBExport";
        const string CAPONEEXPORT_LOCATION = "Exports/CapitalOneExport";
        const string CITIEXPORT_LOCATION = "Exports/CitiExport";
        const string PAYPALEXPORT_LOCATION = "Exports/PayPalExport";
        
        static void Main(string[] args)
        {
            Boolean isCorrectRows = false;
            BudgetMonth thisMonth = new BudgetMonth();
            FileStream notFound = new FileStream("NotFound/NotFoundLines.csv", FileMode.Create);
            StreamWriter writer = new StreamWriter(notFound);
            var change = "yep";
            
            string[] csbfiles = Directory.GetFiles(CSBEXPORT_LOCATION);
            foreach (string f in csbfiles)
            {
                writer.WriteLine("CSB File");
                using (StreamReader reader = new StreamReader(f))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (isCorrectRows)
                        {
                            if (!ParseCSBLine(ref thisMonth, line))
                            {
                                writer.WriteLine(line);
                            }
                        }

                        if (!isCorrectRows && line.Contains("Transaction Number"))
                        {
                            isCorrectRows = true;
                        }
                    }
                }
            }

            string[] caponefiles = Directory.GetFiles(CAPONEEXPORT_LOCATION);
            foreach (string f in caponefiles)
            {
                writer.WriteLine("Capital One File");
                isCorrectRows = false;
                using (StreamReader reader = new StreamReader(f))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (isCorrectRows)
                        {
                            if (!ParseCapOneLine(ref thisMonth, line))
                            {
                                writer.WriteLine(line);
                            }
                        }

                        if (!isCorrectRows && line.Contains("Transaction Date"))
                        {
                            isCorrectRows = true;
                        }
                    }
                }
            }
            
            string[] citifiles = Directory.GetFiles(CITIEXPORT_LOCATION);
            foreach (string f in citifiles)
            {
                writer.WriteLine("CitiBank File");
                isCorrectRows = false;
                using (StreamReader reader = new StreamReader(f))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (isCorrectRows)
                        {
                            var secondLine = reader.ReadLine(); //CitiBank has new line characters in their description field
                            if (!ParseCitiLine(ref thisMonth, line + secondLine))
                            {
                                writer.WriteLine(line + secondLine);
                            }
                        }

                        if (!isCorrectRows && line.Contains("Status"))
                        {
                            isCorrectRows = true;
                        }
                    }
                }
            }

            
            string[] paypalfiles = Directory.GetFiles(PAYPALEXPORT_LOCATION);
            foreach (string f in paypalfiles)
            {
                writer.WriteLine("PayPal File");
                isCorrectRows = false;
                using (StreamReader reader = new StreamReader(f))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (isCorrectRows)
                        {
                            if (!ParsePayPalLine(ref thisMonth, line))
                            {
                                writer.WriteLine(line);
                            }
                        }

                        if (!isCorrectRows && line.Contains("Date"))
                        {
                            isCorrectRows = true;
                        }
                    }
                }
            }


            writer.Close();

            var notfoundpath = AppDomain.CurrentDomain.BaseDirectory + "NotFound/NotFoundLines.csv";
            Console.WriteLine(thisMonth.ToString());
            Process.Start("C:/Program Files (x86)/OpenOffice.org 3/program/scalc.exe", "-view NotFound/NotFoundLines.csv");
            Console.ReadLine();
        }

        private static bool ParsePayPalLine(ref BudgetMonth month, string line)
        {
            string[] entry = line.Split(',');
            Boolean result = false;

            if (!entry[PayPal.Column.Description].Contains("Bank Account"))
            {
                var debit = Math.Abs(Decimal.Parse(entry[PayPal.Column.Amount].Replace("\"", "")));
                result = UpdateCategory(ref month, entry[PayPal.Column.Description], debit);
            }
            return result;
        }
        private static bool ParseCitiLine(ref BudgetMonth month, string line)
        {
            string[] entry = line.Split(',');
            Boolean result = false;

            if (entry[CitiBank.Column.Credit].Replace("\"", "") == string.Empty  && !entry[CitiBank.Column.Description].Contains("ONLINE PAYMENT"))
            {
                var debit = Math.Abs(Decimal.Parse(entry[CitiBank.Column.Debit].Replace("\"", "")));
                result = UpdateCategory(ref month, entry[CitiBank.Column.Description], debit);
            }
            return result;
        }

        private static Boolean ParseCapOneLine(ref BudgetMonth month, string line)
        {
            string[] entry = line.Split(',');
            Boolean result = false;

            if (entry[CapitalOne.Column.Credit] != string.Empty && entry[CapitalOne.Column.Description] == "CREDIT-CASH BACK REWARD")
            {
                month.Income += Math.Abs(Decimal.Parse(entry[CapitalOne.Column.Credit]));
                result = true;
            }
            else if(entry[CapitalOne.Column.Credit] == string.Empty)
            {
                result = UpdateCategory(ref month, entry[CapitalOne.Column.Description], Math.Abs(Decimal.Parse(entry[CapitalOne.Column.Debit])));
            }
            return result;
        }

        private static Boolean ParseCSBLine(ref BudgetMonth month, string line)
        {
            string[] entry = line.Split(',');
            Boolean result = false;

            if (entry[CSB.Column.Credit] != string.Empty)
            {
                month.Income += Math.Abs(Decimal.Parse(entry[CSB.Column.Credit]));
                result = true;
            }
            else
            {
                result = UpdateCategory(ref month, entry[CSB.Column.Description], Math.Abs(Decimal.Parse(entry[CSB.Column.Debit])));
            }

            return result;
        }

        private static Boolean UpdateCategory(ref BudgetMonth month, string description, decimal amount)
        {
            Boolean result = false;
            //month.Total += amount;
            if (IsDescriptionInFile(description, "Categories/Bills.txt"))
            {
                month.Bills += amount;
                result = true;
            }
            else if (IsDescriptionInFile(description, "Categories/Car.txt"))
            {
                month.Car += amount;
                result = true;
            }
            else if (IsDescriptionInFile(description, "Categories/Exercise.txt"))
            {
                month.Exercise += amount;
                result = true;
            }
            else if (IsDescriptionInFile(description, "Categories/Food.txt"))
            {
                month.Food += amount;
                result = true;
            }
            else if (IsDescriptionInFile(description, "Categories/Fun.txt"))
            {
                month.Fun += amount;
                result = true;
            }
            else if (IsDescriptionInFile(description, "Categories/Health.txt"))
            {
                month.Health += amount;
                result = true;
            }
            else if (IsDescriptionInFile(description, "Categories/Liquor.txt"))
            {
                month.Liquor += amount;
                result = true;
            }
            else if (IsDescriptionInFile(description, "Categories/Transport.txt"))
            {
                month.Transport += amount;
                result = true;
            }
            else if (IsDescriptionInFile(description, "Categories/Vacations.txt"))
            {
                month.Vacation += amount;
                result = true;
            }
            else if (IsDescriptionInFile(description, "Categories/Work.txt"))
            {
                month.Work += amount;
                result = true;
            }
            else if (IsDescriptionInFile(description, "Categories/Baby.txt"))
            {
                month.Baby += amount;
                result = true;
            }
            else if (IsDescriptionInFile(description, "Categories/Misc.txt"))
            {
                month.Misc += amount;
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        private static bool IsDescriptionInFile(string desc, string fileName)
        {
            desc = desc.Replace("\"", "");
            using (StreamReader descriptions = new StreamReader(fileName))
            {
                while (!descriptions.EndOfStream)
                {
                    string line = descriptions.ReadLine();
                    if (desc == line)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
