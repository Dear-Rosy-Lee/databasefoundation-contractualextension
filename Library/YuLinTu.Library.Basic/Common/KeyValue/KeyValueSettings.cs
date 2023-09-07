using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable()]
    public class KeyValueSettings : List<KeyValueSections>
    {
        #region Properties

        public KeyValueSections this[string name]
        {
            get { return GetKeyValueSections(name); }
            set { SetKeyValueSections(name, value); }
        }

        #endregion

        #region Methods

        #region Methods - Private

        private KeyValueSections GetKeyValueSections(string name)
        {
            foreach (var sections in this)
                if (sections.Name == name)
                    return sections;

            KeyValueSections setting = new KeyValueSections() { Name = name };
            this.Add(setting);
            return setting;
        }

        private void SetKeyValueSections(string name, KeyValueSections sections)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].Name == name)
                {
                    this[i] = sections;
                    return;
                }

            this.Add(sections);
        }

        #endregion

        #endregion
    }
}
