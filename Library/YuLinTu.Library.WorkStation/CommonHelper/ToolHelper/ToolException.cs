using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace YuLinTu.Library.WorkStation
{
    public class ToolException
    {
        #region Methdos - Exception

        public static bool IsDeadLockException(Exception ex)
        {
            SqlException sqlex = ex as SqlException;
            if (sqlex == null)
                return false;

            if (sqlex.Number != 1205)
                return false;

            return true;
        }

        public static string GetExceptionMessage(Exception ex)
        {
            while (ex != null)
            {
                if (ex.InnerException == null)
                    return ex.Message;

                ex = ex.InnerException;
            }

            return string.Empty;
        }

        #endregion
    }
}
