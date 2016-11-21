﻿using System;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

//important
using LambdicSql;
using LambdicSql.feat.Dapper;
using static LambdicSql.Keywords;
using LambdicSql.SqlBase;
using System.Linq.Expressions;

namespace TestCheck35
{
    public class TestKeywordCondition : ITest
    {
        public IDbConnection _connection;

        public void TestInitialize(IDbConnection connection)
        {
            _connection = connection;
        }

        public class SelectData
        {
            public int Id { get; set; }
        }

        public void Test_Like()
        {
            var query = Sql<DB>.Create(db =>
               Select(new SelectData
               {
                   Id = db.tbl_staff.id
               }).
               From(db.tbl_staff).
               Where(Like(db.tbl_staff.name, "%a%")));

            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	tbl_staff.id AS Id
FROM tbl_staff
WHERE tbl_staff.name LIKE @p_0");
        }

        public void Test_Between()
        {
            var query = Sql<DB>.Create(db =>
               Select(new SelectData
               {
                   Id = db.tbl_staff.id
               }).
               From(db.tbl_staff).
               Where(Between(db.tbl_staff.id, 0, 100)));

            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	tbl_staff.id AS Id
FROM tbl_staff
WHERE tbl_staff.id BETWEEN @p_0 AND @p_1");
        }

        public void Test_In1()
        {
            var query = Sql<DB>.Create(db =>
               Select(new SelectData
               {
                   Id = db.tbl_staff.id
               }).
               From(db.tbl_staff).
               Where(In(db.tbl_staff.id, 1, 2, 3, 4, 5)));

            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	tbl_staff.id AS Id
FROM tbl_staff
WHERE tbl_staff.id IN(@p_0, @p_1, @p_2, @p_3, @p_4)");
        }

        public void Test_In2()
        {
            var sub = Sql<DB>.Create(db =>
                Select(new { id = db.tbl_remuneration.staff_id }).
                From(db.tbl_remuneration).
                Join(db.tbl_staff, db.tbl_staff.id == db.tbl_staff.id).
                Where(1000 < db.tbl_remuneration.money));

            var query = Sql<DB>.Create(db =>
               Select(new SelectData
               {
                   Id = db.tbl_staff.id
               }).
               From(db.tbl_staff).
               Where(In(db.tbl_staff.id, sub)));

            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	tbl_staff.id AS Id
FROM tbl_staff
WHERE tbl_staff.id IN(
	(SELECT
		tbl_remuneration.staff_id AS id
	FROM tbl_remuneration
		JOIN tbl_staff ON (tbl_staff.id) = (tbl_staff.id)
	WHERE (@p_0) < (tbl_remuneration.money)))");
        }

        public void Test_In3()
        {
            var query = Sql<DB>.Create(db =>
               Select(new SelectData
               {
                   Id = db.tbl_staff.id
               }).
               From(db.tbl_staff).
               Where(In(db.tbl_staff.id,
                    Select(new { id = db.tbl_remuneration.staff_id }).
                    From(db.tbl_remuneration).
                    Join(db.tbl_staff, db.tbl_staff.id == db.tbl_staff.id).
                    Where(1000 < db.tbl_remuneration.money)
               )));

            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	tbl_staff.id AS Id
FROM tbl_staff
WHERE tbl_staff.id IN(
	(SELECT
		tbl_remuneration.staff_id AS id
	FROM tbl_remuneration
		JOIN tbl_staff ON (tbl_staff.id) = (tbl_staff.id)
	WHERE (@p_0) < (tbl_remuneration.money)))");
        }

        public void Test_Exists1()
        {
            var sub = Sql<DB>.Create(db =>
                Select(new { id = db.tbl_remuneration.staff_id }).
                From(db.tbl_remuneration).
                Join(db.tbl_staff, db.tbl_staff.id == db.tbl_staff.id).
                Where(1000 < db.tbl_remuneration.money));

            var query = Sql<DB>.Create(db =>
               Select(new SelectData
               {
                   Id = db.tbl_staff.id
               }).
               From(db.tbl_staff).
               Where(Exists(sub)));

            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	tbl_staff.id AS Id
FROM tbl_staff
WHERE EXISTS
	(SELECT
		tbl_remuneration.staff_id AS id
	FROM tbl_remuneration
		JOIN tbl_staff ON (tbl_staff.id) = (tbl_staff.id)
	WHERE (@p_0) < (tbl_remuneration.money))");
        }
        
        public void Test_Exists2()
        {
            var query = Sql<DB>.Create(db =>
               Select(new SelectData
               {
                   Id = db.tbl_staff.id
               }).
               From(db.tbl_staff).
               Where(Exists(
                    Select(new { id = db.tbl_remuneration.staff_id }).
                        From(db.tbl_remuneration).
                        Join(db.tbl_staff, db.tbl_staff.id == db.tbl_staff.id).
                        Where(1000 < db.tbl_remuneration.money)
                   )));
            
            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
@"SELECT
	tbl_staff.id AS Id
FROM tbl_staff
WHERE EXISTS
	(SELECT
		tbl_remuneration.staff_id AS id
	FROM tbl_remuneration
		JOIN tbl_staff ON (tbl_staff.id) = (tbl_staff.id)
	WHERE (@p_0) < (tbl_remuneration.money))");
        }

        public void Test_IsNull()
        {
            var query = Sql<DB>.Create(db =>
               Select(new SelectData
               {
                   Id = db.tbl_staff.id
               }).
               From(db.tbl_staff).
               Where(!IsNull(db.tbl_staff.name)));

            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
 @"SELECT
	tbl_staff.id AS Id
FROM tbl_staff
WHERE NOT (tbl_staff.name IS NULL)");
        }

        public void Test_IsNotNull()
        {
            var query = Sql<DB>.Create(db =>
               Select(new SelectData
               {
                   Id = db.tbl_staff.id
               }).
               From(db.tbl_staff).
               Where(IsNotNull(db.tbl_staff.name)));

            var datas = _connection.Query(query).ToList();
            Assert.IsTrue(0 < datas.Count);
            AssertEx.AreEqual(query, _connection,
 @"SELECT
	tbl_staff.id AS Id
FROM tbl_staff
WHERE tbl_staff.name IS NOT NULL");
        }
    }
}
