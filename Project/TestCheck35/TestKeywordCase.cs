﻿using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Helper;
using static Test.Helper.DBProviderInfo;

//important
using LambdicSql;
using LambdicSql.feat.Dapper;
using static LambdicSql.Keywords;
using static LambdicSql.Funcs;

namespace TestCheck35
{
    [TestClass]
    public class TestKeywordCase
    {
        public TestContext TestContext { get; set; }
        public IDbConnection _connection;

        [TestInitialize]
        public void TestInitialize()
        {
            _connection = TestEnvironment.CreateConnection(TestContext);
            _connection.Open();
        }

        [TestCleanup]
        public void TestCleanup() => _connection.Dispose();

        public class SelectData
        {
            public string Type { get; set; }
        }

        [TestMethod, DataSource(Operation, Connection, Sheet, Method)]
        public void Test_Case1()
        {
            var query = Sql<DB>.Create(db =>
                Select(new SelectData()
                {
                    Type = Case().
                                When(db.tbl_staff.id == 3).Then("x").
                                When(db.tbl_staff.id == 4).Then("y").
                                Else("z").
                            End()
                }).
                From(db.tbl_staff));

            query.Gen(_connection);

            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
 @"SELECT
	CASE
		WHEN (tbl_staff.id) = (@p_0) THEN @p_1
		WHEN (tbl_staff.id) = (@p_2) THEN @p_3
		ELSE @p_4
	END AS Type
FROM tbl_staff",
 3, "x", 4, "y", "z");
        }

