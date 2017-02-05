﻿LambdicSql_β 0.30.0
======================

## Features ...
LambdicSql is ultimate sql builder.<br>
Generate sql text and parameters from lambda. 
<img src="https://github.com/Codeer-Software/LambdicSql/blob/master/lambdicSqlImage.png" width="400">
## Getting Started
LambdicSql from NuGet

    PM> Install-Package LambdicSql

https://www.nuget.org/packages/LambdicSql/<br>
Supported pratforms are
- .NETFramework 3.5~
- PCL
- .NETStandard 1.2~

## Featuring Dapper
Generate sql text and parameters by LambdicSql.<br>
And execut and map to object, recommend using dapper.

    PM> Install-Package Dapper

First you need to put the initialization code.
```csharp
//full .net
DapperAdapter.Assembly = typeof(Dapper.SqlMapper).Assembly;

//.net standard
DapperAdapter.Assembly = typeof(Dapper.SqlMapper).GetTypeInfo().Assembly;
```
## Featuring sqlite-net-pcl
For PCL, recommend sqlite-net-pcl.

    PM> Install-Package sqlite-net-pcl

## Quick Start.
Standard code.
```csharp
using System;
using System.Linq;

//LambdicSql
using LambdicSql;
using static LambdicSql.Symbol;

//for SqlServer and Dapper.
//Of course, other connections are OK.
//OracleConnection, SQLiteConnection, NpgsqlConnection, MySqlConnection, DB2Connection
using System.Data.SqlClient;
using LambdicSql.feat.Dapper;
using System.Data;

//or for sqlite-net-pcl
//using LambdicSql.feat.SQLiteNetPcl

namespace Sample
{
    //tables.
    public class Staff
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Remuneration
    {
        public int id { get; set; }
        public int staff_id { get; set; }
        public DateTime payment_date { get; set; }
        public decimal money { get; set; }
    }

    public class DB
    {
        public Staff tbl_staff { get; set; }
        public Remuneration tbl_remuneration { get; set; }
    }

    public class SelectData
    {
        public string Name { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Money { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            DapperAdapter.Assembly = typeof(Dapper.SqlMapper).Assembly;
            using (var cnn = new SqlConnection("your connection string")) Sample(cnn);
        }

        static void Sample(IDbConnection cnn)
        { 
            var min = 3000;

            //make sql.
            var sql = Db<DB>.Sql(db =>

                //--------------It is lambda expressing SQL---------------------------------
                Select(new SelectData
                {
                    Name = db.tbl_staff.name,
                    PaymentDate = db.tbl_remuneration.payment_date,
                    Money = db.tbl_remuneration.money,
                }).
                From(db.tbl_remuneration).
                    Join(db.tbl_staff, db.tbl_staff.id == db.tbl_remuneration.staff_id).
                Where(min < db.tbl_remuneration.money && db.tbl_remuneration.money < 4000).
                OrderBy(Asc(db.tbl_remuneration.money), Desc(db.tbl_staff.name))
                //--------------------------------------------------------------------------

                );

            //to string and params.
            var info = sql.Build(cnn.GetType());

            //print.
            Console.WriteLine(info.Text);
            Console.WriteLine("\r\nParams");
            foreach (var e in info.GetParams()) Console.WriteLine($"{e.Key} = {e.Value.Value}");

            //execute query by dapper or sql-net-pcl.
            var datas = cnn.Query(sql).ToList();
        }
    }
}
```
<img src="https://github.com/Codeer-Software/LambdicSql/blob/master/SummaryCode.png">
## Supported keywords
|||||||
|:--|:--|:--|:--|:--|:--|
|SELECT|FROM|WHERE|ORDER BY|HAVING|
|GROUP BY|GROUP BY ROLLUP|GROUP BY WITH ROLLUP|GROUP BY CUBE|GROUP BY GROUPING SETS|
|JOIN|LEFT JOIN|RIGHT JOIN|FULL JOIN|CROSS JOIN|
|CASE|WHEN|THEN|END|
|LIMIT|OFFSET|OFFSET ROWS|FETCH NEXT ROWS ONLY|TOP|
|UNION|INTERSECT|EXCEPT|MINUS|
|UPDATE|SET|INSERT INTO|VALUES|DELETE|
|LIKE|BETWEEN|IN|EXISTS|
|ASC|DESC|ALL|DISTINCT|IS NULL|IS NOT NULL|
|WITH|RECURSIVE|ROWNUM|*|DUAL|SYSIBM.SYSDUMMY1|
|CURRENT_DATE|CURRENT_TIME|CURRENT_TIMESTAMP|
|CREATE DATABASE|DROP DATABASE|CREATE TABLE|CREATE TABLE IF NOT EXISTS|DROP TABLE|DROP TABLE IF EXISTS|
|CONSTRAINT|PRIMARY KEY|FOREIGN KEY|CHECK|UNIQUE|NOT NULL|
|DEFAULT|REFERENCES|RESTRICT|CASCADE|
## Supported functions
||||||||
|:--|:--|:--|:--|:--|:--|:--|
|SUM|COUNT|AVG|MIN|MAX|ABS|MOD|
|ROUND|CONCAT||LENGH|LEN|LOWER|UPPER|REPLACE|
|SUBSTRING|EXTRACT|DATEPART|CAST|COALESCE|FIRST_VALUE|LAST_VALUE|
|DENSE_RANK|PERCENT_RANK|CUME_DIST|NTILE|NTH_VALUE|RANK|LAG|
|ROW_NUMBER|OVER|ROWS|PARTITION BY|
## Samples
Look for how to use from the tests.<br>
https://github.com/Codeer-Software/LambdicSql/tree/master/Project/Test.NET35
## Supported database
|DataBase type|Support|
|:--|:--|
|SQL Server|○|
|SQLite|○|
|PostgreSQL|○|
|Oracle|○|
|MySQL|○|
|DB2|○|

