using System.Collections.Immutable;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class to handle select command in IFileCabinetService.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<IEnumerable<FileCabinetRecord>, string[]> print;

        /// <summary>
        /// Initializes new instance of SelectCommandHandler via the IFileCabinetService to handle commands in.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="print">Delegate to print results.</param>
        public SelectCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>, string[]> print)
            : base(service)
        {
            this.print = print;
        }

        /// <summary>
        /// Handles commands from the service.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "select")
                {
                    string[] arguments = commandRequest.Parameters.Split("where", 2);
                    HashSet<FileCabinetRecord> records = new (new FileCabinetRecordComparer());
                    records = this.ParseArgumentsAndReturnRecords(arguments);
                    this.print(records, arguments[0].Split(", "));
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }

        private HashSet<FileCabinetRecord> ParseArgumentsAndReturnRecords(string[] arguments)
        {
            HashSet<FileCabinetRecord> records = new (new FileCabinetRecordComparer());

            if (arguments.Length > 1)
            {
                string[] splitOrs = arguments[1].Trim().Split(" or ");
                string[] ors = splitOrs.Length > 0 ? splitOrs : arguments;

                for (int i = 0; i < ors.Length; i++)
                {
                    string[] splitAnds = ors[i].Trim().Split(" and ");
                    string[] ands = splitAnds.Length > 0 ? splitAnds : ors;
                    List<FileCabinetRecord> newRecords = this.ParseAndsAndReturnRecords(ands, records);

                    foreach (var record in newRecords)
                    {
                        if (!records.Contains(record))
                        {
                            records.Add(record);
                        }
                    }
                }

                return records;
            }

            return this.service.GetRecords().ToHashSet();
        }

        private List<FileCabinetRecord> ParseAndsAndReturnRecords(string[] ands, HashSet<FileCabinetRecord> records)
        {
            ImmutableArray<(string, string)> cacheKey = ands.Select(a => (a.Split("=", 2)[0].Trim().ToUpperInvariant(), a.Split("=", 2)[1].Trim().ToUpperInvariant())).ToImmutableArray<(string, string)>();
            List<FileCabinetRecord> newRecords = new ();

            if (this.service.SearchCache.ContainsKey(cacheKey))
            {
                newRecords.AddRange(this.service.SearchCache[cacheKey]);
                return newRecords;
            }

            HashSet<FileCabinetRecord> initialRecords = new (new FileCabinetRecordComparer());

            for (int j = 0; j < ands.Length; j++)
            {
                string property = ands[j].Split("=", 2)[0].Trim();
                string value = ands[j].Split("=", 2)[1].Trim();
                IEnumerable<FileCabinetRecord>? recordsToAdd = null;

                switch (property.ToUpperInvariant())
                {
                    case "FIRSTNAME":
                        recordsToAdd = this.service.FindByFirstName(value.ToUpperInvariant());
                        break;
                    case "LASTNAME":
                        recordsToAdd = this.service.FindByLastName(value.ToUpperInvariant());
                        break;
                    case "DATEOFBIRTH":
                        if (DateTime.TryParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate))
                        {
                            recordsToAdd = this.service.FindByDateOfBirth(newDate);
                        }

                        break;
                }

                if (recordsToAdd is null)
                {
                    continue;
                }

                foreach (var record in recordsToAdd)
                {
                    if (!initialRecords.Contains(record))
                    {
                        initialRecords.Add(record);
                    }
                }
            }

            if (initialRecords.Count == 0)
            {
                initialRecords = this.service.GetRecords().ToHashSet();
            }

            foreach (var record in initialRecords)
            {
                bool isAndsTrue = true;

                for (int j = 0; j < ands.Length; j++)
                {
                    string property = ands[j].Split("=", 2)[0].Trim();
                    string value = ands[j].Split("=", 2)[1].Trim();

                    isAndsTrue = this.ValidateRecordProperty(property, value, record);
                }

                if (isAndsTrue && !records.Contains(record))
                {
                    newRecords.Add(new FileCabinetRecord() { Id = record.Id, FirstName = record.FirstName, LastName = record.LastName, DateOfBirth = record.DateOfBirth, Status = record.Status, Salary = record.Salary, Permissions = record.Permissions });
                }
            }

            IReadOnlyCollection<FileCabinetRecord> cacheValue = newRecords.AsReadOnly();
            this.service.AddToSearchCache(cacheKey, cacheValue);
            return newRecords;
        }

        private bool ValidateRecordProperty(string property, string value, FileCabinetRecord record)
        {
            bool isAndsTrue = true;

            switch (property.ToUpperInvariant())
            {
                case "ID":
                    bool isInt = int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out int id);
                    isAndsTrue = isInt && record.Id == id && isAndsTrue;
                    break;
                case "FIRSTNAME":
                    isAndsTrue = record.FirstName.ToUpperInvariant() == value.ToUpperInvariant() && isAndsTrue;
                    break;
                case "LASTNAME":
                    isAndsTrue = record.LastName.ToUpperInvariant() == value.ToUpperInvariant() && isAndsTrue;
                    break;
                case "DATEOFBIRTH":
                    bool isDate = DateTime.TryParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate);
                    isAndsTrue = isDate && record.DateOfBirth == newDate && isAndsTrue;
                    break;
                case "STATUS":
                    bool isShort = short.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out short status);
                    isAndsTrue = isShort && record.Status == status && isAndsTrue;
                    break;
                case "SALARY":
                    bool isDecimal = decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal salary);
                    isAndsTrue = isDecimal && record.Salary == salary && isAndsTrue;
                    break;
                case "PERMISSIONS":
                    bool isChar = char.TryParse(value, out char permissions);
                    isAndsTrue = isChar && record.Permissions == permissions && isAndsTrue;
                    break;
            }

            return isAndsTrue;
        }
    }
}