        [TestMethod, DataSource(Operation, Connection, Sheet, Method)]
        public void Test_Case2()
        {
            var query = Sql<DB>.Create(db =>
                Select(new SelectData()
                {
                    Type = Case().
                                When(db.tbl_staff.id == 3).Then("x").
                                When(db.tbl_staff.id == 4).Then("y").
                            End()
                }).
                From(db.tbl_staff));
            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	CASE
		WHEN (tbl_staff.id) = (@p_0) THEN @p_1
		WHEN (tbl_staff.id) = (@p_2) THEN @p_3
	END AS Type
FROM tbl_staff",
3, "x", 4, "y");
        }

        [TestMethod, DataSource(Operation, Connection, Sheet, Method)]
        public void Test_Case3()
        {
            var query = Sql<DB>.Create(db =>
                Select(new SelectData()
                {
                    Type = Case(db.tbl_staff.id).
                                When(3).Then("x").
                                When(4).Then("y").
                                Else("z").
                            End()
                }).
                From(db.tbl_staff));
            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	CASE tbl_staff.id
		WHEN @p_0 THEN @p_1
		WHEN @p_2 THEN @p_3
		ELSE @p_4
	END AS Type
FROM tbl_staff",
3, "x", 4, "y", "z");
        }

        [TestMethod, DataSource(Operation, Connection, Sheet, Method)]
        public void Test_Case4()
        {
            var query = Sql<DB>.Create(db =>
                Select(new SelectData()
                {
                    Type = Case(db.tbl_staff.id).
                                When(3).Then("x").
                                When(4).Then("y").
                            End()
                }).
                From(db.tbl_staff));
            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	CASE tbl_staff.id
		WHEN @p_0 THEN @p_1
		WHEN @p_2 THEN @p_3
	END AS Type
FROM tbl_staff",
3, "x", 4, "y");
        }

        [TestMethod, DataSource(Operation, Connection, Sheet, Method)]
        public void Test_Case5()
        {
            var val = 0;
            var query = Sql<DB>.Create(db =>
                Select(new SelectData()
                {
                    Type = Case().
                                When(val == 3).Then(db.tbl_staff.name).
                                When(val == 4).Then(db.tbl_staff.name).
                                Else(db.tbl_staff.name).
                            End()
                }).
                From(db.tbl_staff));
            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	CASE
		WHEN (@val) = (@p_0) THEN tbl_staff.name
		WHEN (@val) = (@p_1) THEN tbl_staff.name
		ELSE tbl_staff.name
	END AS Type
FROM tbl_staff", new Params()
{
    { "@val", 0 },
    { "@p_0", 3 },
    { "@p_1", 4 }
});
        }

        [TestMethod, DataSource(Operation, Connection, Sheet, Method)]
        public void Test_Case6()
        {
            var exp1 = Sql<DB>.Create(db => db.tbl_staff.id == 3);
            var exp2 = Sql<DB>.Create(db => "x");
            var exp3 = Sql<DB>.Create(db => db.tbl_staff.id == 4);
            var exp4 = Sql<DB>.Create(db => "y");
            var exp5 = Sql<DB>.Create(db => "z");
            var query = Sql<DB>.Create(db =>
                Select(new SelectData()
                {
                    Type = Case().
                                When(exp1).Then(exp2.Body).
                                When(exp3).Then(exp4).
                                Else(exp5).
                            End()
                }).
                From(db.tbl_staff));
            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
 @"SELECT
	CASE
		WHEN (tbl_staff.id) = (@p_0) THEN @p_1
		WHEN (tbl_staff.id) = (@p_2) THEN @p_3
		ELSE @p_4
	END AS Type
FROM tbl_staff",
 3, "x", 4, "y", "z");
        }

        [TestMethod, DataSource(Operation, Connection, Sheet, Method)]
        public void Test_Case7()
        {
            var val = 0;
            var query = Sql<DB>.Create(db =>
                Select(new SelectData()
                {
                    Type = Case(val).
                                When(db.tbl_staff.id).Then(db.tbl_staff.name).
                                When(db.tbl_staff.id).Then(db.tbl_staff.name).
                                Else(db.tbl_staff.name).
                            End()
                }).
                From(db.tbl_staff));
            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	CASE @val
		WHEN tbl_staff.id THEN tbl_staff.name
		WHEN tbl_staff.id THEN tbl_staff.name
		ELSE tbl_staff.name
	END AS Type
FROM tbl_staff", new Params { { "@val", 0 } });
        }

        [TestMethod, DataSource(Operation, Connection, Sheet, Method)]
        public void Test_Case8()
        {
            var exp1 = Sql<DB>.Create(db => db.tbl_staff.id);
            var exp2 = Sql<DB>.Create(db => 3);
            var exp3 = Sql<DB>.Create(db => "x");
            var exp4 = Sql<DB>.Create(db => 4);
            var exp5 = Sql<DB>.Create(db => "y");
            var exp6 = Sql<DB>.Create(db => "z");
            var query = Sql<DB>.Create(db =>
                Select(new SelectData()
                {
                    Type = Case(exp1).
                                When(exp2).Then(exp3.Body).
                                When(exp4).Then(exp5).
                                Else(exp6).
                            End()
                }).
                From(db.tbl_staff));
            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	CASE tbl_staff.id
		WHEN @p_0 THEN @p_1
		WHEN @p_2 THEN @p_3
		ELSE @p_4
	END AS Type
FROM tbl_staff",
3, "x", 4, "y", "z");
        }
        
        [TestMethod, DataSource(Operation, Connection, Sheet, Method)]
        public void Test_Case9()
        {
            var exp1 = Sql<DB>.Create(db => db.tbl_staff.id);
            var exp2 = Sql<DB>.Create(db => 3);
            var exp3 = Sql<DB>.Create(db => "x");
            var exp4 = Sql<DB>.Create(db => 4);
            var exp5 = Sql<DB>.Create(db => "y");
            var exp6 = Sql<DB>.Create(db => "z");
            var caseQuery = Sql<DB>.Create(db =>
                Case(exp1).
                    When(exp2).Then(exp3.Body).
                    When(exp4).Then(exp5).
                    Else(exp6).
                End()
                );
            var query = Sql<DB>.Create(db =>
                Select(new SelectData()
                {
                    Type = caseQuery
                }).
                From(db.tbl_staff));
            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	CASE tbl_staff.id
		WHEN @p_0 THEN @p_1
		WHEN @p_2 THEN @p_3
		ELSE @p_4
	END AS Type
FROM tbl_staff",
3, "x", 4, "y", "z");
        }

        [TestMethod, DataSource(Operation, Connection, Sheet, Method)]
        public void Test_Sub1()
        {
            var sub = Sql<DB>.Create(db => Select(Count(new Asterisk())).From(db.tbl_staff));
            var query = Sql<DB>.Create(db =>
                Select(new
                {
                    Type = Case().
                                When(sub.Cast<int>() == 1).Then("x").
                                Else("z").
                            End()
                }).
                From(db.tbl_staff));

            query.Gen(_connection);

            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	CASE
		WHEN
			((SELECT
				COUNT(*)
			FROM tbl_staff))
			 = 
			(@p_0)
			THEN @p_1
		ELSE @p_2
	END AS Type
FROM tbl_staff", 1, "x", "z");
        }

        [TestMethod, DataSource(Operation, Connection, Sheet, Method)]
        public void Test_Sub2()
        {
            var sub = Sql<DB>.Create(db => Select(Count(new Asterisk())).From(db.tbl_staff));
            var query = Sql<DB>.Create(db =>
                Select(new
                {
                    Type = Case().
                                When(1 == sub.Cast<int>()).Then("x").
                                Else("z").
                            End()
                }).
                From(db.tbl_staff));

            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	CASE
		WHEN
			(@p_0)
			 = 
			((SELECT
				COUNT(*)
			FROM tbl_staff))
			THEN @p_1
		ELSE @p_2
	END AS Type
FROM tbl_staff", 1, "x", "z");
        }
    }
}
