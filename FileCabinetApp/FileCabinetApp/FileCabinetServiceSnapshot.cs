using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represent snapshot of the IFileCabinetService.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records of the snapshotted IFileCabinetService instance.</param>
        public FileCabinetServiceSnapshot(IReadOnlyCollection<FileCabinetRecord> records)
        {
            this.records = records.ToArray();
        }

        /// <summary>
        /// Saves records of the snapshot into the specified csv file.
        /// </summary>
        /// <param name="writer">Writer to write the data.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            FileCabinetRecordCsvWriter recordWriter = new (writer);

            foreach (var record in this.records)
            {
                string propertyValues = string.Join(",", typeof(FileCabinetRecord).GetProperties().Select(p => p.GetValue(record) ?? string.Empty));
                recordWriter.Write(record);
            }

            writer?.Close();
        }
    }
}
