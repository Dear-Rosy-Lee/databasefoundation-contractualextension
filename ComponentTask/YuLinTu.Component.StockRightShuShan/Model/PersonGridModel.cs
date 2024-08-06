using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightShuShan.Model
{

    /// <summary>
    /// 继承与承包方VirtualPerson实体供PersonGrid界面绑定使用
    /// </summary>
    public class PersonGridModel : NotifyCDObject
    {

        private VirtualPerson _virtualPerson;
        public VirtualPerson VirtualPerson
        {
            get { return _virtualPerson; }
            set { _virtualPerson = value; NotifyPropertyChanged(nameof(VirtualPerson)); }
        }

        public string Age
        {
            get
            {
                var person = VirtualPerson?.SharePersonList?.FirstOrDefault(o => o.ICN == VirtualPerson.Number);//拿到户主

                return person == null ? string.Empty : person.Age;
            }
        }


        public eGender Gender
        {
            get
            {
                var person = VirtualPerson?.SharePersonList?.FirstOrDefault(o => o.ICN == VirtualPerson.Number);//拿到户主
                if (person == null)
                    return eGender.Unknow;
                return person.Gender;
            }
        }



    }
}
