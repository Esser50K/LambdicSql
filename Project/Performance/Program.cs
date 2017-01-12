﻿using LambdicSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using static LambdicSql.Symbol;

namespace Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
        }

        public class SelectedData
        {
            public string name { get; set; }
            public DateTime payment_date { get; set; }
            public decimal money { get; set; }
        }
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
        public class Data
        {
            public Staff tbl_staff { get; set; }
            public Remuneration tbl_remuneration { get; set; }
        }
        static void Test()
        {
            //いやいや、やっぱり文字列組み合わせがそんなに遅いわけはない。どこがボトルネックか？

            //リストへの投入は結構コストが高い
            //オブジェクトの生成もただではない


            Console.ReadKey();
            Console.WriteLine("Start");
            BuildedSql info = null;
            var times = new List<double>();
            var watch = new Stopwatch();


            for (int i = 0; i < 10000; i++)
            {
                watch.Start();


                var query = Db<Data>.Sql(db =>

                    Select(new SelectedData()
                    {
                        name = db.tbl_staff.name,
                        payment_date = db.tbl_remuneration.payment_date,
                        money = db.tbl_remuneration.money,
                    }).
                    From(db.tbl_remuneration).
                        Join(db.tbl_staff, db.tbl_remuneration.staff_id == db.tbl_staff.id).
                    Where(3000 < db.tbl_remuneration.money && db.tbl_remuneration.money < 4000));


                info = query.Build(typeof(SqlConnection));

                watch.Stop();
                if (i != 0)
                {
                    times.Add(watch.Elapsed.TotalMilliseconds);
                }
                watch.Reset();
                
            }

            times = times.Skip(1).ToList();
            times.Select(e => e.ToString()).ToList().ForEach(e => Console.WriteLine(e));
            Console.WriteLine(times.Average().ToString());
            Console.ReadKey();
        }


    }

    class XXX<TDB>
    {
        public static Expression<Func<TDB, TResult>> Create<TResult>(Expression<Func<TDB, TResult>> expression)
        {
            return expression;
        }
    }
}

