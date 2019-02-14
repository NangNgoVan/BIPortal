using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIPortal.Models.UI
{
    public enum MessageType { Success = 1, ServerError = -2, BusinessError = -1 }
    public class BlockLanguageModel
    {
        public string BlockTitle { get; set; }
        public string BlockName { get; set; }
        public string MessageSuccess { get; set; }
        public string MessageServerError { get; set; }
        public string MessageBusinessError { get; set; }

        public virtual void SetLanguage(string blockName, object languageObject)
        {
            BlockName = blockName;
            BlockTitle = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + BlockName + ".title");
            MessageSuccess = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "messages." + BlockName + ".success");
            MessageBusinessError = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "messages." + BlockName + ".error_business_1");
            MessageServerError = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "commons.messages.ServerError");
        }

        public virtual void SetLanguage(Object languageObject)
        {
            BlockTitle = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + BlockName + ".title");
            MessageSuccess = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "messages." + BlockName + ".success");
            MessageBusinessError = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "messages." + BlockName + ".error_business_1");
            MessageServerError = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "commons.messages.ServerError");
        }


        public string GetLangByPath(string path, Object languageObject)
        {
            string output = "";

            try
            {
                output = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, path);
            }
            catch (Exception ex)
            {

            }

            return output;
        }
        public string GetMessage(int t)
        {
            string output = "";
            switch (t)
            {
                case 1:
                    output = MessageSuccess;
                    break;
                case -2:
                    output = MessageServerError;
                    break;
                case -1:
                    output = MessageBusinessError;
                    break;
                default:
                    output = MessageSuccess;
                    break;
            }
            return output;
        }


        public static string GetElementLang(Object languageObject, string path)
        {
            string output = "";
            output = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, path);
            return output;
        }
    }
}