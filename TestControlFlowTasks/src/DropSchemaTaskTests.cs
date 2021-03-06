using ETLBox.Connection;
using ETLBox.ControlFlow.Tasks;
using ETLBox.Exceptions;
using ETLBoxTests.Fixtures;
using ETLBoxTests.Helper;
using System.Collections.Generic;
using Xunit;

namespace ETLBoxTests.ControlFlowTests
{
    [Collection("ControlFlow")]
    public class DropSchemaTaskTests
    {
        public static IEnumerable<object[]> Connections => Config.AllConnectionsWithoutSQLite("ControlFlow");

        public DropSchemaTaskTests(ControlFlowDatabaseFixture dbFixture)
        { }

        [Theory, MemberData(nameof(Connections))]
        public void Drop(IConnectionManager connection)
        {
            if (connection.GetType() == typeof(MySqlConnectionManager)
                || connection.GetType() == typeof(MariaDbConnectionManager)
                 || connection.GetType() == typeof(OracleConnectionManager)
                )
                return;
            //Arrange
            CreateSchemaTask.Create(connection, "testcreateschema");
            Assert.True(IfSchemaExistsTask.IsExisting(connection, "testcreateschema"));

            //Act
            DropSchemaTask.Drop(connection, "testcreateschema");

            //Assert
            Assert.False(IfSchemaExistsTask.IsExisting(connection, "testcreateschema"));
        }

        [Theory, MemberData(nameof(Connections))]
        public void DropIfExists(IConnectionManager connection)
        {
            if (connection.GetType() == typeof(MySqlConnectionManager)
                 || connection.GetType() == typeof(MariaDbConnectionManager)
                 || connection.GetType() == typeof(OracleConnectionManager)
                )
                return;

            //Arrange
            DropSchemaTask.DropIfExists(connection, "testcreateschema2");
            CreateSchemaTask.Create(connection, "testcreateschema2");
            Assert.True(IfSchemaExistsTask.IsExisting(connection, "testcreateschema2"));

            //Act
            DropSchemaTask.DropIfExists(connection, "testcreateschema2");

            //Assert
            Assert.False(IfSchemaExistsTask.IsExisting(connection, "testcreateschema2"));
        }


        [Fact]
        public void NotSupportedWithSQLite()
        {
            Assert.Throws<ETLBoxNotSupportedException>(
                () => DropSchemaTask.Drop(Config.SQLiteConnection.ConnectionManager("ControlFlow"), "Test")
                );
        }

        [Fact]
        public void NotSupportedWithMySql()
        {
            Assert.Throws<ETLBoxNotSupportedException>(
                () => DropSchemaTask.Drop(Config.MySqlConnection.ConnectionManager("ControlFlow"), "Test")
                );
        }

        [Fact]
        public void NotSupportedWithOracle()
        {
            Assert.Throws<ETLBoxNotSupportedException>(
                () => DropSchemaTask.Drop(Config.OracleConnection.ConnectionManager("ControlFlow"), "Test")
                );
        }
    }
}