## Sub query.
```csharp
public void TestSubQueryAtSelect()
{
    var sql = Db<DB>.Sql(db =>
        Select(new SelectedData
        {
            Name = db.tbl_staff.name,
            Total = Select(Sum(db.tbl_remuneration.money)).
                        From(db.tbl_remuneration)
        }).
        From(db.tbl_staff));
        
    //Subqueries can also be written separately.
    //Here is the same SQL as above.
    var sub = Db<DB>.Sql(db => 
        Select(Sum(db.tbl_remuneration.money)).
        From(db.tbl_remuneration)
        );

    sql = Db<DB>.Sql(db =>
        Select(new SelectedData
        {
            Name = db.tbl_staff.name,
            Total = sub
        }).
        From(db.tbl_staff));
}
```
```sql
SELECT
        tbl_staff.name AS Name,
        (SELECT
                SUM(tbl_remuneration.money)
        FROM tbl_remuneration) AS Total
FROM tbl_staff
```
```csharp
public void TestSubQueryAtFrom()
{
    //For the where clause, you need to write the subqueries separately.
    var sub = Db<DB>.Sql(db => 
        Select(new 
        {
            name_sub = db.tbl_staff.name,
            PaymentDate = db.tbl_remuneration.payment_date,
            Money = db.tbl_remuneration.money,
        }).
        From(db.tbl_remuneration).
            Join(db.tbl_staff, db.tbl_remuneration.staff_id == db.tbl_staff.id).
        Where(3000 < db.tbl_remuneration.money && db.tbl_remuneration.money < 4000));
    
    var sql = Db<DB>.Sql(db => 
        Select(new SelectData
        {
            Name = sub.Body.name_sub
        }).
        From(sub));
}
```
```sql
SELECT
	subQuery.name_sub AS Name
FROM 
	(SELECT
		tbl_staff.name AS name_sub,
		tbl_remuneration.payment_date AS PaymentDate,
		tbl_remuneration.money AS Money
	FROM tbl_remuneration
		JOIN tbl_staff ON (tbl_remuneration.staff_id) = (tbl_staff.id)
	WHERE ((@p_2) < (tbl_remuneration.money)) AND ((tbl_remuneration.money) < (@p_3))) AS subQuery
```
## Combining queries.
It can be combined parts.
Query, Sub query, Expression.
You can write DRY code.
```csharp
public void TestQueryConcat()
{
    //make sql.
    var select = Db<DB>.Sql(db =>
        Select(new SelectData1
        {
            Name = db.tbl_staff.name,
            PaymentDate = db.tbl_remuneration.payment_date,
            Money = db.tbl_remuneration.money,
        }));

    var from = Db<DB>.Sql(db =>
         From(db.tbl_remuneration).
        Join(db.tbl_staff, db.tbl_remuneration.staff_id == db.tbl_staff.id));

    var where = Db<DB>.Sql(db =>
        Where(3000 < db.tbl_remuneration.money && db.tbl_remuneration.money < 4000));

    var orderby = Db<DB>.Sql(db =>
         OrderBy(Asc(db.tbl_staff.name)));

    var sql = select + from + where + orderby;

    //to string and params.
    var info = sql.Build(_connection.GetType());
    Debug.Print(info.Text);

    //dapper
    var datas = _connection.Query(sql).ToList();
}

```
```sql
SELECT
	tbl_staff.name AS Name,
	tbl_remuneration.payment_date AS PaymentDate,
	tbl_remuneration.money AS Money
FROM tbl_remuneration
	JOIN tbl_staff ON (tbl_remuneration.staff_id) = (tbl_staff.id)
WHERE ((@min) < (tbl_remuneration.money)) AND ((tbl_remuneration.money) < (@p_1))
ORDER BY
	tbl_staff.name ASC
```
Expressions.
```csharp
public void TestSqlExpression()
{
    //make sql.
    var expMoneyAdd = Db<DB>.Sql(db => db.tbl_remuneration.money + 100);
    var expWhereMin = Db<DB>.Sql(db => 3000 < db.tbl_remuneration.money);
    var expWhereMax = Db<DB>.Sql(db => db.tbl_remuneration.money < 4000);

    var sql = Db<DB>.Sql(db =>
       Select(new SelectData1
       {
           Name = db.tbl_staff.name,
           PaymentDate = db.tbl_remuneration.payment_date,
           Money = expMoneyAdd,
       }).
       From(db.tbl_remuneration).
           Join(db.tbl_staff, db.tbl_remuneration.staff_id == db.tbl_staff.id).
       Where(expWhereMin && expWhereMax).
       OrderBy(Asc(db.tbl_staff.name)));

    //to string and params.
    var info = sql.Build(_connection.GetType());
    Debug.Print(info.Text);

    //dapper
    var datas = _connection.Query(sql).ToList();
}
```
```sql
SELECT
	tbl_staff.name AS Name,
	tbl_remuneration.payment_date AS PaymentDate,
	(tbl_remuneration.money) + (@p_0) AS Money
FROM tbl_remuneration
	JOIN tbl_staff ON (tbl_remuneration.staff_id) = (tbl_staff.id)
WHERE ((@p_1) < (tbl_remuneration.money)) AND ((tbl_remuneration.money) < (@p_2))
ORDER BY
	tbl_staff.name ASC
```

