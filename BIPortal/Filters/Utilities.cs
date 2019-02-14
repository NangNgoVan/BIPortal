using System;
using System.Configuration;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;

namespace BIPortal.Filters
{
    public class Utilities
    {
        /// <summary>
        /// Embed Tableau
        /// </summary>
        private static string _tableauUrl = "";
        private static string _tableauUser = "";
        private static string _clientIp = "";
        private static string _domainLDAP = WebConfigurationManager.AppSettings["LDAP_CONNECTION"];
        private static string _portLDAP = WebConfigurationManager.AppSettings["LDAP_PORT"];

        // Embed Tableau
        static public string GetTicket(string site)
        {

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string str;
            try
            {
                //var request = (HttpWebRequest)WebRequest.Create("http://10.0.131.53/" + "trusted");

                if (_tableauUrl == "")
                {
                    _tableauUrl = ConfigurationManager.AppSettings["tableauUrl"];
                    if (_tableauUrl[_tableauUrl.Length - 1] != '/') _tableauUrl += "/";
                }

                if (_tableauUser == "") _tableauUser = ConfigurationManager.AppSettings["tableauUser"];
                if (_clientIp == "") _clientIp = ConfigurationManager.AppSettings["clientIp"];
                //Bo sung them truong target_site

                var request = (HttpWebRequest)WebRequest.Create(_tableauUrl + "trusted");
                var encoding = new UTF8Encoding();
                var postData = "username=" + _tableauUser;
                //postData += "&client_ip=" + _clientIp;
                postData += "&target_site=" + site;
                byte[] data = encoding.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                str = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                str = ex.Message;
            }
            return str;
        }

        // Check UserLdap
        static public string IsAuthenticated(string username, string password)
        {
            string sRetval = "";
            Char charRange = '@';
            string userLdap = "";
            string _dc = "";

            int startIndex = username.IndexOf(charRange);
            int endIndex = username.Length;
            int length = endIndex - startIndex;


            if (startIndex > 0)
            {
                //UserName Ldap
                userLdap = username.Substring(0, startIndex).Trim();

                //Domain dc =sungroup,dc =com,dc=vn
                string domain = username.Substring(startIndex, length).Trim();
                _dc = domain.Replace("@", "dc=").Replace(".", ",dc=");
            }
            else
            {
                //UserName not Ldap
                userLdap = username.Trim();
            }

            try
            {
                // Create the new LDAP connection
                LdapDirectoryIdentifier ldi = new LdapDirectoryIdentifier(_domainLDAP, Int32.Parse(_portLDAP));

                LdapConnection ldapConnection = new LdapConnection(ldi);
                ldapConnection.AuthType = AuthType.Basic;
                ldapConnection.SessionOptions.ProtocolVersion = 3;
                //NetworkCredential nc = new NetworkCredential(username, password,_domainLDAP); //correct password


                NetworkCredential nc = new NetworkCredential(string.Format("uid={0},ou=people," + _dc, userLdap), password);


                ldapConnection.Bind(nc);
                Console.WriteLine("LdapConnection authentication success");
                ldapConnection.Dispose();

            }
            catch (LdapException e)
            {
                Console.WriteLine("\r\nUnable to login:\r\n\t" + e.Message);
                sRetval = e.Message;

            }
            catch (Exception e)
            {
                Console.WriteLine("\r\nUnexpected exception occured:\r\n\t" + e.GetType() + ":" + e.Message);
                sRetval = e.Message;

            }

            return sRetval;

        }
    }
}