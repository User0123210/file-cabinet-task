using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents xml reader class for the file cabinet records.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly XmlReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">Reader with which data should be read.</param>
        public FileCabinetRecordXmlReader(XmlReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads all string via the specified reader.
        /// </summary>
        /// <returns>Read strings as FileCabinetRecord list.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            FileCabinetRecord[]? records = Array.Empty<FileCabinetRecord>();

            if (this.reader is not null)
            {
                XmlSerializer serializer = new (typeof(FileCabinetRecord[]), new XmlRootAttribute("Records"));
                records = serializer.Deserialize(this.reader) as FileCabinetRecord[];
            }

            return records is not null ? records.ToList() : new List<FileCabinetRecord>();
        }
    }
}
