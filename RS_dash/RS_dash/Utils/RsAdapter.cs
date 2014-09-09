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

        private static string server = Properties.Settings.Default.ReportServer;
        private static readonly ReportingService2010 rs = new ReportingService2010();
        private static readonly ReportExecutionService re = new ReportExecutionService();
        private static ReportStore rsStore = ReportStore.getStore();
        

        private static ICredentials credentials;

        private static RsAdapter adapter = new RsAdapter();

        // брать пользователя и пароль из настроек
        public static RsAdapter getAdapter()
        {
            credentials = CredentialCache.DefaultCredentials;
            setCredentials(credentials);
            setServerUrl(server);
            return adapter;
        }

        public static RsAdapter getAdapter(string domain, string user, string password)
        {
            credentials = new NetworkCredential(user, password, domain);
            setCredentials(credentials);
            setServerUrl(server);
            return adapter;
        }

        private static void setServerUrl(string server)
        {
            string protocol;
            if (Properties.Settings.Default.UseHttps)
                protocol = "https://";
            else
                protocol = "http://";

            rs.Url = protocol  + server + Properties.Settings.Default.ReportService;
            re.Url = protocol + server + Properties.Settings.Default.ExecurionService;

        }

        private static void setCredentials(ICredentials credentials)
        {
            rs.Credentials = credentials;
            re.Credentials = credentials;
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
