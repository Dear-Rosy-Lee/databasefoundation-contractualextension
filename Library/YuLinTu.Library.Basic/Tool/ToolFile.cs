using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace YuLinTu.Library.Basic
{
    public class ToolFile
    {
        #region Properties

        #region Properties - Parameter

        public bool ShowNewFolderButton { get; set; }
        public bool Multiselect { get; set; }
        public string InitialDirectory { get; set; }
        public string Title { get; set; }
        public string Filter { get; set; }
        public string FileName { get; set; }
        public string[] FileNames { get; set; }

        #endregion

        #region Properties - Filter

        public static string FilterAllFile { get { return LanguageAttribute.GetLanguage("flt29901"); } }
        public static string FilterExcelFile { get { return LanguageAttribute.GetLanguage("flt29902"); } }
        public static string FilterShapeFile { get { return LanguageAttribute.GetLanguage("flt29903"); } }
        public static string FilterDwgFile { get { return LanguageAttribute.GetLanguage("flt29904"); } }
        public static string FilterMpjFile { get { return LanguageAttribute.GetLanguage("flt29905"); } }
        public static string FilterXmlFile { get { return LanguageAttribute.GetLanguage("flt29906"); } }
        public static string FilterTextFile { get { return LanguageAttribute.GetLanguage("flt29907"); } }

        #endregion

        #endregion

        #region Ctor

        static ToolFile()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_ToolFile);
        }

        #endregion

        #region Methods - File Dialog

        public bool OpenFileAll()
        {
            Filter = FilterAllFile;
            return OpenFile();
        }

        public bool OpenFileExcel()
        {
            Filter = FilterExcelFile;
            return OpenFile();
        }

        public bool OpenFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = InitialDirectory;
            dlg.Multiselect = Multiselect;
            dlg.Title = Title;
            dlg.Filter = Filter;
            if (File.Exists(FileName))
                dlg.FileName = FileName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileName = dlg.FileName;
                FileNames = dlg.FileNames;
                return true;
            }

            return false;
        }

        public bool SaveFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = InitialDirectory;
            dlg.Title = Title;
            dlg.Filter = Filter;
            dlg.AddExtension = true;
            if (File.Exists(FileName))
                dlg.FileName = FileName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileName = dlg.FileName;
                return true;
            }

            return false;
        }

        public bool OpenFolder()
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = InitialDirectory;
            dlg.Description = Title;
            dlg.ShowNewFolderButton = ShowNewFolderButton;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileName = dlg.SelectedPath;
                Environment.CurrentDirectory = FileName;
                return true;
            }

            return false;
        }

        #endregion

        #region Methods - File Name

        public static string ReplaceExtensionFileName(string fileName, string extension)
        {
            string dir = Path.GetDirectoryName(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName);
            fileName = string.Format("{0}{1}", fileName, extension);

            return Path.Combine(dir, fileName);
        }

        public static string[] GetFiles(string path)
        {
            string[] fileNames = Directory.GetFiles(path);
            return fileNames;
        }

        public static string[] GetFiles(string path, string[] extension)
        {
            List<string> list = new List<string>();

            string[] fileNames = Directory.GetFiles(path);
            for (int i = 0; i < fileNames.Length; i++)
            {
                string fileName = fileNames[i];
                string ext = Path.GetExtension(fileName);
                if (extension.Contains(ext.ToLower()))
                    list.Add(fileName);
            }

            list.Sort();
            return list.ToArray();
        }

        #endregion

        #region Methods - Directory

        public static void CreateDirectory(string fileName)
        {
            string path = Path.GetDirectoryName(fileName);
            Directory.CreateDirectory(path);
        }

        public static void TraversalDirectoryContent(TraversalDirectoryDelegate method, string path)
        {
            if (!Directory.Exists(path))
                return;
            if (method == null)
                return;

            if (!method(path, eDirectoryContentType.Directory))
                return;

            innerTraversalDirectoryContent(method, path, false);
        }

        public static void TraversalDirectoryContent(TraversalDirectoryDelegate method, string path, bool isPriorityChild)
        {
            if (!Directory.Exists(path))
                return;
            if (method == null)
                return;

            if (!isPriorityChild && !method(path, eDirectoryContentType.Directory))
                return;

            innerTraversalDirectoryContent(method, path, isPriorityChild);

            if (isPriorityChild && !method(path, eDirectoryContentType.Directory))
                return;
        }

        private static bool innerTraversalDirectoryContent(TraversalDirectoryDelegate method, string path, bool isPriorityChild)
        {
            string[] fileNames = Directory.GetFiles(path);
            string[] folderNames = Directory.GetDirectories(path);

            foreach (string folder in folderNames)
            {
                if (!isPriorityChild && !method(folder, eDirectoryContentType.Directory))
                    return false;
                if (!innerTraversalDirectoryContent(method, folder, isPriorityChild))
                    return false;
                if (isPriorityChild && !method(folder, eDirectoryContentType.Directory))
                    return false;
            }

            foreach (string file in fileNames)
                if (!method(file, eDirectoryContentType.File))
                    return false;

            return true;
        }

        #endregion
    }
}
