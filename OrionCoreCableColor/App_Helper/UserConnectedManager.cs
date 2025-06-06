using OrionCoreCableColor.DbConnection.OrionDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace OrionCoreCableColor.App_Helper
{
    public static class UserConnectedManager
    {
        public static string GetVisitorIPAddress(bool GetLan = false)
        {
            string visitorIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (String.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
            {
                GetLan = true;
                visitorIPAddress = string.Empty;
            }

            if (GetLan)
            {
                if (string.IsNullOrEmpty(visitorIPAddress))
                {
                    //This is for Local(LAN) Connected ID Address
                    string stringHostName = Dns.GetHostName();
                    //Get Ip Host Entry
                    IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                    //Get Ip Address From The Ip Host Entry Address List
                    IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                    try
                    {
                        visitorIPAddress = arrIpAddress[arrIpAddress.Length - 2].ToString();
                    }
                    catch
                    {
                        try
                        {
                            visitorIPAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            try
                            {
                                arrIpAddress = Dns.GetHostAddresses(stringHostName);
                                visitorIPAddress = arrIpAddress[0].ToString();
                            }
                            catch
                            {
                                visitorIPAddress = "127.0.0.1";
                            }
                        }
                    }
                }
            }


            return visitorIPAddress;
        }


        //public static string GetIpWithSQL()
        //{
        //    using(var contexto = new ORIONDBEntities())
        //    {
        //        return contexto.sp_GetIpAdress().FirstOrDefault();
        //    }
        //}


        //public string getIPConnected()
        //{
        //    string ip = string.Empty;

            

        //    foreach (HostName localHostName in NetworkInformation.GetHostNames())
        //    {
        //        if (localHostName.IPInformation != null)
        //        {
        //            if (localHostName.Type == HostNameType.Ipv4)
        //            {
        //                ip = localHostName.ToString();
        //                break;
        //            }
        //        }
        //    }

        //    return ip;
        //}
    }
}