using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    public class ADGroupCollection
    {
        public List<ADGroup> AllItem = new List<ADGroup>();
        public void Add(ADGroup group)
        {
            this.AllItem.Add(group);
        }

        //public List<ADGroup> OrderBy()
        //{
        //    var a = from item in AllItem
        //            orderby item.DisplayName
        //            select item;

        //    return a.ToList();
        //}
    }
}
