using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                    List<FileCabinetRecord> recs = new ();

                    if (arguments.Length > 1)
                    {
                        string[] splitOrs = arguments[1].Trim().Split(" or ");
                        string[] ors = splitOrs.Length > 0 ? splitOrs : arguments;

                        for (int i = 0; i < ors.Length; i++)
                        {
                            string[] splitAnds = ors[i].Trim().Split(" and ");
                            string[] ands = splitAnds.Length > 0 ? splitAnds : ors;
                            ImmutableArray<(string, string)> cacheKey = ands.Select(a => (a.Split("=", 2)[0].Trim().ToUpperInvariant(), a.Split("=", 2)[1].Trim().ToUpperInvariant())).ToImmutableArray<(string, string)>();
                            List<FileCabinetRecord> newRecords = new ();

                            if (this.service.SearchCache.ContainsKey(cacheKey))
                            {
                                newRecords.AddRange(this.service.SearchCache[cacheKey]);
                            }
                            else
                            {
                                HashSet<FileCabinetRecord> initialRecords = new ();

                                for (int j = 0; j < ands.Length; j++)
                                {
                                    string property = ands[j].Split("=", 2)[0].Trim();
                                    string value = ands[j].Split("=", 2)[1].Trim();

                                    switch (property.ToUpperInvariant())
                                    {
                                        case "FIRSTNAME":
                                            var firstNameRecords = this.service.FindByFirstName(value.ToUpperInvariant());

                                            foreach (var record in firstNameRecords)
                                            {
                                                if (!initialRecords.Contains(record))
                                                {
                                                    initialRecords.Add(record);
                                                }
                                            }

                                            break;
                                        case "LASTNAME":
                                            var lastNameRecords = this.service.FindByLastName(value.ToUpperInvariant());

                                            foreach (var record in lastNameRecords)
                                            {
                                                if (!initialRecords.Contains(record))
                                                {
                                                    initialRecords.Add(record);
                                                }
                                            }

                                            break;
                                        case "DATEOFBIRTH":
                                            if (DateTime.TryParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate))
                                            {
                                                var dateRecords = this.service.FindByDateOfBirth(newDate);

                                                foreach (var record in dateRecords)
                                                {
                                                    if (!initialRecords.Contains(record))
                                                    {
                                                        initialRecords.Add(record);
                                                    }
                                                }
                                            }

                                            break;
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

                                        switch (property.ToUpperInvariant())
                                        {
                                            case "ID":
                                                if (record.Id != int.Parse(value, CultureInfo.InvariantCulture))
                                                {
                                                    isAndsTrue = false;
                                                }

                                                break;
                                            case "FIRSTNAME":
                                                if (record.FirstName.ToUpperInvariant() != value.ToUpperInvariant())
                                                {
                                                    isAndsTrue = false;
                                                }

                                                break;
                                            case "LASTNAME":
                                                if (record.LastName.ToUpperInvariant() != value.ToUpperInvariant())
                                                {
                                                    isAndsTrue = false;
                                                }

                                                break;
                                            case "DATEOFBIRTH":
                                                if (record.DateOfBirth != DateTime.ParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None))
                                                {
                                                    isAndsTrue = false;
                                                }

                                                break;
                                            case "STATUS":
                                                if (record.Status != short.Parse(value, CultureInfo.InvariantCulture))
                                                {
                                                    isAndsTrue = false;
                                                }

                                                break;
                                            case "SALARY":
                                                if (record.Salary != decimal.Parse(value, CultureInfo.InvariantCulture))
                                                {
                                                    isAndsTrue = false;
                                                }

                                                break;
                                            case "PERMISSIONS":
                                                if (record.Permissions != char.Parse(value))
                                                {
                                                    isAndsTrue = false;
                                                }

                                                break;
                                        }
                                    }

                                    if (isAndsTrue && !recs.Contains(record))
                                    {
                                        newRecords.Add(new FileCabinetRecord() { Id = record.Id, FirstName = record.FirstName, LastName = record.LastName, DateOfBirth = record.DateOfBirth, Status = record.Status, Salary = record.Salary, Permissions = record.Permissions });
                                    }
                                }
                            }

                            IReadOnlyCollection<FileCabinetRecord> cacheValue = newRecords.AsReadOnly();
                            this.service.AddToSearchCache(cacheKey, cacheValue);
                            recs.AddRange(newRecords);
                        }
                    }
                    else
                    {
                        recs = this.service.GetRecords().ToList();
                    }

                    this.print(recs, arguments[0].Split(", "));
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }
    }
}