## Condition utility
```csharp
public void TestCondition(bool minCondition, bool maxCondition)
{
    //Condition is written only when the first argument is valid.
    var exp = Db<DB>.Sql(db =>
        new Condition(minCondition, 3000 < db.tbl_remuneration.money) &&
        new Condition(maxCondition, db.tbl_remuneration.money < 4000));

    var query = Db<DB>.Sql(db =>
        Select(Asterisk()).
        From(db.tbl_remuneration).
        Where(exp)
    );
}
```
min max enable.
```sql
SELECT *
FROM tbl_remuneration
WHERE ((@p_0) < (tbl_remuneration.money)) AND ((tbl_remuneration.money) < (@p_1))
```
min, max disable, vanish where clause.
```sql
SELECT *
FROM tbl_remuneration
```
## Support for combination of the text.
You can insert text to expression.
And insert expression to text.
```csharp
//You can use text.
public void TestFormatText()
{
    //make sql.
    var sql = Db<DB>.Sql(db =>
        Select(new SelectedData
        {
            name = db.tbl_staff.name,
            payment_date = db.tbl_remuneration.payment_date,
            money = (decimal)"{0} + 1000".ToSql(db.tbl_remuneration.money),
        }).
        From(db.tbl_remuneration).
            Join(db.tbl_staff, db.tbl_remuneration.staff_id == db.tbl_staff.id).
        Where(3000 < db.tbl_remuneration.money && db.tbl_remuneration.money < 4000));

    //to string and params.
    var info = sql.Build(_connection.GetType());
    Debug.Print(info.SqlText);

    //if you installed dapper, use this extension.
    var datas = _connection.Query(sql).ToList();
}
```
```sql
SELECT
	tbl_staff.name AS name,
	tbl_remuneration.payment_date AS payment_date,
	tbl_remuneration.money + 1000 AS money
FROM tbl_remuneration
	JOIN tbl_staff ON (tbl_remuneration.staff_id) = (tbl_staff.id)
WHERE ((@p_0) < (tbl_remuneration.money)) AND ((tbl_remuneration.money) < (@p_1))
```
## 2 way sql.
Do you know 2 way sql?
It's executable sql text.
And change by condition and keyword.
```csharp
//2 way sql
public void TestFormat2WaySql()
{
    TestFormat2WaySql(true, true, 1000);
}
public void TestFormat2WaySql(bool minCondition, bool maxCondition, decimal bonus)
{
    //make sql.
    //replace /*no*/---/**/.
    var text = @"
SELECT
tbl_staff.name AS name,
tbl_remuneration.payment_date AS payment_date,
tbl_remuneration.money + /*0*/1000/**/ AS money
FROM tbl_remuneration 
JOIN tbl_staff ON tbl_staff.id = tbl_remuneration.staff_id
/*1*/WHERE tbl_remuneration.money = 100/**/";
    
    var sql = Db<DB>.Sql(db => text.TwoWaySql(
        bonus,
        Where(
            new Condition(minCondition, 3000 < db.tbl_remuneration.money) &&
            new Condition(maxCondition, db.tbl_remuneration.money < 4000))
        ));
    var info = sql.Build(_connection.GetType());
    Debug.Print(info.SqlText);

    //if you installed dapper, use this extension.
    var datas = _connection.Query<SelectedData>(sql).ToList();
}
```
```sql
SELECT
	tbl_staff.name AS name,
    tbl_remuneration.payment_date AS payment_date,
	tbl_remuneration.money + @bonus AS money
FROM tbl_remuneration 
    JOIN tbl_staff ON tbl_staff.id = tbl_remuneration.staff_id
WHERE ((@p_0) < (tbl_remuneration.money)) AND ((tbl_remuneration.money) < (@p_1))
```
## Add clauses and functions
If there are missing clauses or functions you can easily add them.
Definitions are never executed, so you do not need to implement the inside of the method.
```csharp
static class MyFuncs
{
    [FuncStyleConverter]
    internal static Cos(double angle) { throw new NotSupportedException(); }

    //The limit clause has already been implemented with LambdicSql, but here we write it as a sample.
    [ClauseStyleConverter]
    public static ClauseChain<Non> Limit(object offset, object count) { throw new InvalitContextException(nameof(Limit)); }

    [ClauseStyleConverter]
    public static ClauseChain<TSelected> Limit<TSelected>(this ClauseChain<TSelected> before, object offset, object count) { throw new InvalitContextException(nameof(Limit)); }

    [MethodFormatConverter(Format = "TABLESAMPLE [0](|[1])")]
    public static ClauseChain<Non> TableSample(SamplingMethod method, double percentage) { throw new InvalitContextException(nameof(Limit)); }

    [MethodFormatConverter(Format = "TABLESAMPLE [1](|[2])")]
    public static ClauseChain<TSelected> TableSample<TSelected>(this ClauseChain<TSelected> before, SamplingMethod method, double percentage) { throw new InvalitContextException(nameof(Limit)); } 
}
[EnumToStringConverter]
public enum SamplingMethod
{
    //Become capitalized.
    System,

    //You can also specify the converted character string.
    [FieldSqlName("BERNOULLI")]
    Bernoulli
}
```
MethodFormatConverter's rules.

