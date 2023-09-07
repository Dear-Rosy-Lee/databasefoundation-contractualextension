using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace YuLinTu.Library.Basic
{
    public class TextSerializer
    {
        #region Properties

        public TextWriter Writer { get; set; }

        #endregion

        #region Fields

        private const string stringDivider = "------------------------------------------";

        #endregion

        #region Ctor

        public TextSerializer()
            : this(null)
        {
        }

        public TextSerializer(TextWriter writer)
        {
            Writer = writer;
        }

        #endregion

        #region Methods

        #region Methods - Public

        public void Serialize(object target)
        {
            if (Writer == null)
                throw new ArgumentNullException("writer");
            if (target == null)
                throw new ArgumentNullException("target");

            ReflectSerialize(target);
        }

        private void ReflectSerialize(object target)
        {
            Writer.WriteLine(target);
            Writer.WriteLine(stringDivider);

            ToolReflection.TraversalProperties(PropertyHandler, target);

            Writer.WriteLine();
        }

        #endregion

        #region Methods - Private

        private bool PropertyHandler(string propertyName, object propertyValue)
        {
            if (propertyValue == null ||
                propertyValue is string)
            {
                WriteValue(propertyName, propertyValue);
                return true;
            }

            if (propertyValue is IEnumerable)
            {
                IEnumerable list = propertyValue as IEnumerable;
                foreach (object obj in list)
                    ReflectSerialize(obj);

                return true;
            }

            if (propertyValue is ITextSerializable)
            {
                (propertyValue as ITextSerializable).Serialize(Writer);
                return true;
            }

            if (propertyValue.GetType().IsClass)
            {
                ReflectSerialize(propertyValue);
                return true;
            }

            WriteValue(propertyName, propertyValue);
            return true;
        }

        private void WriteValue(string name, object value)
        {
            string valString = value == null ? "" : value.ToString();

            Writer.WriteLine(string.Format("{0} : {1}", name, valString));
        }

        #endregion

        #endregion
    }
}
