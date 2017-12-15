
using System;
//using NUnit.Framework;
using System.Data;
using System.Collections.Generic;
using ErikEJ.SqlCeScripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.RepoTest
{
    /// <summary>
    /// NOTE: this test class uses the adventureworks SQL server
    /// To run this test you need to download some version of AdventureWorks for SQL server and configure your connection string below
    /// Download path for the Database is here: https://github.com/Microsoft/sql-server-samples/releases/tag/adventureworks
    /// This particular test was programmed for the SQLServer2014 version but other versions should also work.
    /// Example data connection string:  Data Source=localhost;Initial Catalog=AdventureWorks2014;Integrated Security=True
    /// </summary>
    [TestClass]
    public class SQLServerAdventureWorkTest
    {
        //Configure this connection string to your server:
        public string connectString = "Data Source=localhost;Initial Catalog=AdventureWorks2014;Integrated Security=True";

        [TestMethod]
        public void TestGetAllTableNames()
        {
            var list = new List<string>();
            using (IRepository repo = new ServerDBRepository4(connectString))
            {
                list = repo.GetAllTableNamesForExclusion();
            }
            Assert.IsTrue(list.Count > 20);
            Assert.IsTrue(list.Contains("Person.Address"));
        }

        [TestMethod]
        public void TestGetAllColumns()
        {
            var list = new List<Column>();
            using (IRepository repo = new ServerDBRepository4(connectString))
            {
                list = repo.GetAllColumns();
            }
            Assert.IsTrue(list.Count > 40);
        }

        [TestMethod]
        public void TestGetAllViews()
        {
            var list = new List<View>();
            using (IRepository repo = new ServerDBRepository4(connectString))
            {
                list = repo.GetAllViews();
            }
            Assert.IsTrue(list.Count > 5);
        }

        [TestMethod]
        public void TestGetAllViewColumns()
        {
            var list = new List<Column>();
            using (IRepository repo = new ServerDBRepository4(connectString))
            {
                list = repo.GetAllViewColumns();
            }
            Assert.IsTrue(list.Count > 11);
        }

        [TestMethod]
        public void TestGetAllTriggers()
        {
            var list = new List<Trigger>();
            using (IRepository repo = new ServerDBRepository4(connectString))
            {
                list = repo.GetAllTriggers();
            }
            Assert.IsTrue(list.Count >= 0);
        }


        [TestMethod]
        public void TestGetAllPrimaryKeys()
        {
            var list = new List<PrimaryKey>();
            using (IRepository repo = new ServerDBRepository4(connectString))
            {
                list = repo.GetAllPrimaryKeys();
            }
            Assert.IsTrue(list.Count > 20);
        }

        [TestMethod]
        public void TestGetAllForeignKeys()
        {
            var list = new List<ErikEJ.SqlCeScripting.Constraint>();
            using (IRepository repo = new ServerDBRepository4(connectString))
            {
                list = repo.GetAllForeignKeys();
            }
            Assert.IsTrue(list.Count > 11);
        }

        [TestMethod]
        public void TestGetAllIndexes()
        {
            var list = new List<Index>();
            using (IRepository repo = new ServerDBRepository4(connectString))
            {
                list = repo.GetAllIndexes();
            }
            Assert.IsTrue(list.Count > 22);
        }

        [TestMethod]
        public void TestGetIndexesFromTable()
        {
            var list = new List<Index>();
            using (IRepository repo = new ServerDBRepository4(connectString))
            {
                list = repo.GetIndexesFromTable("Address");  //ToDo think about including schema in the table name
            }
            Assert.IsTrue(list.Count > 1);
        }

        [TestMethod]
        public void TestGetDataFromReader()
        {
            var columns = new List<Column>
            {   new Column { ColumnName = "AddressTypeID"},
                new Column { ColumnName = "Name"},
                new Column { ColumnName = "rowguid"},
                new Column { ColumnName = "ModifiedDate"}
            };

            IDataReader reader = null;

            using (IRepository repo = new ServerDBRepository4(connectString, true))
            {
                reader = repo.GetDataFromReader("Person.AddressType", columns);
                while (reader.Read())
                {
                    Assert.IsTrue(reader.GetValue(0) is int);
                    Assert.IsTrue(reader.GetValue(3) is DateTime);
                }
            }
        }

        [TestMethod]
        public void TestTabDetails()
        {
            var tabDtl = new TabDetails();
            using (IRepository repo = new ServerDBRepository4(connectString, true))
            {
                tabDtl = repo.GetTableDetails("Person.Address");
            }
            Assert.IsTrue(tabDtl.TableName == "Address");
            Assert.IsTrue(tabDtl.Schema == "Person");
            Assert.IsTrue(tabDtl.Columns.Count == 9);
            Assert.IsTrue(tabDtl.Indexes.Count > 0);
            Assert.IsTrue(tabDtl.PrimaryKeys.Count > 0);
            Assert.IsTrue(tabDtl.FkConstraints.Count > 0);
        }

        [TestMethod]
        public void TestViewDetails()
        {
            var viewDtl = new ViewDetails();
            using (IRepository repo = new ServerDBRepository4(connectString, true))
            {
                viewDtl = repo.GetViewDetails("HumanResources.vEmployeeDepartment");
            }
            Assert.IsTrue(viewDtl.ViewName == "vEmployeeDepartment");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(viewDtl.Definition));
            Assert.IsTrue(viewDtl.Schema == "HumanResources");
            Assert.IsTrue(viewDtl.Columns.Count == 10);
        }
    }
}
