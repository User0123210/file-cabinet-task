using System.Globalization;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents xml writer class for the file cabinet records.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">Writer with which data should be written.</param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes record to the specified writer.
        /// </summary>
        /// <param name="record">Record to write.</param>
        public void Write(FileCabinetRecord record)
        {
            if (record is not null)
            {
                this.writer.WriteStartElement("record");
                this.writer.WriteAttributeString("id", Convert.ToString(record.Id, CultureInfo.InvariantCulture));

                this.writer.WriteElementString("firstName", record.FirstName);
                this.writer.WriteElementString("lastName", record.LastName);
                this.writer.WriteElementString("dateOfBirth", string.Format(CultureInfo.InvariantCulture, $"{record.DateOfBirth.Date:yyyy-MM-dd}"));
                this.writer.WriteElementString("status", Convert.ToString(record.Status, CultureInfo.InvariantCulture));
                this.writer.WriteElementString("salary", Convert.ToString(record.Salary, CultureInfo.InvariantCulture));
                this.writer.WriteElementString("permissions", Convert.ToString((int)record.Permissions, CultureInfo.InvariantCulture));
                this.writer.WriteEndElement();
            }
        }
    }
}
