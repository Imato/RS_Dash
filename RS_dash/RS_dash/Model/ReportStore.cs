using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_dash.Model
{
    public class ReportStore
    {
        private static List<Report> reports = new List<Report>();

        private static ReportStore store = new ReportStore();

        private ReportStore() { }

        public static void addReport(Report r)
        {
            reports.Add(r);
        }

        public static ReportStore getStore()
        {
            return store;
        }

    }
}
