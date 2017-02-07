using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace TimeTag
{
    public class outz_User
    {
        public string Lto { get; set; }
        public string Mid { get; set; }
        public string Pa { get; set; }
        public bool IsNewDb { get; set; }

        public outz_User()
        {
            
        }

        public outz_User(NameValueCollection nvc)
        {
            try
            {
                if (nvc != null && nvc.GetValues("lto") != null && nvc.GetValues("mid") != null && nvc.GetValues("pa") != null && nvc.GetValues("db") != null)
                {
                    Properties.Settings.Default.UserInfo.Clear();
                    Properties.Settings.Default.UserInfo.Add("lto:" + nvc.GetValues("lto").GetValue(0));
                    Properties.Settings.Default.UserInfo.Add("medid:" + nvc.GetValues("mid").GetValue(0));
                    Properties.Settings.Default.UserInfo.Add("pa:" + nvc.GetValues("pa").GetValue(0));
                    Properties.Settings.Default.UserInfo.Add("db:" + nvc.GetValues("db").GetValue(0));
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Fetching url data failed: " + ex.Message);
            }
            if (Properties.Settings.Default.UserInfo.Count == 4)
            {
                this.Lto = Properties.Settings.Default.UserInfo[0].Split(new[] { ':' })[1];
                this.Mid = Properties.Settings.Default.UserInfo[1].Split(new[] { ':' })[1];
                this.Pa = Properties.Settings.Default.UserInfo[2].Split(new[] { ':' })[1];
                this.IsNewDb = Properties.Settings.Default.UserInfo[3].Split(new[] { ':' })[1] == "new";
            }
        }

        public outz_User(string lto, string medid, string pa, bool isNewDb)
        {
            this.Lto = lto;
            this.Mid = medid;
            this.Pa = pa;
            this.IsNewDb = isNewDb;
        }

        public void SaveToFile()
        {
            HelperSetting.WriteUserInfo(Properties.Settings.Default.UserInfo, this);
        }
    }
}
