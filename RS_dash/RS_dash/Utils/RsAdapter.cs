using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RS_dash.ReportServer;
using RS_dash.ReportExecution;
using System.Net;
using RS_dash.Model;

namespace RS_dash.Utils
{
    public class RsAdapter
    {
        private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly ReportingService2010 rs = new ReportingService2010();
        private static readonly ReportExecutionService re = new ReportExecutionService();
        private static ReportStore rsStore = ReportStore.getStore();        

        private static RsAdapter adapter = new RsAdapter();

        // брать пользователя и пароль из настроек
        public static RsAdapter getAdapter()
        {            
            setCredentials();
            setServerUrl();
            return adapter;
        }

        private static void setServerUrl()
        {
            string server = Properties.Settings.Default.ReportServer;
            string protocol;
            if (Properties.Settings.Default.UseHttps)
                protocol = "https://";
            else
                protocol = "http://";

            rs.Url = protocol  + server + Properties.Settings.Default.ReportService;
            re.Url = protocol + server + Properties.Settings.Default.ExecurionService;

        }

        private static void setCredentials()
        {
            
            ICredentials credentials;

            if (Properties.Settings.Default.UseSSO)
                credentials = CredentialCache.DefaultCredentials;
            else
                credentials = new NetworkCredential(
                                            Properties.Settings.Default.User,
                                            Security.decrypt(Properties.Settings.Default.Password),
                                            Properties.Settings.Default.Domain);

            rs.Credentials = credentials;
            re.Credentials = credentials;
        }

        public List<Report> getReports()
        {
            log.Debug("Get all reports");

            // use local store or update cache
            if (rsStore.getReportsCount() == 0)
                updateReportsCache();

            return rsStore.getAllReports();
        }

        public void updateReportsCache()
        {
            log.Debug("Update local reports cache");

            CatalogItem[] items = null;

            try
            {
                items = rs.ListChildren("/", true);

            }

            catch(Exception ex)
            {
                log.Fatal("Error in updating local reports cache from server: {0}", ex.Message);
            }

            if (items != null)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    CatalogItem item = items[i];

                    if (!item.Hidden && item.TypeName == "Report")
                    {
                        Report r = new Report(item.Name, item.ID, item.Path, item.Description);
                        rsStore.addReport(r);
                    }
                    
                }

                rsStore.saveReports();
            }
           
        }


        public List<Report> getReports(string name)
        {
            log.Debug("Find report {0}", name);

            CatalogItem[] items = null;

            Property[] property = new Property[]{new Property(){Name = "Recursive", Value="True"}};

            SearchCondition condition0 = new SearchCondition();
            condition0.Condition = ConditionEnum.Contains;
            condition0.ConditionSpecified = true;
            condition0.Name = "Name";
            condition0.Values[0] = name;

            SearchCondition condition1 = new SearchCondition();
            condition1.Condition = ConditionEnum.Equals;
            condition1.ConditionSpecified = true;
            condition1.Name = "TypeName";
            condition1.Values[0] = "Report";

            SearchCondition[] conditions = new SearchCondition[2];
            conditions[0] = condition0;
            conditions[1] = condition1;

            List<Report> reports = new List<Report>();


            try
            {
                items = rs.FindItems("/", BooleanOperatorEnum.And, property, conditions);


                if (items != null)
                {
                    foreach (CatalogItem item in items)
                    {
                        if (item.Hidden == false)
                            reports.Add(new Report(item.Name, item.ID, item.Path, item.Description));
                    }
                }
            }

            catch (Exception ex)
            {
                log.Fatal("Error by finding report {0}: {1}", name, ex.Message);
            }

            return reports;

        }





    }
}
