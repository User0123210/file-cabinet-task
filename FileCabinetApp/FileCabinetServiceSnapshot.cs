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
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records of the snapshotted IFileCabinetService instance.</param>
        public FileCabinetServiceSnapshot(IReadOnlyCollection<FileCabinetRecord> records)
        {
            this.records = records.ToArray();
        }

        /// <summary>
        /// Gets collection of records.
        /// </summary>
        /// <value>
        /// <Records>Collection of records.</Records>
        /// </value>
        public IReadOnlyCollection<FileCabinetRecord> Records
        {
            get => this.records;
        }

        /// <summary>
        /// Saves records of the snapshot into the specified csv file.
        /// </summary>
        /// <param name="writer">Writer to write the data.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            if (writer is not null)
            {
                writer.BaseStream.Seek(0, SeekOrigin.End);
                FileCabinetRecordCsvWriter recordWriter = new(writer);
                string propertyNames = string.Join(",", typeof(FileCabinetRecord).GetProperties().Select(p => p.Name));
                writer?.WriteLine(propertyNames);

                foreach (var record in this.records)
                {
                    string propertyValues = string.Join(",", typeof(FileCabinetRecord).GetProperties().Select(p => p.GetValue(record) ?? string.Empty));
                    recordWriter.Write(record);
                }

                writer.Close();
            }
        }

        /// <summary>
        /// Saves records of the snapshot into the specified xml file.
        /// </summary>
        /// <param name="writer">Writer to write the data.</param>
        public void SaveToXml(StreamWriter writer)
        {
            if (writer is not null)
            {
                writer.BaseStream.Seek(0, SeekOrigin.End);
                XmlWriter xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true });
                FileCabinetRecordXmlWriter recordWriter = new(xmlWriter);

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("records");

                foreach (var record in this.records)
                {
                    recordWriter.Write(record);
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();

                xmlWriter.Close();
                writer.Close();
            }
        }

        /// <summary>
        /// Replaces records of the snapshot with the records from specified csv file.
        /// </summary>
        /// <param name="reader">Reader of the csv file to read.</param>
        public void LoadFromCsv(StreamReader reader)
        {
            FileCabinetRecordCsvReader csvReader = new (reader);
            this.records = csvReader.ReadAll().ToArray();
        }

        /// <summary>
        /// Replaces records of the snapshot with the records from specified xmlfile.
        /// </summary>
        /// <param name="reader">Reader of the xml file to read.</param>
        public void LoadFromXml(StreamReader reader)
        {
            if (reader is not null)
            {
                reader.BaseStream.Position = 0;
                FileCabinetRecordXmlReader xmlReader = new (XmlReader.Create(reader));
                this.records = xmlReader.ReadAll().ToArray();
            }
        }
    }
}
