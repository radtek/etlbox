using ETLBox.Connection;
using ETLBox.ControlFlow.Tasks;
using ETLBoxTests.Fixtures;
using ETLBoxTests.Helper;
using System.Collections.Generic;
using Xunit;

namespace ETLBoxTests.ControlFlowTests
{
    [Collection("ControlFlow")]
    public class RowCountTaskTests
    {
        public SqlConnectionManager SqlConnection => Config.SqlConnection.ConnectionManager("ControlFlow");
        public static IEnumerable<object[]> Connections => Config.AllSqlConnections("ControlFlow");
        public static IEnumerable<object[]> Access => Config.AccessConnection("ControlFlow");
        public RowCountTaskTests(ControlFlowDatabaseFixture dbFixture)
        { }

        [Theory, MemberData(nameof(Connections))
            , MemberData(nameof(Access))]
        public void NormalCount(IConnectionManager connection)
        {
            //Arrange
            TwoColumnsTableFixture tableDef = new TwoColumnsTableFixture(connection, "RowCountTest");
            tableDef.InsertTestData();
            //Act
            int? actual = RowCountTask.Count(connection, "RowCountTest");
            //Assert
            Assert.Equal(3, actual);
        }

        [Theory, MemberData(nameof(Connections))
            , MemberData(nameof(Access))] //If access fails with "Internal OLE Automation error", download and install: https://www.microsoft.com/en-us/download/confirmation.aspx?id=50040
                                          //see also: https://stackoverflow.com/questions/54632928/internal-ole-automation-error-in-ms-access-using-oledb
        public void CountWithCondition(IConnectionManager connection)
        {
            //Arrange
            TwoColumnsTableFixture tc = new TwoColumnsTableFixture(connection, "RowCountTest");
            tc.InsertTestData();
            //Act
            int? actual = RowCountTask.Count(connection, "RowCountTest", $"{tc.QB}Col1{tc.QE} = 2");
            //Assert
            Assert.Equal(1, actual);
        }

        [Fact]
        public void SqlServerQuickQueryMode()
        {
            //Arrange
            TwoColumnsTableFixture tableDef = new TwoColumnsTableFixture(SqlConnection, "RowCountTest");
            tableDef.InsertTestData();
            //Act
            int? actual = RowCountTask.Count(SqlConnection, "RowCountTest", RowCountOptions.QuickQueryMode);
            //Assert
            Assert.Equal(3, actual);
        }


    }
}
