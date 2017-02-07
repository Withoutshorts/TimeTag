using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTag
{
    public class outz_Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.Windows.Visibility DescriptionVisibility
        {
            get
            {
                // Should only be shown for Gladsaxe users
                return (!RequireAllFields() || string.IsNullOrEmpty(Description)) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }
        }
        public int JobId { get; set; }
        public int ProjectGroup1 { get; set; }
        public int ProjectGroup2 { get; set; }
        public int ProjectGroup3 { get; set; }
        public int ProjectGroup4 { get; set; }
        public int ProjectGroup5 { get; set; }
        public int ProjectGroup6 { get; set; }
        public int ProjectGroup7 { get; set; }
        public int ProjectGroup8 { get; set; }
        public int ProjectGroup9 { get; set; }
        public int ProjectGroup10 { get; set; }

        public List<int> ListProjectGroups 
        {
            get 
            {
                _lstProjectGroups = new List<int>();
                _lstProjectGroups.Add(ProjectGroup1);
                _lstProjectGroups.Add(ProjectGroup2);
                _lstProjectGroups.Add(ProjectGroup3);
                _lstProjectGroups.Add(ProjectGroup4);
                _lstProjectGroups.Add(ProjectGroup5);
                _lstProjectGroups.Add(ProjectGroup6);
                _lstProjectGroups.Add(ProjectGroup7);
                _lstProjectGroups.Add(ProjectGroup8);
                _lstProjectGroups.Add(ProjectGroup9);
                _lstProjectGroups.Add(ProjectGroup10);

                return _lstProjectGroups;
            } 
        }

        public List<outz_Activity> ListAllActivities { get; set; }

        private List<int> _lstProjectGroups;

        public outz_Activity()
        {
        }

        /// <summary>
        /// Get all the activity items for auto compelete box based on the selected customer or job
        /// </summary>
        /// <param name="onlyActiveJobList"></param>
        public void GetAllNames(bool positive, string medid, string jobid, string lto, bool isNewDb)
        {
            ActivityData activity = new ActivityData(lto, isNewDb);
            int intMid = 0;
            int intJobid = 0;
            try
            {
                intMid = int.Parse(medid);
                intJobid = int.Parse(jobid);
            }
            catch
            {
                throw new Exception("Get all names for activity positive exception: Medid or Jobid is not integer");
            }



            if (positive)
            {              
                this.ListAllActivities = activity.SelectAllActivitysOnlyPositive("", 0, 50, intMid, intJobid);
            }
            else
            {
                outz_ProGroupRelations pro = new outz_ProGroupRelations();
                List<outz_ProGroupRelations> lstProGroups = pro.GetAll(intMid, lto, isNewDb); // KAN IKKE FINDE LTO

                List<outz_Activity> lstAllAct = activity.SelectAllActivitysNotPositive("", 0, 50, intMid, intJobid);

                foreach (outz_Activity act in lstAllAct)
                {
                    foreach (int group in act.ListProjectGroups)
                    {
                        if (lstProGroups.Exists(x => x.ProjectGroupId == group))
                        {
                            if(this.ListAllActivities == null)
                                this.ListAllActivities = new List<outz_Activity> ();
                            this.ListAllActivities.Add(act);
                            break;
                        }
                    }                 
                }              
            }
        }

        /// <summary>
        /// Overwrite ToString to show hierarchecal. Customer as title with indent job list below
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Define match strategy for each job customer object
        /// </summary>
        /// <param name="search"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SearchActivity(string search, object value)
        {
            outz_Activity act = (outz_Activity)value;
            if (act != null)
            {
                //Look for a match in jobs and customers
                if (act.ToString().ToLower().Contains(search.ToLower()))
                    return true;
            }

            return false;
        }

        // <summary>
        /// Get activity id no by job name or customer name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>"0" for not found job</returns>
        public static string GetId(string name, List<outz_Activity> listNames)
        {
            string id = "0";


            try
            {
                foreach (outz_Activity ac in listNames)
                {
                    if (ac.Name != null && ac.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        id = ac.Id.ToString();
                        break;
                    }
                }
            }
            catch
            {
                throw;
            }

            return id;
        }

        private static bool RequireAllFields()
        {
            string[] customers = HelperSetting.ReadLines(Properties.Settings.Default.UserInfo, 3);
            string[] requireAllFields = HelperSetting.ReadLines(Properties.Settings.Default.RequireAllFields, 10);
            var currentCustomer = customers[0].Split(':').Skip(1).ToArray()[0];
            return string.IsNullOrEmpty(currentCustomer) || !requireAllFields.Any(s => currentCustomer.Equals(s, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
