using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace YuLinTu.Library.Entity
{
    public enum eUserState
    {
        [EntityEnumName("key41001", IsLanguageName = true)]
        Offline = 0,
        [EntityEnumName("key41002", IsLanguageName = true)]
        Online = 1,
        [EntityEnumName("key41005", IsLanguageName = true)]
        Disable = 5,
    }
}
