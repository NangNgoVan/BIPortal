using BIPortal.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIPortal.Models.BusinessModels
{
    public class BlockWelcomeLangModel : BlockLanguageModel
    {
        public string Lblwelcome { set; get; }

        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            Lblwelcome = Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + BlockName + ".Lblwelcome.#cdata-section");
        }
    }
}