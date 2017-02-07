using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTag
{
    public class outz_ProGroupRelations
    {
        public int ProjectGroupId { get; set; }
        public string MedId { get; set; }

        public outz_ProGroupRelations()
        {
        }

        public List<outz_ProGroupRelations> GetAll(int mid, string lto, bool isNewDb)
        {
            ProGroupRelationsData progroup = new ProGroupRelationsData(lto, isNewDb);
            return progroup.GetProGroupRelationsByMid(0, 1000, mid);
        }
    }
}
