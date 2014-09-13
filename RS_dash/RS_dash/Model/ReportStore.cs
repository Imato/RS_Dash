using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace RS_dash.Model
{
    public class ReportStore
    {
        private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static List<Report> reports = new List<Report>();
        private static List<Report> usedReports = new List<Report>();

        private static ReportStore store = new ReportStore();

        private static string fileReports;
        private static string fileUsedReports;

        private ReportStore() 
        {
            fileReports = Properties.Settings.Default.ReportList;
            fileUsedReports = Properties.Settings.Default.UsedReportList;
            loadReports();
        }

        public static ReportStore getStore()
        {
            return store;
        }

        public int getReportsCount(){
            return reports.Count;
        }

        public int getUsedReportsCount()
        {
            return usedReports.Count;
        }

        public List<Report> getAllReports()
        {
            return reports;
        }

        public List<Report> getAllUsedReports()
        {
            return usedReports;
        }

        public void addReport(Report report)
        {
            reports.Add(report);
            if (report.Used)
                usedReports.Add(report);
        }

        public void saveReports()        
        {
            TextWriter stream;
            XmlSerializer sr = new XmlSerializer(typeof(List<Report>));

            try
            {
                log.Debug("Save reports list to file");
                stream = new StreamWriter(fileReports);
                sr.Serialize(stream, reports);
                stream.Close();

                log.Debug("Save used reports list to file");
                stream = new StreamWriter(fileUsedReports);
                sr.Serialize(stream, usedReports);
                stream.Close();
            }

            catch (Exception ex)
            {
                log.Fatal("Error in saving reports to file: {0}", ex.Message);
            }
            
        }

        private void loadReports()
        {
            TextReader stream;
            XmlSerializer sr = new XmlSerializer(typeof(List<Report>));

            try
            {
                log.Debug("Load reports list from file");
                stream = new StreamReader(fileReports);
                reports = (List<Report>)sr.Deserialize(stream);
                stream.Close();

                log.Debug("Load used reports list from file");
                stream = new StreamReader(fileUsedReports);
                usedReports = (List<Report>)sr.Deserialize(stream);
                stream.Close();
            }

            catch (Exception ex)
            {
                log.Fatal("Error in getting reports from file: {0}", ex.Message);
            }

        }
    }
}
