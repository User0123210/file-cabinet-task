using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

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
            string propertyNames = string.Join(",", typeof(FileCabinetRecord).GetProperties().Select(p => p.Name));
            writer?.WriteLine(propertyNames);

            foreach (var record in this.records)
            {
                string propertyValues = string.Join(",", typeof(FileCabinetRecord).GetProperties().Select(p => p.GetValue(record) ?? string.Empty));
                recordWriter.Write(record);
            }

            writer?.Close();
        }

        /// <summary>
        /// Saves records of the snapshot into the specified xml file.
        /// </summary>
        /// <param name="writer">Writer to write the data.</param>
        public void SaveToXml(StreamWriter writer)
        {
            XmlWriter xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true });
            FileCabinetRecordXmlWriter recordWriter = new (xmlWriter);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("records");

            foreach (var record in this.records)
            {
                recordWriter.Write(record);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();

            xmlWriter.Close();
            writer?.Close();
        }
    }
}
