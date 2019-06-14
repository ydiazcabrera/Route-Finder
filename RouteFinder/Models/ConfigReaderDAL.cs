using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RouteFinder
{
    public class ConfigReaderDAL
    {
        /// <summary>
        /// This method gets keys out of the Web.Secrets.Config. You will pass in that key name as a parameter. 
        /// It will return the value of the key.
        /// </summary>
        /// <param name="key">String key name in Web.Secerts.Config</param>
        /// <returns>stirng with key value</returns>
        public static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings; //Gets setting from Web.config file
                return appSettings[key] ?? null; //return the value of the key passed in if that fails return null;
            }
            catch (ConfigurationErrorsException)
            {
                return null;//If it gets an exception cannont find file catch the exception and return a null
            }
        }
    }
}