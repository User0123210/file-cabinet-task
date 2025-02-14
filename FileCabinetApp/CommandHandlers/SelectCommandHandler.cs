using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Initializes new instance of SelectCommandHandler via the IFileCabinetService to handle commands in.
        /// </summary>
        /// <param name="service"></param>
        public SelectCommandHandler(IFileCabinetService service)
            : base(service)
        {
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
                        var initialRecords = this.service.GetRecords();

                        for (int i = 0; i < ors.Length; i++)
                        {
                            string[] splitAnds = ors[i].Trim().Split(" and ");
                            string[] ands = splitAnds.Length > 0 ? splitAnds : ors;
                            var records = initialRecords;

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
                                    recs.Add(new FileCabinetRecord() { Id = record.Id, FirstName = record.FirstName, LastName = record.LastName, DateOfBirth = record.DateOfBirth, Status = record.Status, Salary = record.Salary, Permissions = record.Permissions });
                                }
                            }
                        }
                    }
                    else
                    {
                        recs = this.service.GetRecords().ToList();
                    }

                    StringBuilder row = new ();
                    PropertyInfo[] initialProperties = typeof(FileCabinetRecord).GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo[] properties = initialProperties.Where(p => arguments[0].Split(", ").Any(a => a.Trim().ToUpperInvariant() == p.Name.ToUpperInvariant())).ToArray();
                    properties = properties.Length > 0 ? properties : initialProperties;
                    int cols = properties.Length;
                    int[] widths = new int[cols];
                    StringBuilder edge = new ();

                    for (int c = 0; c < cols; c++)
                    {
                        var pr = properties[c];

                        if (pr is not null)
                        {
                            var lengths = recs.Select(e => e is not null && pr.GetValue(e) is not null ? pr.GetValue(e) !.ToString() !.Length : 0);
                            int max = 0;

                            if (lengths is not null && lengths.Any())
                            {
                                max = lengths.Max(e => e);
                            }

                            widths[c] = pr.Name.Length > max ? pr.Name.Length : max;
                        }

                        edge = edge.Append("+" + new string('-', widths[c] + 2));
                    }

                    edge = edge.Append(CultureInfo.InvariantCulture, $"+{Environment.NewLine}");
                    int colNum = 0;

                    Console.WriteLine(edge);

                    foreach (var p in properties)
                    {
                        row = row.Append(string.Format(CultureInfo.InvariantCulture, "| {0} ", p.Name + new string(' ', widths[colNum] - p!.Name.ToString() !.Length)));
                        colNum++;
                    }

                    row = row.Append(CultureInfo.InvariantCulture, $"|{Environment.NewLine}");

                    Console.WriteLine(row);
                    Console.WriteLine(edge);

                    foreach (var rec in recs)
                    {
                        row = new StringBuilder();

                        for (int j = 0; j < cols; j++)
                        {
                            var p = properties[j].GetValue(rec);
                            row = row.Append(string.Format(CultureInfo.InvariantCulture, "| {0} ", p is string || p is char ? p + new string(' ', widths[j] - p!.ToString() !.Length) : new string(' ', widths[j] - p!.ToString() !.Length) + p!.ToString()));
                        }

                        row = row.Append(CultureInfo.InvariantCulture, $"|{Environment.NewLine}");
                        Console.WriteLine(row);
                        Console.WriteLine(edge);
                    }
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }
    }
}
