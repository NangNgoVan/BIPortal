using BIPortal.Models.UI;

namespace BIPortal.Models.BusinessModels
{
    public class BlockLoginLangModel : BlockLanguageModel
    {
        public string LblHelpTitle { set; get; }

        public string LblUserName { set; get; }

        public string LblPassword { set; get; }

        public string BtnSubmit { set; get; }

        public string LblForgetPassword { set; get; }

        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            LblHelpTitle = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + BlockName + ".LblHelpTitle");
            LblUserName = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + BlockName + ".LblUserName");
            LblPassword = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + BlockName + ".LblPassword");
            BtnSubmit = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + BlockName + ".BtnSubmit");
            LblForgetPassword = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + BlockName + ".LblForgetPassword");
        }
    }
}