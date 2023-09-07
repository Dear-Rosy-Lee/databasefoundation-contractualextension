using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public abstract class ConfigBase
    {
        #region Ctor

        static ConfigBase()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_Config);
        }

        #endregion

        #region Methods

        #region Methods - Override

        #endregion

        #region Methods - Serialize

        public bool Serialize(string fileName)
        {
            try
            {
                ToolFile.CreateDirectory(fileName);
                ToolSerialization.SerializeXml(fileName, this);
            }
            catch (Exception ex)
            {
                WriteError(20108, "ex20108", ex);
                return false;
            }

            return true;
        }

        public static T Deserialize<T>(string fileName) where T : ConfigBase
        {
            T obj = default(T);

            try
            {
                obj = (T)ToolSerialization.DeserializeXml(fileName, typeof(T));
            }
            catch (Exception ex)
            {
                WriteError(20109, "ex20109", ex);
                obj = (T)Activator.CreateInstance(typeof(T));
            }

            obj.Serialize(fileName);

            return obj;
        }

        #endregion

        #region Methods - Private

        private static void WriteError(int id, string idString, Exception ex)
        {
            string msg = LanguageAttribute.GetLanguage(idString);
            msg = string.Format(msg, ToolException.GetExceptionMessage(ex));
            Trace.WriteLine(new Log()
            {
                EventID = id,
                Grade = eLogGrade.Error,
                Description = msg,
                Source = ex.Source,
                TargetType = eOperationTargetType.File
            });
        }

        #endregion

        #endregion
    }
}