|Symbol|content|
|:--|:--|
|&#X7c;|Even if a line break is entered, it remains in the first row up to that position.|
|[i]|Insert i th argument|
|[<, >i]| Expands the ith argument. The argument must be an array. In the <>, specify a separator.|
|[$i]| Insert character string in SQL with direct value without using parameter.|
|[#i]| When a column is entered Make it a column name without a table name.|
|[!i]| Special character string. Put the specified character string directly into SQL. Used for db name and constraint name.|
If you want to control more conversion, please create a class that inherits the following attributes.

|Attribute|content|
|:--|:--|
|MethodConverterAttribute|Method|
|NewConverterAttribute|Constructor|
|ObjectConverterAttribute|Conversion of objects. Use for Type|
|MemberConverterAttribute|Field, Property|

LambdicSql itself also defines functions and phrases using the mechanism described here, so I think that it will be a sample. Please see samples.<br>
https://github.com/Codeer-Software/LambdicSql/blob/master/Project/LambdicSql.Shared/Symbol.Clauses.cs
https://github.com/Codeer-Software/LambdicSql/blob/master/Project/LambdicSql.Shared/Symbol.Etc.cs
https://github.com/Codeer-Software/LambdicSql/blob/master/Project/LambdicSql.Shared/Symbol.Funcs.cs
https://github.com/Codeer-Software/LambdicSql/blob/master/Project/LambdicSql.Shared/SymbolSub.cs
