#pragma warning disable CA1031, CA1801, IDE0060
namespace FileCabinetGenerator
{
    using Bogus;
    using Bogus.Extensions;
    using FileCabinetApp;
    using System.Globalization;

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
        private static IRecordValidator validator = new DefaultValidator();

        /// <summary>
        /// Runs Console Application.
        /// </summary>
        /// <param name="args">The arguments of the application.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Generator, developed by {Program.DeveloperName}");
            Console.WriteLine();
            FileStream? outputFile = null;

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
                        Generate();
                        Console.WriteLine($"{recordsAmount} records were written to {outputFile}");
                        parseInputs = Array.Empty<string>();
                    }
                }
            }
            while (isRunning);

            outputFile?.Close();
        }

        private static void Generate()
        {
            List<FileCabinetRecord> data = new ();

            for (int i = 0; i < recordsAmount; i++)
            {
                FileCabinetRecord record = new Faker<FileCabinetRecord>().
                    StrictMode(true).
                    RuleFor(r => r.Id, i => startId++).
                    RuleFor(r => r.FirstName, f => f.Name.FirstName().
                    Trim().
                    ClampLength(validator.MinNameLength, validator.MaxNameLength)).
                    RuleFor(r => r.LastName, l => l.Name.LastName().
                    Trim().
                    ClampLength(validator.MinNameLength, validator.MaxNameLength)).
                    RuleFor(r => r.DateOfBirth, d =>
                    {
                        DateTime data = d.Date.
                                        Between(validator.MinDate, DateTime.Now);
                        var result = DateTime.ParseExact(data.ToString(validator.DateFormat, CultureInfo.InvariantCulture), validator.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
                        return result;
                    }).
                    RuleFor(r => r.Status, st => st.Random.Short()).
                    RuleFor(r => r.Salary, s => s.Random.Decimal(0, decimal.MaxValue)).
                    RuleFor(r => r.Permissions, p => (char)p.Random.Int(32, 126));
                data.Add(record);
            }
        }
    }
}
