﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents csv writer class for the file cabinet records.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;
        private bool isPropertiesWritten;

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
            if (!this.isPropertiesWritten)
            {
                string propertyNames = string.Join(",", typeof(FileCabinetRecord).GetProperties().Select(p => p.Name));
                this.writer.WriteLine(propertyNames);
                this.isPropertiesWritten = true;
            }

            string propertyValues = string.Join(",", typeof(FileCabinetRecord).GetProperties().Select(p => p.GetValue(record) ?? string.Empty));
            this.writer.WriteLine(propertyValues);
        }
    }
}
