using ETLBox.Connection;
using ETLBox.DataFlow;
using ETLBox.DataFlow.Connectors;
using ETLBoxTests.Fixtures;
using ETLBoxTests.Helper;
using System.IO;
using System.Xml.Serialization;
using Xunit;

namespace ETLBoxTests.DataFlowTests
{
    [Collection("DataFlow")]
    public class XmlDestinationTests
    {
        public SqlConnectionManager SqlConnection => Config.SqlConnection.ConnectionManager("DataFlow");
        public XmlDestinationTests(DataFlowDatabaseFixture dbFixture)
        {
        }

        public class MySimpleRow
        {
            public int Col1 { get; set; }
            public string Col2 { get; set; }
        }

        [Fact]
        public void SimpleFlowWithObject()
        {
            //Arrange
            TwoColumnsTableFixture s2C = new TwoColumnsTableFixture("XmlDestSimple");
            s2C.InsertTestDataSet3();
            DbSource<MySimpleRow> source = new DbSource<MySimpleRow>(SqlConnection, "XmlDestSimple");

            //Act
            XmlDestination<MySimpleRow> dest = new XmlDestination<MySimpleRow>("./SimpleWithObject.xml", ResourceType.File);
            source.LinkTo(dest);
            source.Execute();
            dest.Wait();

            //Assert
            Assert.Equal(File.ReadAllText("res/XmlDestination/TwoColumnsSet3.xml")
                , File.ReadAllText("./SimpleWithObject.xml"));
        }

        [XmlRoot("MySimpleRow")]
        public class MyAttributeRow
        {
            [XmlAttribute]
            public int Col1 { get; set; }
            [XmlAttribute]
            public string Col2 { get; set; }
        }

        [Fact]
        public void SimpleOnlyAttributes()
        {
            //Arrange
            TwoColumnsTableFixture s2C = new TwoColumnsTableFixture("XmlDestOnlyAttributes");
            s2C.InsertTestDataSet3();
            DbSource<MyAttributeRow> source = new DbSource<MyAttributeRow>(SqlConnection, "XmlDestOnlyAttributes");

            //Act
            XmlDestination<MyAttributeRow> dest = new XmlDestination<MyAttributeRow>("./SimpleOnlyAttributes.xml", ResourceType.File);
            source.LinkTo(dest);
            source.Execute();
            dest.Wait();

            //Assert
            Assert.Equal(File.ReadAllText("res/XmlDestination/TwoColumnsAttributesSet3.xml")
                , File.ReadAllText("./SimpleOnlyAttributes.xml"));
        }
    }
}
