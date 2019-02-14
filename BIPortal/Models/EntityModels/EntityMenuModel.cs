namespace BIPortal.Models.EntityModels
{
    public class EntityMenuModel
    {
        private int _level;
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string PathTableau { get; set; }
        public string Site_Root { get; set; }
        public int ParentId { get; set; }
        public string MenuLevel { get; set; }
        public bool Status { get; set; }
        public int DeptID { get; set; }
        public bool Selected { get; set; }

        public string FilterCommand { set; get; }

        public string FilterValue { set; get; }
        public string LevelTree { set; get; }
        public int Priority { set; get; }

        public int Level
        {


            get
            {
                string[] separatingChars = { "@@@" };
                string[] lstLevel = (LevelTree + "@@@abc").Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

                _level = lstLevel.Length - 1;
                return _level;
            }
        }

        public bool HasChild { set; get; }
        public EntityMenuModel()
        {
            Name = "";
            MenuLevel = "";
            Status = false;
            FilterCommand = "";
            FilterValue = "";
            Path = "";
            PathTableau = "";
            Site_Root = "";
            Priority = 0;
        }

        public EntityMenuModel Clone()
        {
            EntityMenuModel output = new EntityMenuModel();
            output.FilterCommand = this.FilterCommand;
            output.FilterValue = this.FilterValue;
            output.LevelTree = this.LevelTree;
            output.MenuId = this.MenuId;
            output.MenuLevel = this.MenuLevel;
            output.Name = this.Name;
            output.ParentId = this.ParentId;
            output.Path = this.Path;
            output.Site_Root = Site_Root;
            output.PathTableau = PathTableau;
            output.Priority = this.Priority;
            output.Status = this.Status;
            return output;
        }
    }
}