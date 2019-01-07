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
                    this.Lto = Convert.ToString(nvc.GetValues("lto").GetValue(0));
                    this.Mid = Convert.ToString(nvc.GetValues("mid").GetValue(0));
                    this.Pa = Convert.ToString(nvc.GetValues("pa").GetValue(0));
                    this.IsNewDb = Convert.ToString(nvc.GetValues("db").GetValue(0)) == "new";
                }
                else
                {
                    string[] infos = HelperSetting.UserInfo;
                    if(infos != null && infos.Length == 4)
                    {
                        this.Lto = infos[0].Split(':').Skip(1).ToArray()[0];
                        this.Mid = infos[1].Split(':').Skip(1).ToArray()[0];
                        this.Pa = infos[2].Split(':').Skip(1).ToArray()[0];
                        this.IsNewDb = infos[3].Split(':').Skip(1).ToArray()[0] == "new";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fetching url data failed: " + ex.Message);
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
            HelperSetting.SaveUserInfo(HelperSetting.UserInfoPath, this.Lto, this.Mid, this.Pa, this.IsNewDb);
        }
    }
}
