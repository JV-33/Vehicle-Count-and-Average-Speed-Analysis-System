﻿using Microsoft.EntityFrameworkCore;
using Nexall.Data.DataContext;
using Nexall.Services;

namespace NEXALLTest
{
    [TestClass]
    public class DataImportServiceTests
    {
        private NexallContext _context;
        private DataImportService _service;

        [TestInitialize]
        public void Initialize()
        {
            var databaseName = Guid.NewGuid().ToString();

            var optionsBuilder = new DbContextOptionsBuilder<NexallContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName);

            _context = new NexallContext(optionsBuilder.Options);
            _service = new DataImportService(_context);
        }

        [TestCleanup]
        public void Cleanup_ImportedStatistics_FromContext()
        {
            _context.Statistics.RemoveRange(_context.Statistics);
            _context.SaveChanges();
        }

        [TestMethod]
        public void ImportData_ValidDataProvided_TwoRecordsImported()
        {
            var filePath = Path.GetTempFileName();
            File.WriteAllLines(filePath, new[] { "2023-10-29\t100\tAB1234", "2023-10-28\t90\tCD5678" });

            _service.ImportData(filePath);
            var statistics = _context.Statistics.ToList();

            Assert.AreEqual(2, statistics.Count);
        }

        [TestMethod]
        public void ImportData_EmptyFileProvided_NoDataImported()
        {
            var filePath = Path.GetTempFileName();
            File.WriteAllLines(filePath, new string[] { });

            _service.ImportData(filePath);
            var statistics = _context.Statistics.ToList();

            Assert.AreEqual(0, statistics.Count);
        }

        [TestMethod]
        public void ImportData_InvalidFilePathProvided_NothingImported()
        {
            var filePath = "invalidPath.txt";

            _service.ImportData(filePath);
            var statistics = _context.Statistics.ToList();

            Assert.AreEqual(0, statistics.Count);
        }

        [TestMethod]
        public void ImportData_InvalidDataFormatProvided_NoDataImported()
        {
            var filePath = Path.GetTempFileName();
            File.WriteAllLines(filePath, new[] { "invalidData" });

            _service.ImportData(filePath);
            var statistics = _context.Statistics.ToList();

            Assert.AreEqual(0, statistics.Count);
        }

        [TestMethod]
        public void ImportData_IncorrectDataStructureProvided_NoDataImported()
        {
            var filePath = Path.GetTempFileName();
            File.WriteAllLines(filePath, new[] { "2023-10-29\t100\tAB1234\tExtraColumn", "2023-10-28\t90" });

            _service.ImportData(filePath);
            var statistics = _context.Statistics.ToList();

            Assert.AreEqual(0, statistics.Count);
        }
    }
}