using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Bogus;
using Bogus.Extensions;
using FileCabinetApp;
using FileCabinetApp.Validators;

#pragma warning disable CA1303, CA1031, IDE0060

namespace FileCabinetGenerator
{
    /// <summary>
    /// Represents FileCabinetGenerator Console App.
    /// </summary>
    internal class Program
    {
        private const string DeveloperName = "Serafima Mochalova";
        private static string outputType = "csv";
        private static int recordsAmount;
        private static int startId = 1;
        private static bool isRunning = true;
        private static FileStream? outputFile;

        /// <summary>
        /// Runs Console Application.
        /// </summary>
        /// <param name="args">The arguments of the application.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Generator, developed by {Program.DeveloperName}");
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                line = line?.Trim();
                var inputs = line != null ? line.Split(new char[] { '=', ' ' }, 3) : new string[] { string.Empty, string.Empty };
                const int commandIndex = 0;
                string command = inputs[commandIndex];
                var parseInputs = inputs;

                if (command == "exit")
                {
                    isRunning = false;
                }

                while (parseInputs.Length > 1)
                {
                    command = parseInputs[commandIndex];
                    var paramValue = parseInputs[commandIndex + 1];

                    if (command == "--output-type" || command == "-t")
                    {
                        switch (paramValue.ToUpperInvariant())
                        {
                            case "CSV":
                                outputType = "csv";
                                break;
                            case "XML":
                                outputType = "xml";
                                break;
                        }

                        Console.WriteLine($"Using {outputType} output type.");
                    }
                    else if (command == "--output" || command == "-o")
                    {
                        try
                        {
                            outputFile?.Close();
                            outputFile = new (paramValue, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                            Console.WriteLine($"Using {paramValue} outputFile.");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("An error ocured during the specifying of output file.");
                        }
                    }
                    else if (command == "--records-amount" || command == "-a")
                    {
                        if (int.TryParse(paramValue, out int amount))
                        {
                            if (amount >= 0)
                            {
                                recordsAmount = amount;
                                Console.WriteLine($"Using {recordsAmount} records amount.");
                            }
                            else
                            {
                                Console.WriteLine("Records amount should be more than 0.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Records amount should be an integer.");
                        }
                    }
                    else if (command == "--start-id" || command == "-i")
                    {
                        if (int.TryParse(paramValue, out int id))
                        {
                            startId = id;
                            Console.WriteLine($"Start id is {startId}.");
                        }
                        else
                        {
                            Console.WriteLine("Start id should be an integer.");
                        }
                    }

                    if (parseInputs.Length > 2)
                    {
                        parseInputs = parseInputs[commandIndex + 2].Split(new char[] { ' ', '=' }, 3);
                    }
                    else if (parseInputs.Length <= 2)
                    {
                        switch (outputType)
                        {
                            case "csv":
                                ExportToCsv(Generate());
                                break;
                            case "xml":
                                ExportToXml(Generate());
                                break;
                        }

                        Console.WriteLine($"{recordsAmount} records were written to {outputFile?.Name}");
                        parseInputs = Array.Empty<string>();
                    }
                }
            }
            while (isRunning);

            outputFile?.Close();
        }

        private static ReadOnlyCollection<FileCabinetRecord> Generate()
        {
            List<FileCabinetRecord> data = new ();

            for (int i = 0; i < recordsAmount; i++)
            {
                FileCabinetRecord record = new Faker<FileCabinetRecord>().
                    StrictMode(true).
                    RuleFor(r => r.Id, i => startId++).
                    RuleFor(r => r.FirstName, f => f.Name.FirstName().
                    Trim().
                    ClampLength(2, 60)).
                    RuleFor(r => r.LastName, l => l.Name.LastName().
                    Trim().
                    ClampLength(2, 60)).
                    RuleFor(r => r.DateOfBirth, d =>
                    {
                        DateTime data = d.Date.
                                        Between(new DateTime(1950, 1, 1), DateTime.Now);
                        var result = DateTime.ParseExact(data.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
                        return result;
                    }).
                    RuleFor(r => r.Status, st => st.Random.Short()).
                    RuleFor(r => r.Salary, s => s.Random.Decimal(0, decimal.MaxValue)).
                    RuleFor(r => r.Permissions, p => (char)p.Random.Int(32, 126));
                data.Add(record);
            }

            return data.AsReadOnly();
        }

        private static void ExportToCsv(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (outputFile is not null)
            {
                StreamWriter writer = new (outputFile);
                string propertyNames = string.Join(",", typeof(FileCabinetRecord).GetProperties().Select(p => p.Name));
                writer?.WriteLine(propertyNames);

                foreach (var record in records)
                {
                    string propertyValues = string.Join(",", typeof(FileCabinetRecord).GetProperties().Select(p => p.GetValue(record) ?? string.Empty));
                    writer?.WriteLine(propertyValues);
                }

                writer?.Close();
            }
        }

        private static void ExportToXml(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (outputFile is not null)
            {
                XmlSerializer serializer = new (typeof(FileCabinetRecord[]), new XmlRootAttribute("records"));
                serializer.Serialize(outputFile, records.ToArray());
            }
        }
    }
}
