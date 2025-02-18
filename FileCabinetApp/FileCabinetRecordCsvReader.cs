namespace FileCabinetApp
{
    /// <summary>
    /// Represents csv reader class for the file cabinet records.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">Reader with which data should be reader.</param>
        public FileCabinetRecordCsvReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads all string via the specified reader.
        /// </summary>
        /// <returns>Read strings as FileCabinetRecord list.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> records = new ();

            if (this.reader is not null)
            {
                this.reader.BaseStream.Position = 0;
                string? firstString = this.reader.ReadLine();
                string[] properties = firstString is not null ? firstString.Split(',') : Array.Empty<string>();

                while (!this.reader.EndOfStream)
                {
                    FileCabinetRecord record = new ();
                    string[] values = this.reader.ReadLine() !.Split(',');

                    for (int i = 0; i < properties.Length; i++)
                    {
                        switch (properties[i])
                        {
                            case "Id":
                                bool isValidInt = int.TryParse(values[i], out int id);
                                record.Id = isValidInt ? id : default;
                                break;
                            case "FirstName":
                                record.FirstName = values[i];
                                break;
                            case "LastName":
                                record.LastName = values[i];
                                break;
                            case "DateOfBirth":
                                bool isValidDate = DateTime.TryParse(values[i], out DateTime date);
                                record.DateOfBirth = isValidDate ? date : DateTime.Now;
                                break;
                            case "Status":
                                bool isValidShort = short.TryParse(values[i], out short shortNumber);
                                record.Status = isValidShort ? shortNumber : default;
                                break;
                            case "Salary":
                                bool isValidDecimal = decimal.TryParse(values[i], out decimal decimalNumber);
                                record.Salary = isValidDecimal ? decimalNumber : default;
                                break;
                            case "Permissions":
                                bool isValidChar = char.TryParse(values[i], out char charNumber);
                                record.Permissions = isValidChar ? charNumber : default;
                                break;
                        }
                    }

                    records.Add(record);
                }
            }

            return records;
        }
    }
}
