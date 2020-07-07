# TODO

## Enhancements
- If not everything is connected to an destination when using predicates, it can be that the dataflow never finishes. Write some tests. See Github project DataflowEx for implementation how to create a predicate that always discards records not transferred.
- Now the Connection Manager have a PrepareBulkInsert/CleanUpBulkInsert method. There are missing tests that check that make use of the Modify-Db settings and verify improved performance. DbDestination modifies these server side settings only once and at then end end of all batches.
- VoidDestination: [Use a NullBlock as Target](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.dataflow.dataflowblock.nulltarget?view=netcore-3.1)
- Check if SMOConnectionManager can be reinstalled again
- All sources (DbSource, CsvSource, etc. )  always read all the data from the source. For development purposes it would be benefical if only the first X rows are read from the source. A property `public int Limit` could be introduced, so that only the first X rows are read for a DBSource/CSVSource/JsonSource/. This is quite easy to implement as SqlTask already has the Limit property. For Csv/Json, there should be a counter on the lines within the stream reader...
- CreateTableTask.CreateOrAlter() and Migrate(): add functionality to alter a table if empty, or Migrate a table if not empty

## Refactoring

- Remove SqlTask: Add task name & Comments before sql code Make sql task name optional
- The MaxBufferSize parameter does call every time InitBufferObjects to reinitialize the TPL dataflow object with the new bounded capacity. This is due to the restrictions
that the bounded capacity can only be set when creating the TPL objects. It would be good to have everything more abstract and cleaner: The constructor should not initialize any objects whatsoever. The whole linking of objects would only between the abstract ETLBox objects. Then, when starting the data flow, the initialization would take place. This means not only the creation of the TPL objects, but also the linking between them as well as the other thing like waiting for completion (AddPredecessorCompletion) etc. This would mean that the whole DataFlowLinker as well as the current interfaces are refactored. 

## Bugs

- PrimaryKeyConstrainName now is part of TableDefinition, but not read from "GetTableDefinitionFrom"
- GCPressure was detected on CSVSource - verify if CSVSource really is the root cause. (See performance tests, improve tests that uses memory as source) 
- Multicast: When DataFlow has set a MaxBufferSize, the Multicast will loose messages if the buffer of the linked target is "full":
https://stackoverflow.com/questions/22127660/broadcastblock-with-guaranteed-delivery-in-tpl-dataflow/22128371#22128371
This should be avoided, e.g. by replacing the BroadcastBlock

# Improved Odbc support:

This is only relevant for Unknown Odbc (or OleDb) source. For better Odbc supportl also  look at DbSchemaReader(martinjw) in github.
Currently, if not table definition is given, the current implementation of TableDefintion.FromTable name throws an exception that the table does not exists (though it does). 
For known Odbc connection (like Sql Server), the sql is known, but for the "default" odbc connection there can't be a sql to get the table definition. But this could be done using the Ado.NET schema objects. 
It would be good if the connection manager would return the code how to find if a table exists. Then the normal conneciton managers would run some sql code, and the Odbc could use ADO.NET to retrieve if the table exists and to get the table definition (independent from the database).

# New feature

- CopyTableDefinitionTask - uses TableDefinition to retrieve the current table definiton and the creates a new table. 
Very good for testing purposes.


# Oracle

Add missing tests for specific data type conversions. E.g. number(22,2) should also create the correct .net datatype. Currently the DataTypeConverter will parse it into System.String.


