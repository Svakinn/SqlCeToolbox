using System;
//using NUnit.Framework;
using System.Data;
using System.Collections.Generic;
using ErikEJ.SQLiteScripting;
using ErikEJ.SqlCeScripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SQLiteScriptingTests
{
    public SQLiteScriptingTests()
    {
        dbPath = System.Environment.CurrentDirectory;
        var pos = dbPath.LastIndexOf(@"Tests\bin");
        dbPath = dbPath.Substring(0, pos) + @"Tests\";
    }
    //private const string dbPath = @"C:\Code\SqlCeToolbox\src\API\SqlCeScripting40\Tests\";
    private string dbPath = "";

    private string chinookConnectionString()
    {
        return string.Format(
        @"Data Source={0}chinook.db", dbPath);
    }
    private string infoConnectionString()
    {
        return string.Format(
        @"Data Source={0}inf2700_orders-1.db", dbPath);
    }
    private string fkConnectionString()
    {
        return string.Format(
        @"Data Source={0}FkMultiKey.db", dbPath);
    }

    private string viewsConnectionString()
    {
        return string.Format(
        @"Data Source={0}views.db", dbPath);
    }

    private string noRowIdConnectionString()
    {
        return string.Format(
        @"Data Source={0}norowid.db", dbPath);
    }

    [TestMethod]
    public void TestGetAllTableNames()
    {
        var list = new List<string>();
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            list = repo.GetAllTableNames();
        }
        Assert.IsTrue(list.Count == 11);
        Assert.IsTrue(list[0] == "Album");
    }

    [TestMethod]
    public void TestGetAllTableNames2()
    {
        var list = new List<string>();
        using (IRepository repo = new SQLiteRepository(infoConnectionString()))
        {
            list = repo.GetAllTableNames();
        }
        Assert.IsTrue(list.Count == 8);
    }

    [TestMethod]
    public void TestGetAllColumns()
    {
        var list = new List<Column>();
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            list = repo.GetAllColumns();
        }
        Assert.AreEqual(67, list.Count);
        Assert.AreEqual("bigint", list[0].DataType);
    }


    [TestMethod]
    public void TestGetAllColumns2()
    {
        var list = new List<Column>();
        using (IRepository repo = new SQLiteRepository(infoConnectionString()))
        {
            list = repo.GetAllColumns();
        }
        Assert.AreEqual(68, list.Count);
        Assert.AreEqual("bigint", list[0].DataType);
    }

    [TestMethod]
    public void TestGetAllViews()
    {
        var list = new List<View>();
        using (IRepository repo = new SQLiteRepository(infoConnectionString()))
        {
            list = repo.GetAllViews();
        }
        Assert.IsTrue(list.Count == 3);
    }

    [TestMethod]
    public void TestGetView()
    {
        var list = new List<View>();
        using (var repo = new SQLiteRepository(viewsConnectionString()))
        {
            list = repo.GetAllViews();
        }
        Assert.IsTrue(list.Count == 1);
    }

    [TestMethod]
    public void TestGetIndexesNoRowId()
    {
        var list = new List<Index>();
        using (var repo = new SQLiteRepository(noRowIdConnectionString()))
        {
            list = repo.GetAllIndexes();
        }
        Assert.IsTrue(list.Count == 0);
    }

    [TestMethod]
    public void TestGetAllViewColumns()
    {
        var list = new List<Column>();
        using (IRepository repo = new SQLiteRepository(infoConnectionString()))
        {
            list = repo.GetAllViewColumns();
        }
        Assert.IsTrue(list.Count == 11);
    }

    [TestMethod]
    public void TestGetAllTriggers()
    {
        var list = new List<Trigger>();
        using (IRepository repo = new SQLiteRepository(infoConnectionString()))
        {
            list = repo.GetAllTriggers();
        }
        Assert.IsTrue(list.Count == 0);
    }

    [TestMethod]
    public void TestGetAllPrimaryKeys()
    {
        var list = new List<PrimaryKey>();
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            list = repo.GetAllPrimaryKeys();
        }
        Assert.AreEqual(13, list.Count);
        Assert.IsTrue(list[0].KeyName == "sqlite_master_PK_Album");
    }

    [TestMethod]
    public void TestGetAllPrimaryKeys2()
    {
        var list = new List<PrimaryKey>();
        using (IRepository repo = new SQLiteRepository(infoConnectionString()))
        {
            list = repo.GetAllPrimaryKeys();
        }
        Assert.AreEqual(11, list.Count);
        Assert.AreEqual("CUSTOMER219ORDERS", list[0].KeyName);
    }

    [TestMethod]
    public void TestGetAllForeignKeys()
    {
        var list = new List<ErikEJ.SqlCeScripting.Constraint>();
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            list = repo.GetAllForeignKeys();
        }
        Assert.IsTrue(list.Count == 11);
        Assert.IsTrue(list[0].ConstraintName == "FK_Album_0_0");
    }

    [TestMethod]
    public void TestGetAllForeignKeysMultiColumnKey()
    {
        var list = new List<ErikEJ.SqlCeScripting.Constraint>();
        using (IRepository repo = new SQLiteRepository(fkConnectionString()))
        {
            list = repo.GetAllForeignKeys();
        }
        Assert.IsTrue(list.Count == 1);
        Assert.IsTrue(list[0].ConstraintName == "FK_BEVERAGE_DIRECTORY_0_0");
    }

    [TestMethod]
    public void TestGetAllIndexes()
    {
        var list = new List<Index>();
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            list = repo.GetAllIndexes();
        }
        Assert.IsTrue(list.Count == 22);
        Assert.IsTrue(list[0].IndexName == "IFK_AlbumArtistId");
    }

    [TestMethod]
    public void TestGetAllIndexes2()
    {
        var list = new List<Index>();
        using (IRepository repo = new SQLiteRepository(infoConnectionString()))
        {
            list = repo.GetAllIndexes();
        }
        Assert.IsTrue(list.Count == 0);
    }

    [TestMethod]
    public void TestGetIndexesFromTable()
    {
        var list = new List<Index>();
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            list = repo.GetIndexesFromTable("Album");
        }
        Assert.IsTrue(list.Count == 2);
        Assert.IsTrue(list[1].IndexName == "IPK_Album");
    }

    [TestMethod]
    public void TestDatabaseInfo()
    {
        var values = new List<KeyValuePair<string, string>>();
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            values = repo.GetDatabaseInfo();
        }
        Assert.IsTrue(values.Count == 4);
    }

    [TestMethod]
    public void TestParse()
    {
        var sql = "SELECT * FROM Album;" + Environment.NewLine + "GO";
        var result = string.Empty;
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            result = repo.ParseSql(sql);
        }
        Assert.IsTrue(result.StartsWith("SCAN "));

        sql = "SELECT * FROM Album WHERE AlbumId = 1;" + Environment.NewLine + "GO";
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            result = repo.ParseSql(sql);
        }
        Assert.IsTrue(result.StartsWith("SEARCH TABLE "));

    }

    [TestMethod]
    public void TestPragma()
    {
        var sql = "pragma table_info(Album);" + Environment.NewLine + "GO";
        DataSet result = null;
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            result = repo.ExecuteSql(sql);
        }
        Assert.IsTrue(result.Tables.Count == 1);
    }

    [TestMethod]
    public void TestGetDataFromReader()
    {
        var columns = new List<Column>
        {   new Column { ColumnName = "AlbumId"},
            new Column { ColumnName = "Title"},
            new Column { ColumnName = "ArtistId"},
        };

        IDataReader reader = null;

        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            reader = repo.GetDataFromReader("Album", columns);
            while (reader.Read())
            {
                Assert.IsTrue(reader.GetValue(0) is long);
                Assert.IsTrue(reader.GetValue(2) is long);
            }
        }
    }

    //Now tests for the new methods for table and view details
    [TestMethod]
    public void TestTabDetails()
    {
        var tabDtl = new TabDetails();
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            tabDtl = repo.GetTableDetails("Album");
        }
        Assert.IsTrue(!string.IsNullOrWhiteSpace(tabDtl.TableName));
        Assert.IsTrue(tabDtl.Columns.Count == 3);
        Assert.IsTrue(tabDtl.Indexes.Count > 0);
        Assert.IsTrue(tabDtl.PrimaryKeys.Count > 0);
        Assert.IsTrue(tabDtl.FkConstraints.Count > 0);
    }

    [TestMethod]
    public void TestViewDetails()
    {
        var viewDtl = new ViewDetails();
        using (IRepository repo = new SQLiteRepository(chinookConnectionString()))
        {
            viewDtl = repo.GetViewDetails("test");
        }
        Assert.IsTrue(!string.IsNullOrWhiteSpace(viewDtl.ViewName));
        Assert.IsTrue(!string.IsNullOrWhiteSpace(viewDtl.Definition));
        Assert.IsTrue(viewDtl.Columns.Count == 3);
    }
}
