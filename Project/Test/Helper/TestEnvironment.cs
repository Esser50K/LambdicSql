﻿using LambdicSql.QueryBase;
using LambdicSql.SqlServer;
using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using Test.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    static class TestEnvironment
    {
        internal static IDbAdapter Adapter => new SqlServerAdapter(File.ReadAllText(FindNearFile("db.txt")).Trim());
        internal static string ConnectionString => File.ReadAllText(FindNearFile("db.txt")).Trim();
        internal static string SQLiteTest1Path => Path.GetFullPath("../../../SQLiteTest1.db");

        static string FindNearFile(string fileName)
        {
            var path = typeof(TestEnvironment).Assembly.Location;
            while (true)
            {
                var filePath = Path.Combine(path, fileName);
                if (File.Exists(filePath))
                {
                    return filePath;
                }
                path = Path.GetDirectoryName(path);
            }
            throw new NotSupportedException();
        }

        internal static IDbConnection CreateConnection(object db)
        {
            switch (db.ToString())
            {
                case "SQLServer": return new SqlConnection(ConnectionString);
                case "SQLite": return new SQLiteConnection("Data Source=" + SQLiteTest1Path);
            }
            throw new NotSupportedException();
        }
    }

   // [TestClass]
    public class Initializer
    {
        [TestMethod]
        public void CreateSQLiteTable()
        {
            var path = TestEnvironment.SQLiteTest1Path;
            File.Delete(path);
            using (var connection = new SQLiteConnection("Data Source=" + path))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = Resources.CreateTableRemuneration;
                    command.ExecuteNonQuery();
                    command.CommandText = Resources.CreateTableStaff;
                    command.ExecuteNonQuery();
                    command.CommandText = Resources.CreateTableDataChange;
                    command.ExecuteNonQuery();
                }
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        var staff = new object[][]
                        {
                            new object[] {1, "'Emma'"},
                            new object[] {2, "'Noah'"},
                            new object[] {3, "'Olivia'"},
                            new object[] {4, "'Jackson'" }
                        };
                        foreach (var e in staff)
                        {
                            command.CommandText = string.Format(Resources.InsertTableStaff, e[0], e[1]);
                            command.ExecuteNonQuery();
                        }

                        var remuneration = new object[][]
                        {
                            new object[] {1, 1, "2016/1/1",  3000.0000},
                            new object[] {2, 1, "2016/2/1",  3000.0000},
                            new object[] {3, 2, "2016/1/1",  2000.0000},
                            new object[] {4, 2, "2016/2/1",  2500.0000},
                            new object[] {5, 3, "2016/1/1",  4000.0000},
                            new object[] {6, 3, "2016/2/1",  4000.0000},
                            new object[] {7, 4, "2016/1/1",  3500.0000},
                            new object[] {8, 5, "2016/2/1",  3500.0000}
                        };
                        foreach (var e in remuneration)
                        {
                            command.CommandText = string.Format(Resources.InsertTableRemuneration, e[0], e[1], e[2], e[3]);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
        }
    }
}
