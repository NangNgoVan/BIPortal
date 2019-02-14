using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIPortal.Models.UI
{
    public class BlockModel
    {

        public object DataModel { set; get; }

        public BlockLanguageModel LanguageModel { set; get; }

        public string BlockName { set; get; }

        public string BlockId { set; get; }

        public int DeptID { set; get; }
        public BlockModel()
        {

        }

        public BlockModel(string blockName)
        {
            BlockName = blockName;
            LanguageModel = new BlockLanguageModel();
            LanguageModel.BlockName = blockName;
            BlockId = BlockName + "_1";
        }

        public BlockModel(string blockName, Object languageObject)
        {
            BlockName = blockName;
            LanguageModel = new BlockLanguageModel();
            LanguageModel.BlockName = blockName;
            BlockId = BlockName + "_1";

            LanguageModel.SetLanguage(languageObject);
        }

        public BlockModel(string blockName, Object languageObjet, BlockLanguageModel languageModel)
        {
            BlockName = blockName;
            LanguageModel = languageModel;
            LanguageModel.BlockName = blockName;
            BlockId = BlockName + "_1";

            LanguageModel.SetLanguage(languageObjet);
        }
        public BlockModel(object data_model, string block_name, string block_id)
        {
            DataModel = data_model;

            BlockName = block_name;
            BlockId = block_id;
            LanguageModel = new BlockLanguageModel();
            LanguageModel.BlockName = block_name;
        }

        public BlockModel(object data_model, string block_name, string block_id, Object languageObjet)
        {
            DataModel = data_model;

            BlockName = block_name;
            BlockId = block_id;
            LanguageModel = new BlockLanguageModel();
            LanguageModel.BlockName = block_name;
            LanguageModel.SetLanguage(languageObjet);
        }
    }
}