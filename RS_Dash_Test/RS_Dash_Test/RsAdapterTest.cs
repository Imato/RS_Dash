using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RS_dash.Utils;
using RS_dash.Model;
using System.Collections.Generic;

namespace RS_Dash_Test
{
    [TestClass]
    public class RsAdapterTest
    {
        RsAdapter rsAdapter;

        [TestInitialize]
        public void initTest()
        {
            rsAdapter = RsAdapter.getAdapter();

        }

        [TestMethod]
        public void checkAdapter()
        {
            Assert.IsNotNull(rsAdapter);
        }

        [TestMethod]
        public void getReports()
        {
            rsAdapter = RsAdapter.getAdapter();

            List<Report> reports = rsAdapter.getReports("RD-010");
            if (reports != null)
            {
                foreach (var r in reports)
                {
                    Console.WriteLine(r.Path + " - " + r.Id);
                }
            }

            Assert.IsNull(reports);

        }


    }
}
