using ETLBox.Connection;
using ETLBox.DataFlow;
using ETLBox.DataFlow.Connectors;
using ETLBox.DataFlow.Transformations;
using ETLBoxTests.Fixtures;
using ETLBoxTests.Helper;
using System;
using System.Collections.Generic;
using Xunit;

namespace ETLBoxTests.DataFlowTests
{
    [Collection("DataFlow")]
    public class RowMultiplicationErrorLinkingTests
    {
        public SqlConnectionManager SqlConnection => Config.SqlConnection.ConnectionManager("DataFlow");
        public RowMultiplicationErrorLinkingTests(DataFlowDatabaseFixture dbFixture)
        {
        }

        public class MySimpleRow
        {
            public int Col1 { get; set; }
            public string Col2 { get; set; }
        }

        [Fact]
        public void ThrowExceptionInFlow()
        {
            //Arrange
            TwoColumnsTableFixture source2Columns = new TwoColumnsTableFixture("RowMultiplicationSource");
            source2Columns.InsertTestData();

            DbSource<MySimpleRow> source = new DbSource<MySimpleRow>(SqlConnection, "RowMultiplicationSource");
            RowMultiplication<MySimpleRow> multiplication = new RowMultiplication<MySimpleRow>(
                row =>
                {
                    List<MySimpleRow> result = new List<MySimpleRow>();
                    result.Add(row);
                    if (row.Col1 == 2) throw new Exception("Error in Flow!");
                    return result;

                });
            MemoryDestination<MySimpleRow> dest = new MemoryDestination<MySimpleRow>();
            MemoryDestination<ETLBoxError> errorDest = new MemoryDestination<ETLBoxError>();


            //Act
            source.LinkTo(multiplication);
            multiplication.LinkTo(dest);
            multiplication.LinkErrorTo(errorDest);
            source.Execute();
            dest.Wait();
            errorDest.Wait();

            //Assert
            Assert.Collection<ETLBoxError>(errorDest.Data,
                d => Assert.True(!string.IsNullOrEmpty(d.RecordAsJson) && !string.IsNullOrEmpty(d.ErrorText))
            );
        }



        [Fact]
        public void ThrowExceptionWithoutHandling()
        {
            //Arrange
            TwoColumnsTableFixture source2Columns = new TwoColumnsTableFixture("RowMultiplicationSource");
            source2Columns.InsertTestData();

            DbSource<MySimpleRow> source = new DbSource<MySimpleRow>(SqlConnection, "RowMultiplicationSource");
            RowMultiplication<MySimpleRow> multiplication = new RowMultiplication<MySimpleRow>(
                row =>
                {
                    List<MySimpleRow> result = new List<MySimpleRow>();
                    result.Add(row);
                    if (row.Col1 == 2) throw new Exception("Error in Flow!");
                    return result;

                });
            MemoryDestination<MySimpleRow> dest = new MemoryDestination<MySimpleRow>();

            //Act & Assert
            source.LinkTo(multiplication);
            multiplication.LinkTo(dest);

            Assert.Throws<AggregateException>(() =>
            {
                source.Execute();
                dest.Wait();
            });
        }
    }
}
