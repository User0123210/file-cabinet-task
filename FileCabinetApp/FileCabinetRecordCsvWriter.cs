namespace FileCabinetApp
{
    /// <summary>
    /// Represents csv writer class for the file cabinet records.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">Writer with which data should be written.</param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes record to the specified writer.
        /// </summary>
        /// <param name="record">Record to write.</param>
        public void Write(FileCabinetRecord record)
        {
            string propertyValues = string.Join(",", typeof(FileCabinetRecord).GetProperties().Select(p => p.GetValue(record) ?? string.Empty));
            this.writer.WriteLine(propertyValues);
        }
    }
}
