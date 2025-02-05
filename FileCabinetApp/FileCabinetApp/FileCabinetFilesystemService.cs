using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represent file cabinet service for work with filesytem.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 292;
        private readonly FileStream stream;
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="stream">Stream to write.</param>
        /// <param name="validator">Validator to check record parameters.</param>
        public FileCabinetFilesystemService(FileStream stream, IRecordValidator validator)
        {
            this.stream = stream;
            this.validator = validator;
        }

        /// <summary>
        /// Gets minimal possible length of the name.
        /// </summary>
        /// <value>this.minNameLength.</value>
        public int MinNameLength { get => this.validator.MinNameLength; }

        /// <summary>
        /// Gets maximum possible length of the name.
        /// </summary>
        /// <value>this.maxNameLength.</value>
        public int MaxNameLength { get => this.validator.MaxNameLength; }

        /// <summary>
        /// Gets a value indicating whether the name should contain only letter characters or not.
        /// </summary>
        /// <value>isOnlyLetterName.</value>
        public bool IsOnlyLetterName { get => this.validator.IsOnlyLetterName; }

        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        public string DateFormat { get => this.validator.DateFormat; }

        /// <summary>
        /// Gets minimum possible date.
        /// </summary>
        /// <value>this.minDate.</value>
        public DateTime MinDate { get => this.validator.MinDate; }

        /// <summary>
        /// Gets information about the number of records in the service.
        /// </summary>
        /// <value>
        /// <records.Count>Information about the number of records in the service.</records.Count>
        /// </value>
        public int GetStat
        {
            get
            {
                int numberOfRecords = 0;
                this.stream.Position = 0;
                byte[] buffer = new byte[RecordSize];

                while (this.stream.Read(buffer, 0, RecordSize) != 0)
                {
                    numberOfRecords++;
                    this.stream.Position += RecordSize;
                }

                return numberOfRecords;
            }
        }

        /// <summary>
        /// Gets array of valid permissions.
        /// </summary>
        /// <returns>An array of valid permissions.</returns>
        public ReadOnlyCollection<char> GetValidPermissions() => this.validator.GetValidPermissions();

        /// <summary>
        /// Creates a new value and adds it into the records list.
        /// </summary>
        /// <param name="recordParameters">Parameters of the value to change.</param>
        /// <returns>Id of the created value.</returns>
        public int CreateRecord(FileCabinetRecordParameterObject recordParameters)
        {
            if (recordParameters is not null)
            {
                int numberOfRecords = this.GetStat;
                Span<int> copyDecimal = new (new int[4]);
                decimal.GetBits(recordParameters.Salary, copyDecimal);
                byte[] value = new byte[RecordSize];

                byte[] buffer = new byte[RecordSize];

                while (this.stream.Read(buffer, 0, RecordSize) != 0)
                {
                }

                BitConverter.GetBytes(1).CopyTo(value, 0);
                BitConverter.GetBytes(numberOfRecords + 1).CopyTo(value, 2);
                Encoding.UTF8.GetBytes(recordParameters.FirstName.PadRight(120, '\0')).CopyTo(value, 6);
                Encoding.UTF8.GetBytes(recordParameters.LastName.PadRight(120, '\0')).CopyTo(value, 126);
                BitConverter.GetBytes(recordParameters.DateOfBirth.Year).CopyTo(value, 246);
                BitConverter.GetBytes(recordParameters.DateOfBirth.Month).CopyTo(value, 250);
                BitConverter.GetBytes(recordParameters.DateOfBirth.Day).CopyTo(value, 254);
                BitConverter.GetBytes(recordParameters.Status).CopyTo(value, 258);
                BitConverter.GetBytes(copyDecimal[0]).CopyTo(value, 274);
                BitConverter.GetBytes(copyDecimal[1]).CopyTo(value, 278);
                BitConverter.GetBytes(copyDecimal[2]).CopyTo(value, 282);
                BitConverter.GetBytes(copyDecimal[3]).CopyTo(value, 286);
                BitConverter.GetBytes(recordParameters.Permissions).CopyTo(value, 290);
                this.stream.Write(value, 0, RecordSize);
                this.stream.Flush();
                this.stream.Position = 0;
                return numberOfRecords + 1;
            }

            return 0;
        }

        /// <summary>
        /// Gets copy of the records as value array.
        /// </summary>
        /// <returns>Array of the records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new ();
            byte[] buffer = new byte[RecordSize];
            this.stream.Position = 0;
            int id;
            string firstName;
            string lastName;
            int year;
            int month;
            int day;
            short status;
            decimal salary;
            char permissions;
            int[] copyDecimal = new int[4];

            while (this.stream.Read(buffer, 0, RecordSize) != 0)
            {
                this.stream.Position += RecordSize;
                id = BitConverter.ToInt32(buffer, 2);
                firstName = Encoding.UTF8.GetString(buffer[6..126]).TrimEnd('\0');
                lastName = Encoding.UTF8.GetString(buffer[126..246]).TrimEnd('\0');
                year = BitConverter.ToInt32(buffer, 246);
                month = BitConverter.ToInt32(buffer, 250);
                day = BitConverter.ToInt32(buffer, 254);
                DateTime dateOfBirth = new (year, month, day);
                status = BitConverter.ToInt16(buffer, 258);
                copyDecimal[0] = BitConverter.ToInt32(buffer, 274);
                copyDecimal[1] = BitConverter.ToInt32(buffer, 278);
                copyDecimal[2] = BitConverter.ToInt32(buffer, 282);
                copyDecimal[3] = BitConverter.ToInt32(buffer, 286);
                salary = new decimal(copyDecimal);
                permissions = BitConverter.ToChar(buffer, 290);

                records.Add(new FileCabinetRecord() { Id = id, FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status,  Salary = salary, Permissions = permissions });
            }

            return records.AsReadOnly();
        }

        /// <summary>
        /// Edits the existing value with the specified id.
        /// </summary>
        /// <param name="id">Id of the value to edit.</param>
        /// <param name="recordParameters">Parameters of the value to change.</param>
        public void EditRecord(int id, FileCabinetRecordParameterObject recordParameters)
        {
            if (recordParameters is not null)
            {
                byte[] buffer = new byte[RecordSize];
                this.stream.Position = 0;
                int recordId;

                Span<int> copyDecimal = new (new int[4]);
                decimal.GetBits(recordParameters.Salary, copyDecimal);
                byte[] value = new byte[RecordSize - 6];

                while (this.stream.Read(buffer, 0, RecordSize) != 0)
                {
                    recordId = BitConverter.ToInt32(buffer, 2);

                    if (recordId == id)
                    {
                        Encoding.UTF8.GetBytes(recordParameters.FirstName.PadRight(120, '\0')).CopyTo(value, 0);
                        Encoding.UTF8.GetBytes(recordParameters.LastName.PadRight(120, '\0')).CopyTo(value, 120);
                        BitConverter.GetBytes(recordParameters.DateOfBirth.Year).CopyTo(value, 240);
                        BitConverter.GetBytes(recordParameters.DateOfBirth.Month).CopyTo(value, 244);
                        BitConverter.GetBytes(recordParameters.DateOfBirth.Day).CopyTo(value, 248);
                        BitConverter.GetBytes(recordParameters.Status).CopyTo(value, 252);
                        BitConverter.GetBytes(copyDecimal[0]).CopyTo(value, 268);
                        BitConverter.GetBytes(copyDecimal[1]).CopyTo(value, 272);
                        BitConverter.GetBytes(copyDecimal[2]).CopyTo(value, 276);
                        BitConverter.GetBytes(copyDecimal[3]).CopyTo(value, 280);
                        BitConverter.GetBytes(recordParameters.Permissions).CopyTo(value, 284);
                        this.stream.Position -= RecordSize;
                        this.stream.Position += 6;
                        this.stream.Write(value, 0, RecordSize - 6);
                        this.stream.Flush();
                        break;
                    }
                }

                this.stream.Position = 0;
            }
        }

        /// <summary>
        /// Looks for records with firstName property equal to the specified firstName parameter.
        /// </summary>
        /// <param name="firstName">First name of the records to seek.</param>
        /// <returns>Array of the found records with the specified firstName.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName) => throw new NotImplementedException();

        /// <summary>
        /// Looks for records with lastName property equal to the specified lastName parameter.
        /// </summary>
        /// <param name="lastName">Last name of the records to seek.</param>
        /// <returns>Array of the found records with the specified lastName.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName) => throw new NotImplementedException();

        /// <summary>
        /// Looks for records with dateOfBirth property equal to the specified date parameter.
        /// </summary>
        /// <param name="date">Date of birth of the records to seek.</param>
        /// <returns>Array of the found records with the specified dateOfBirth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime date) => throw new NotImplementedException();

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetDefaultService.
        /// </summary>
        public void ChangeValidatorToCustom() => throw new NotImplementedException();

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetCustomService.
        /// </summary>
        public void ChangeValidatorToDefault() => throw new NotImplementedException();

        /// <summary>
        /// Makes snapshot of the IFileCabinetService with the copy of the records.
        /// </summary>
        /// <returns>FileCabinetServiceSnapshot of the current IFileCabinetService instance.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot() => throw new NotImplementedException();
    }
}
