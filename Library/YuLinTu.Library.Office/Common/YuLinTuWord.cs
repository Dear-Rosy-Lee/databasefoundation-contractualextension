// (C) 2024 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Words;
using Aspose.Words.Tables;
using Aspose.Words.Rendering;
using System.Drawing.Printing;
using Aspose.Words.Drawing;
using System.Collections;
using Aspose.Words.Fields;
using Aspose.Words.Saving;

namespace YuLinTu.Library.Office
{
    /// <summary>
    /// Word对象基类
    /// </summary>
    [Serializable]
    public abstract class WordBase : TemplateBase
    {
        #region Properties

        private string fileName;//文件名称

        private string savedFileName;   // 保存的文件路径

        public string SavedFileName
        {
            get { return savedFileName; }
        }

        private bool isUseQRState = false;

        /// <summary>
        /// 是否使用二维码状态
        /// </summary>
        public bool IsUseQRState
        {
            get { return isUseQRState; }
            set { isUseQRState = value; }
        }

        /// <summary>
        /// 空数据替换字符
        /// </summary>
        public string EmptyReplacement { get; set; }

        private Dictionary<string, string> qrDic = new Dictionary<string, string>();

        public Dictionary<string, string> QrDic
        {
            get { return qrDic; }
            set { qrDic = value; }
        }

        #endregion Properties

        #region ConstName

        public const string TEMPLATEFILE_NOT_OPEN = "未初始化模板";

        #endregion ConstName

        #region Fields

        public Document doc;//文档
        public DocumentBuilder builder;//构建器

        #endregion Fields

        #region Ctor

        public WordBase()
        {
            EmptyReplacement = "";
            base.TemplateType = TemplateType.Word;
        }

        ~WordBase()
        {
        }

        #endregion Ctor

        #region Methods

        #region Methods - Public

        /// <summary>
        /// 注销
        /// </summary>
        public static void Dispose()
        {
        }

        /// <summary>
        /// 获取文档构建器
        /// </summary>
        /// <returns></returns>
        public DocumentBuilder GetDocumentBuilder()
        {
            return builder;
        }

        /// <summary>
        /// 获取文档
        /// </summary>
        /// <returns></returns>
        public Document GetDocument()
        {
            return doc;
        }

        /// <summary>
        /// 打开模版文件
        /// </summary>
        /// <param name="filePath">文件名</param>
        /// <returns></returns>
        public bool OpenTemplate(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }
            this.fileName = fileName;
            doc = new Document(fileName);
            if (doc != null)
            {
                builder = new DocumentBuilder(doc);
            }
            return doc != null;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (doc == null)
            {
                return;
            }
            doc = null;
            builder = null;
            GC.Collect();
        }

        /// <summary>
        /// 打印数据
        /// </summary>
        /// <param name="data"></param>
        public void Print(object data)
        {
            if (doc == null)
            {
                throw new Exception(TEMPLATEFILE_NOT_OPEN);
            }
            bool success = OnSetParamValue(data);
            if (!success)
            {
                return;
            }
            doc.Print();
            Close();
        }

        /// <summary>
        /// 打印数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="close"></param>
        public void Print(object data, bool close)
        {
            if (doc == null)
            {
                throw new Exception(TEMPLATEFILE_NOT_OPEN);
            }
            bool success = OnSetParamValue(data);
            if (!success)
            {
                return;
            }
            doc.Print();
            if (close)
            {
                Close();
            }
        }

        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="data"></param>
        public void PrintPreview(object data, string filePath)
        {
            if (doc == null)
            {
                throw new Exception(TEMPLATEFILE_NOT_OPEN);
            }
            //陈泽林 20161010 通过传递路径保存到默认文件路径下
            //string filePath = WordOperator.InitalzieDefatultDirectory();
            //filePath += Guid.NewGuid() + ".dot";
            filePath = filePath + ".dot";
            bool success = OnSetParamValue(data);
            if (!success)
            {
                return;
            }

            var printView = ToolConfigurationSetting.GetSpecialAppSettingValue("YuLinTuDocumentPrintvieMode", "false").ToLower() == "true";
            if (!printView)
            {
                WordOperator.InitalzieDirectory(filePath);
                doc.Save(filePath);
                System.Diagnostics.Process.Start(filePath);
                return;
            }
            PrintDocument document = new AsposeWordsPrintDocument(doc) as PrintDocument;
            if (document == null)
            {
                WordOperator.InitalzieDirectory(filePath);
                doc.Save(filePath);
                System.Diagnostics.Process.Start(filePath);
                return;
            }
            DocumentPrinterEventArgs args = DocumentObjectPrinter.PrintViewDocument(document, DocumentPrinterType.PrintView);
            if (!args.Success)
            {
                WordOperator.InitalzieDirectory(filePath);
                doc.Save(filePath);
                System.Diagnostics.Process.Start(filePath);
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        public void Save()
        {
            if (doc == null)
            {
                throw new Exception(TEMPLATEFILE_NOT_OPEN);
            }
            fileName = WordOperator.InitalizeValideFileName(fileName);
            string extension = System.IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                fileName += ".doc";
            }
            else
            {
                fileName = extension == ".doc" ? fileName : fileName.Replace(extension, ".doc");
            }
            WordOperator.InitalzieDirectory(fileName);
            doc.Save(fileName, SaveFormat.Doc);
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="fileName">文件名称</param>
        public void SaveAs(object data, string fileName)
        {
            if (doc == null)
            {
                throw new Exception(TEMPLATEFILE_NOT_OPEN);
            }
            fileName = WordOperator.InitalizeValideFileName(fileName);
            string extension = System.IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                fileName += ".doc";
            }
            else
            {
                fileName = extension == ".doc" ? fileName : fileName.Replace(extension, ".doc");
            }

            this.savedFileName = fileName;

            bool success = OnSetParamValue(data);
            if (success)
            {
                WordOperator.InitalzieDirectory(fileName);
                doc.Save(fileName, SaveFormat.Doc);
                Close();
            }
        }
        public void SaveAsDocx(object data, string fileName)
        {
            if (doc == null)
            {
                throw new Exception(TEMPLATEFILE_NOT_OPEN);
            }
            fileName = WordOperator.InitalizeValideFileName(fileName);
            string extension = System.IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                fileName += ".docx";
            }
            else
            {
                fileName = extension == ".docx" ? fileName : fileName.Replace(extension, ".docx");
            }

            this.savedFileName = fileName;

            bool success = OnSetParamValue(data);
            if (success)
            {
                WordOperator.InitalzieDirectory(fileName);
                doc.Save(fileName, SaveFormat.Docx);
                Close();
            }
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="close">是否关闭</param>
        public void SaveAs(object data, string fileName, bool close)
        {
            if (doc == null)
            {
                throw new Exception(TEMPLATEFILE_NOT_OPEN);
            }
            fileName = WordOperator.InitalizeValideFileName(fileName);
            string extension = System.IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                fileName += ".doc";
            }
            else
            {
                fileName = extension == ".doc" ? fileName : fileName.Replace(extension, ".doc");
            }
            bool success = OnSetParamValue(data);
            if (success)
            {
                WordOperator.InitalzieDirectory(fileName);
                doc.Save(fileName, SaveFormat.Doc);
                if (close)
                {
                    Close();
                }
            }
        }

        /// <summary>
        /// 另存为Xps文档
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="fileName">文件名称</param>
        public void SaveAsXps(object data, string fileName)
        {
            if (doc == null)
            {
                throw new Exception(TEMPLATEFILE_NOT_OPEN);
            }
            fileName = WordOperator.InitalizeValideFileName(fileName);
            string extension = System.IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                fileName += ".xps";
            }
            else
            {
                fileName = extension == ".xps" ? fileName : fileName.Replace(extension, ".xps");
            }
            bool success = OnSetParamValue(data);
            if (success)
            {
                WordOperator.InitalzieDirectory(fileName);
                doc.Save(fileName);
                Close();
            }
        }

        /// <summary>
        /// 另存为Xps文档
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="fileName">文件名称</param>
        public void SaveAsJpeg(object data, string fileName)
        {
            if (doc == null)
            {
                throw new Exception(TEMPLATEFILE_NOT_OPEN);
            }
            fileName = WordOperator.InitalizeValideFileName(fileName);
            string extension = System.IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                fileName += ".jpg";
            }
            else
            {
                fileName = extension == ".jpg" ? fileName : fileName.Replace(extension, ".jpg");
            }
            bool success = OnSetParamValue(data);
            if (success)
            {
                WordOperator.InitalzieDirectory(fileName);
                ImageSaveOptions options = new ImageSaveOptions(SaveFormat.Jpeg);
                options.Resolution = 300;
                doc.Save(fileName, options);
                Close();
            }
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="close">是否关闭</param>
        public void SaveAsMultiFile(object data, string fileName, bool isSavePcAsPDF = false, bool isSavePcAsJpg = true)
        {
            if (doc == null)
            {
                throw new Exception(TEMPLATEFILE_NOT_OPEN);
            }
            fileName = WordOperator.InitalizeValideFileName(fileName);
            string extension = System.IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                fileName += ".doc";
            }
            else
            {
                fileName = extension == ".doc" ? fileName : fileName.Replace(extension, ".doc");
            }
            bool success = OnSetParamValue(data);
            if (success)
            {
                WordOperator.InitalzieDirectory(fileName);
                if (isSavePcAsPDF)
                {
                    fileName = System.IO.Path.ChangeExtension(fileName, ".pdf");
                    doc.Save(fileName, SaveFormat.Pdf);
                }
                else
                {
                    if (isSavePcAsJpg)
                    {
                        ImageSaveOptions options = new ImageSaveOptions(SaveFormat.Jpeg);
                        options.Resolution = 300;
                        fileName = fileName.Replace(".doc", "");
                        int order = 1;
                        string nameFile = fileName + order.ToString() + ".jpg";
                        for (int i = 0; i < doc.PageCount; i++)
                        {
                            options.PageIndex = i;
                            doc.Save(nameFile, options);
                            order++;
                            nameFile = fileName + order.ToString() + ".jpg";
                        }
                    }
                    else
                    {
                        doc.Save(fileName, SaveFormat.Doc); 
                    }
                }
                Close();
            }
        }

        /// <summary>
        /// 清空书签值
        /// </summary>
        public void ClearBookmarkValue()
        {
            if (doc == null)
            {
                return;
            }
            for (int index = 1; index < doc.Range.Bookmarks.Count; index++)
            {
                doc.Range.Bookmarks[index].Text = "";
            }
        }

        #endregion Methods - Public

        #region Methods - Protected

        /// <summary>
        /// 设置书签值
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        protected void SetBookmarkValue(object paramName, string paramValue)
        {
            if (doc == null || paramName == null)
            {
                return;
            }
            try
            {
                if (IsUseQRState)
                {
                    if (qrDic.Count > 0 && qrDic.ContainsKey((string)paramName))
                    {
                        qrDic[(string)paramName] = (string)paramValue;
                    }
                }
                Bookmark bookmark = doc.Range.Bookmarks[paramName.ToString()];
                if (bookmark == null)
                {
                    return;
                }
                //2018.3.13 修改，空字符替换
                bookmark.Text = string.IsNullOrEmpty(paramValue) ? EmptyReplacement : paramValue;
                bookmark = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        protected void SetBookmarkValue(object paramName, int index, string paramValue)
        {
            SetBookmarkValue(paramName + (index == 0 ? "" : index.ToString()), paramValue);
        }

        protected bool IsHaveBookmark(object paramName)
        {
            if (doc == null || paramName == null)
            {
                return false;
            }
            try
            {
                Bookmark bookmark = doc.Range.Bookmarks[paramName.ToString()];
                if (bookmark == null)
                {
                    return false;
                }
                return true;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 设置控件值
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        protected void SetControlValue(object paramName, object paramValue, int controlType = 71)
        {
            if (doc == null || paramName == null || paramValue == null)
            {
                return;
            }
            try
            {
                FieldType fieldType = (FieldType)Enum.Parse(typeof(FieldType), controlType.ToString());
                foreach (FormField field in doc.Range.FormFields)
                {
                    if (field != null && field.Type == fieldType
                        && field.Name == paramName.ToString())
                    {
                        field.Checked = paramValue != null;
                        break;
                    }
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 设置表格单元值-插入到第二节表格，也从0序号开始
        /// </summary>
        /// <param name="tableIndex">表序号和段落无关，第几个表就是第几个</param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="value"></param>
        protected void SetTableCellValueDHDZ(int tableIndex, int rowIndex, int colIndex, string value)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                if (rowIndex >= table.Rows.Count)
                {
                    return;
                }
                Row row = table.Rows[rowIndex];
                Cell cell = row.Cells[colIndex];
                if (cell != null)
                {
                    NodeCollection secs = doc.GetChildNodes(NodeType.Section, true);
                    builder.MoveToSection(1);
                    builder.MoveToCell(tableIndex - 1, rowIndex, colIndex, 0);
                    builder.Write(value);
                }
                cell = null;
                row = null;
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 设置表格单元值
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="value"></param>
        protected string GetTableCellValue(int tableIndex, int rowIndex, int columnIndex)
        {
            if (doc == null)
            {
                return "";
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return "";
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return "";
            }
            try
            {
                if (rowIndex >= table.Rows.Count)
                {
                    return "";
                }
                Row row = table.Rows[rowIndex];
                if (columnIndex >= row.Cells.Count)
                {
                    return "";
                }
                Cell cell = row.Cells[columnIndex];
                row = null;
                table = null;
                tables = null;
                return cell.GetText();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return "";
        }

        /// <summary>
        /// 设置表格单元值
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="value"></param>
        protected void SetTableRowHeight(int tableIndex, int rowIndex, double height)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                if (rowIndex >= table.Rows.Count)
                {
                    return;
                }
                Row row = table.Rows[rowIndex];
                row.RowFormat.Height = height;
                row = null;
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 跳转节点设置表格单元值
        /// </summary>
        protected void SetTableCellValue(int section, int tableIndex, int rowIndex, int colIndex, string imagePath, double width, double height, bool setHeight = true)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            try
            {
                DocumentBuilder shapeBuilder = new DocumentBuilder(doc);
                shapeBuilder.MoveToSection(section);
                shapeBuilder.MoveToCell(tableIndex, rowIndex, colIndex, 0);
                Shape shape = new Shape(doc, ShapeType.Image);
                shape.ImageData.SetImage(imagePath);
                shape.Width = width;
                shape.Height = height;
                shape.HorizontalAlignment = HorizontalAlignment.Center;//水平对齐
                shape.VerticalAlignment = VerticalAlignment.Center;//垂直对齐
                shape.WrapSide = WrapSide.Both;
                shapeBuilder.CellFormat.TopPadding = 2;
                shapeBuilder.CellFormat.LeftPadding = 2;
                shapeBuilder.CellFormat.RightPadding = 2;
                shapeBuilder.CellFormat.BottomPadding = 2;
                shapeBuilder.InsertNode(shape);
                if (setHeight)
                {
                    shapeBuilder.RowFormat.Height = height + 2;
                }
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 设置表格单元值
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="value"></param>
        protected void SetTableCellValue(int tableIndex, int rowIndex, int colIndex, string value)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                if (rowIndex >= table.Rows.Count)
                {
                    return;
                }
                Row row = table.Rows[rowIndex];
                Cell cell = row.Cells[colIndex];
                if (cell != null)
                {
                    cell.RemoveAllChildren();
                    Paragraph para = new Paragraph(doc);
                    para.AppendChild(new Run(doc, string.IsNullOrEmpty(value) ? EmptyReplacement : value));
                    cell.AppendChild(para);
                }
                cell = null;
                row = null;
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 设置表格单元值
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="value"></param>
        protected void SetTableCellValue(int tableIndex, int rowIndex, int colIndex, string value, int textOrientation, int alignment)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                if (rowIndex >= table.Rows.Count)
                {
                    return;
                }
                Row row = table.Rows[rowIndex];
                Cell cell = row.Cells[colIndex];
                if (cell != null)
                {
                    builder.MoveToCell(tableIndex, rowIndex, colIndex, 0);
                    if (textOrientation > 0)
                    {
                        cell.CellFormat.Orientation = InitalizeTextOrientation(textOrientation);
                    }
                    if (alignment > 0)
                    {
                        cell.CellFormat.VerticalAlignment = InitalizeCellAlignment(alignment);
                    }
                    builder.Write(value);
                }
                cell = null;
                row = null;
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 设置文字方向
        /// </summary>
        /// <param name="orientation"></param>
        private TextOrientation InitalizeTextOrientation(int orientation)
        {
            TextOrientation textOrientation = TextOrientation.Horizontal;
            switch (orientation)
            {
                case 1:
                    textOrientation = TextOrientation.Downward;
                    break;

                case 2:
                    textOrientation = TextOrientation.Horizontal;
                    break;

                case 3:
                    textOrientation = TextOrientation.HorizontalRotatedFarEast;
                    break;

                case 4:
                    textOrientation = TextOrientation.Upward;
                    break;

                case 5:
                    textOrientation = TextOrientation.VerticalFarEast;
                    break;

                default:
                    break;
            }
            return textOrientation;
        }

        /// <summary>
        /// 初始化单元格对齐方式
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        private CellVerticalAlignment InitalizeCellAlignment(int alignment)
        {
            CellVerticalAlignment cellAlignment = CellVerticalAlignment.Center;
            switch (alignment)
            {
                case 1:
                    cellAlignment = CellVerticalAlignment.Top;
                    break;

                case 2:
                    cellAlignment = CellVerticalAlignment.Center;
                    break;

                case 3:
                    cellAlignment = CellVerticalAlignment.Bottom;
                    break;

                default:
                    break;
            }
            return cellAlignment;
        }

        /// <summary>
        /// 设置表格单元值
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="value"></param>
        protected void SetTableCellValue(int tableIndex, int rowIndex, int colIndex, string imagePath, double width, double height, bool setHeight = true)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                if (rowIndex >= table.Rows.Count)
                {
                    return;
                }
                Row row = table.Rows[rowIndex];
                Cell cell = row.Cells[colIndex];
                if (cell == null)
                {
                    return;
                }
                DocumentBuilder shapeBuilder = new DocumentBuilder(doc);
                shapeBuilder.MoveToCell(tableIndex, rowIndex, colIndex, 0);
                Shape shape = new Shape(doc, ShapeType.Image);
                shape.ImageData.SetImage(imagePath);
                shape.Width = width;
                shape.Height = height;
                shape.HorizontalAlignment = HorizontalAlignment.Center;//水平对齐
                shape.VerticalAlignment = VerticalAlignment.Center;//垂直对齐
                shape.WrapSide = WrapSide.Both;
                shapeBuilder.CellFormat.TopPadding = 2;
                shapeBuilder.CellFormat.LeftPadding = 2;
                shapeBuilder.CellFormat.RightPadding = 2;
                shapeBuilder.CellFormat.BottomPadding = 2;
                shapeBuilder.InsertNode(shape);
                if (setHeight)
                {
                    shapeBuilder.RowFormat.Height = height + 2;
                }
                cell = null;
                row = null;
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        protected void SetTableCellValue(int section, int tableIndex, int rowIndex, int colIndex, string imagePath, double width, double height, double rotation, bool setHeight = true)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            try
            {
                DocumentBuilder shapeBuilder = new DocumentBuilder(doc);
                shapeBuilder.MoveToSection(section);
                shapeBuilder.MoveToCell(tableIndex, rowIndex, colIndex, 0);
                Shape shape = new Shape(doc, ShapeType.Image);
                shape.ImageData.SetImage(imagePath);
                shape.Width = width;
                shape.Height = height;
                shape.HorizontalAlignment = HorizontalAlignment.Center;//水平对齐
                shape.VerticalAlignment = VerticalAlignment.Center;//垂直对齐
                shape.WrapSide = WrapSide.Both;
                shape.Rotation = rotation;
                shapeBuilder.CellFormat.TopPadding = 2;
                shapeBuilder.CellFormat.LeftPadding = 2;
                shapeBuilder.CellFormat.RightPadding = 2;
                shapeBuilder.CellFormat.BottomPadding = 2;
                shapeBuilder.InsertNode(shape);
                if (setHeight)
                {
                    shapeBuilder.RowFormat.Height = height + 2;
                }
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 调到节点设置表格单元值
        /// </summary>
        protected void SetTableCellValue(int section, int tableIndex, int rowIndex, int colIndex, string value)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            try
            {
                builder.MoveToSection(section);
                builder.MoveToCell(tableIndex, rowIndex, colIndex, 0);
                builder.Write(string.IsNullOrEmpty(value) ? EmptyReplacement : value);

                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 设置表格单元值
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="value"></param>
        protected void SetTableCellBordor(int tableIndex, int rowIndex, int columnIndex, int border)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                if (rowIndex >= table.Rows.Count)
                {
                    return;
                }
                Row row = table.Rows[rowIndex];
                if (columnIndex >= row.Cells.Count)
                {
                    return;
                }
                Cell cell = row.Cells[columnIndex];
                switch (border)
                {
                    case 1:
                        cell.CellFormat.Borders.Top.Shadow = true;
                        break;

                    case 2:
                        cell.CellFormat.Borders.Right.Shadow = true;
                        break;

                    case 3:
                        cell.CellFormat.Borders.Bottom.Shadow = true;
                        break;

                    case 4:
                        cell.CellFormat.Borders.Left.Shadow = true;
                        break;
                }
                row = null;
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 获取表格个数
        /// </summary>
        /// <returns></returns>
        protected int GetTableCount()
        {
            if (doc == null)
            {
                return 0;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0)
            {
                return 0;
            }
            else
                return tables.Count;
        }

        /// <summary>
        /// 复制最后一节
        /// </summary>
        protected void AddSection()
        {
            if (doc == null)
            {
                return;
            }

            try
            {
                Section section = doc.Sections[doc.Sections.Count - 1];
                Section newSection = (Section)doc.ImportNode(section, true);
                doc.Sections.Add(newSection);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 删除最后一节
        /// </summary>
        protected void DeleteSection()
        {
            if (doc == null)
            {
                return;
            }

            try
            {
                doc.Sections.RemoveAt(doc.Sections.Count - 1);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 添加重复表
        /// </summary>
        /// <param name="cloneTableIndex">复制的重复表序号</param>
        protected void AddTable(int cloneTableIndex, int addTableIndex)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || cloneTableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[cloneTableIndex] as Table;

            Table beforetable = tables[addTableIndex] as Table;
            if (table == null)
            {
                return;
            }

            Table targetTable = table.Clone(true) as Table;
            try
            {
                if (beforetable == null)
                {
                    table.ParentNode.InsertAfter(targetTable, table);
                    //table.ParentNode.InsertAfter(new Paragraph(doc), table);
                }
                else
                {
                    table.ParentNode.InsertAfter(targetTable, table);
                    //table.ParentNode.InsertAfter(new Paragraph(doc), table);
                }

                beforetable = null;
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 添加重复表
        /// </summary>
        /// <param name="cloneTableIndex">复制的重复表序号</param>
        /// <param name="cloneTableIndex">表后添加几个空格</param>
        /// <param name="cloneTableIndex">是否在表后添加空格</param>
        protected void AddTable(int cloneTableIndex, int addPara = 0, bool isAddPara = false)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || cloneTableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[cloneTableIndex] as Table;
            if (table == null)
            {
                return;
            }
            Table targetTable = table.Clone(true) as Table;
            try
            {
                table.ParentNode.InsertAfter(targetTable, table);
                if (isAddPara)
                {
                    for (int i = 0; i < addPara; i++)
                    {
                        table.ParentNode.InsertAfter(new Paragraph(doc), table);
                    }
                }

                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 删除最后一个段落[删除第三页后使用-地块示意图四川规范插件用]
        /// </summary>
        protected void DeleteParagraph()
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Paragraph, true);
            if (tables == null || tables.Count == 0)
            {
                return;
            }
            Paragraph paragraph = tables[tables.Count - 1] as Paragraph;

            if (paragraph == null)
            {
                return;
            }

            try
            {
                doc.ChildNodes.Remove(paragraph);
                paragraph = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 删除最后一个表格
        /// </summary>
        protected void DeleteTable()
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0)
            {
                return;
            }
            Table table = tables[tables.Count - 1] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                doc.ChildNodes.Remove(table);
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 删除倒数第n个表格
        /// </summary>
        /// <param name="index"></param>
        protected void DeleteTableReverse(int index)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0)
            {
                return;
            }
            if (index > tables.Count - 1) return;
            Table table = tables[tables.Count - 1 - index] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                doc.ChildNodes.Remove(table);
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 删除表格
        /// </summary>
        protected void DeleteTable(int tableIndex)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                doc.ChildNodes.Remove(table);
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 删除某一节
        /// </summary>
        /// <param name="index"></param>
        protected void DeleteSection(int index)
        {
            if (doc == null)
            {
                return;
            }

            if (index < 0 || index > doc.Sections.Count - 1)
            {
                return;
            }

            try
            {
                doc.Sections.RemoveAt(index);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 删除指定表格的指定行
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="rowIndex"></param>
        protected void DeleteRow(int tableIndex, int rowIndex)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                table.Rows.RemoveAt(rowIndex);
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 向指定的表中插入单元格
        /// </summary>
        /// <param name="tableIndex"></param>
        protected void InsertTableCell(int tableIndex)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                table.Rows.Add(table.LastRow.Clone(true));
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 向指定的表中插入单元格
        /// </summary>
        /// <param name="tableIndex">表号</param>
        /// <param name="rowCount">插入几行数据</param>
        protected void InsertTableCell(int tableIndex, int rowCount)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                for (int index = 0; index < rowCount; index++)
                {
                    Node node = table.LastRow.Clone(true);
                    table.Rows.Add(node);
                    node = null;
                }
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 向指定的表中插入单元格
        /// </summary>
        /// <param name="tableIndex">表号</param>
        /// <param name="tableIndex">开始插入行</param>
        /// <param name="rowCount">插入几行数据</param>
        protected void InsertTableRow(int tableIndex, int startRow, int rowCount)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            if (startRow >= table.Rows.Count)
            {
                return;
            }
            try
            {
                for (int index = 0; index < rowCount; index++)
                {
                    Node node = table.Rows[startRow].Clone(true);
                    table.Rows.Insert(startRow, node);
                    node = null;
                }
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 向指定的表中插入单元格
        /// </summary>
        /// <param name="tableIndex">表号</param>
        /// <param name="tableIndex">开始插入行</param>
        /// <param name="rowCount">插入几行数据</param>
        protected void InsertTableRowClone(int tableIndex, int startRow, int rowCount = 0)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            if (startRow >= table.Rows.Count)
            {
                return;
            }
            try
            {
                Node node = null;
                if (rowCount == 0)
                {
                    node = table.Rows[startRow].Clone(true);
                    table.Rows.Add(node);
                }
                else
                {
                    for (int index = 0; index < rowCount; index++)
                    {
                        node = table.Rows[startRow].Clone(true);
                        table.Rows.Insert(table.Rows.Count, node);
                    }
                }
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 向指定的表中复制对应行数的单元格并追加到表尾，如拷贝1-4行，拷贝几次(多行复制会复制格式)
        /// </summary>
        /// <param name="tableIndex">表号</param>
        /// <param name="startRow">开始插入行</param>
        ///  <param name="endRow">结束插入行</param>
        /// <param name="Count">复制几次</param>
        protected void InsertTableRowCloneByCount(int tableIndex, int startRow, int endRow, int Count = 1)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            if (startRow >= table.Rows.Count)
            {
                return;
            }
            endRow++;//因为插入是插入到当前的前一行
            var addrowcount = endRow - startRow;//复制的行
            try
            {
                Node node = null;
                int addindex = endRow;
                for (int i = 0; i < Count; i++)
                {
                    for (int index = startRow; index < endRow; index++)
                    {
                        node = table.Rows[index].Clone(true);
                        table.Rows.Insert(addindex, node);
                        addindex++;
                    }
                }
                node = null;
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 向指定的表中添加单元格
        /// </summary>
        /// <param name="tableIndex">表号</param>
        /// <param name="tableIndex">开始添加行</param>
        /// <param name="rowCount">插入几行数据</param>
        protected void AddTableRow(int tableIndex, int startRow, int rowCount)
        {
            if (doc == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            if (startRow >= table.Rows.Count)
            {
                return;
            }
            try
            {
                for (int index = 0; index < rowCount; index++)
                {
                    Node node = table.Rows[startRow].Clone(false);
                    table.Rows.Add(node);
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="imagePath"></param>
        protected void InsertImageCell(object paramName, string imagePath, double width, double height, bool setHeight = true)
        {
            if (paramName == null || string.IsNullOrEmpty(imagePath))
            {
                return;
            }
            Bookmark bookmark = doc.Range.Bookmarks[paramName.ToString()];
            if (bookmark == null)
            {
                return;
            }
            if (doc == null || builder == null)
            {
                return;
            }
            Shape shape = new Shape(doc, ShapeType.Image);
            shape.ImageData.SetImage(imagePath);
            shape.Width = width;
            shape.Height = height;
            shape.HorizontalAlignment = HorizontalAlignment.Center;//水平对齐
            shape.VerticalAlignment = VerticalAlignment.Center;//垂直对齐
            builder.MoveToBookmark(paramName.ToString());
            builder.CellFormat.TopPadding = 2;
            builder.CellFormat.LeftPadding = 2;
            builder.CellFormat.RightPadding = 2;
            builder.CellFormat.BottomPadding = 2;
            if (setHeight)
            {
                builder.RowFormat.Height = height;
            }
            builder.InsertNode(shape);
            shape = null;
            bookmark = null;
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="imagePath"></param>
        protected void InsertImageCellWithoutPading(object paramName, string imagePath, double width, double height)
        {
            if (paramName == null || string.IsNullOrEmpty(imagePath))
            {
                return;
            }
            Bookmark bookmark = doc.Range.Bookmarks[paramName.ToString()];
            if (bookmark == null)
            {
                return;
            }
            if (doc == null || builder == null)
            {
                return;
            }
            Shape shape = new Shape(doc, ShapeType.Image);
            shape.ImageData.SetImage(imagePath);
            shape.Width = width;
            shape.Height = height;
            shape.HorizontalAlignment = HorizontalAlignment.Center;//水平对齐
            shape.VerticalAlignment = VerticalAlignment.Center;//垂直对齐
            builder.MoveToBookmark(paramName.ToString());
            builder.RowFormat.Height = height;
            builder.InsertNode(shape);
            shape = null;
            bookmark = null;
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="imagePath"></param>
        protected void InsertImageCell(object paramName, string imagePath, double width, double height, int hAlignment, int vAlignment)
        {
            if (paramName == null || string.IsNullOrEmpty(imagePath))
            {
                return;
            }
            Bookmark bookmark = doc.Range.Bookmarks[paramName.ToString()];
            if (bookmark == null)
            {
                return;
            }
            if (doc == null || builder == null)
            {
                return;
            }
            Shape shape = new Shape(doc, ShapeType.Image);
            shape.ImageData.SetImage(imagePath);
            shape.Width = width;
            shape.Height = height;
            shape.HorizontalAlignment = (HorizontalAlignment)hAlignment;//水平对齐
            shape.VerticalAlignment = (VerticalAlignment)vAlignment;//垂直对齐
            builder.MoveToBookmark(paramName.ToString());
            builder.CellFormat.TopPadding = 2;
            builder.CellFormat.LeftPadding = 2;
            builder.CellFormat.RightPadding = 2;
            builder.CellFormat.BottomPadding = 2;
            builder.RowFormat.Height = height + 2;
            builder.InsertNode(shape);
            shape = null;
            bookmark = null;
        }

        /// <summary>
        /// 水平合并表中单元格
        /// </summary>
        protected void HorizontalMergeTable(int tableIndex, int rowIndex, int startColumnIndex, int endColumnIndex)
        {
            if (doc == null || builder == null)
            {
                return;
            }
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            try
            {
                if (rowIndex >= table.Rows.Count)
                {
                    return;
                }
                if (endColumnIndex <= startColumnIndex)
                {
                    return;
                }
                Row row = table.Rows[rowIndex];
                if (endColumnIndex > row.Cells.Count)
                {
                    return;
                }
                Cell cell = row.Cells[startColumnIndex];
                if (cell == null)
                {
                    return;
                }
                for (int index = startColumnIndex; index < endColumnIndex; index++)
                {
                    cell = row.Cells[index];
                    cell.CellFormat.HorizontalMerge = CellMerge.Previous;
                }
                cell = null;
                row = null;
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 垂直合并表中单元格
        /// </summary>
        protected void VerticalMergeTable(int tableIndex, int startRowIndex, int endRowIndex, int columnIndex)
        {
            if (doc == null || builder == null)
            {
                return;
            }
            //builder.CellFormat.VerticalMerge = CellMerge.Previous;
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }

            try
            {
                if (startRowIndex >= table.Rows.Count || endRowIndex >= table.Rows.Count)
                {
                    return;
                }
                if (endRowIndex <= startRowIndex)
                {
                    return;
                }
                //for (int index = startRowIndex; index <= endRowIndex; index++)
                //{
                //    Row row = table.Rows[index];
                //    if (columnIndex > row.Cells.Count)
                //    {
                //        continue;
                //    }
                //    Cell cell = row.Cells[columnIndex];
                //    if (cell == null)
                //    {
                //        continue;
                //    }
                //    builder.MoveToCell(0, index, columnIndex,0);
                //    //builder.MoveTo(cell);
                //    builder.CellFormat.VerticalMerge = CellMerge.Previous;

                //    //cell.CellFormat.VerticalMerge = CellMerge.Previous;
                //    cell = null;
                //    row = null;
                //}
                //builder.CellFormat.VerticalMerge = CellMerge.None;

                Row Startrow = table.Rows[startRowIndex];
                if (columnIndex > Startrow.Cells.Count)
                {
                    return;
                }
                Cell Startcell = Startrow.Cells[columnIndex];
                if (Startcell == null)
                {
                    return;
                }
                Row Endtrow = table.Rows[endRowIndex];

                Cell Endcell = Endtrow.Cells[columnIndex];
                if (Endcell == null)
                {
                    return;
                }
                MergeCells(Startcell, Endcell);

                table = null;
                tables = null;
                GC.Collect();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="data"></param>
        public void PrintPreview(object data)
        {
            try
            {
                if (doc == null)
                {
                    throw new Exception(TEMPLATEFILE_NOT_OPEN);
                }
                string filePath = WordOperator.InitalzieDefatultDirectory();
                filePath += Guid.NewGuid() + ".dot";
                bool success = OnSetParamValue(data);
                if (!success)
                {
                    return;
                }
                bool printView = ToolConfigurationSetting.GetSpecialAppSettingValue("YuLinTuDocumentPrintvieMode", "false").ToLower() == "true";
                if (!printView)
                {
                    WordOperator.InitalzieDirectory(filePath);
                    doc.Save(filePath);
                    System.Diagnostics.Process.Start(filePath);
                    return;
                }
                PrintDocument document = new AsposeWordsPrintDocument(doc) as PrintDocument;
                if (document == null)
                {
                    WordOperator.InitalzieDirectory(filePath);
                    doc.Save(filePath);
                    System.Diagnostics.Process.Start(filePath);
                    return;
                }
                DocumentPrinterEventArgs args = DocumentObjectPrinter.PrintViewDocument(document, DocumentPrinterType.PrintView);
                if (!args.Success)
                {
                    WordOperator.InitalzieDirectory(filePath);
                    doc.Save(filePath);
                    System.Diagnostics.Process.Start(filePath);
                }
            }
            finally
            {
                Close();
            }
        }

        protected static void MergeCells(Cell startCell, Cell endCell)
        {
            Table parentTable = startCell.ParentRow.ParentTable;

            // Find the row and cell indices for the start and end cell.
            System.Drawing.Point startCellPos = new System.Drawing.Point(startCell.ParentRow.IndexOf(startCell), parentTable.IndexOf(startCell.ParentRow));
            System.Drawing.Point endCellPos = new System.Drawing.Point(endCell.ParentRow.IndexOf(endCell), parentTable.IndexOf(endCell.ParentRow));
            // Create the range of cells to be merged based off these indices. Inverse each index if the end cell if before the start cell.
            System.Drawing.Rectangle mergeRange = new System.Drawing.Rectangle(Math.Min(startCellPos.X, endCellPos.X), Math.Min(startCellPos.Y, endCellPos.Y),
                Math.Abs(endCellPos.X - startCellPos.X) + 1, Math.Abs(endCellPos.Y - startCellPos.Y) + 1);

            foreach (Row row in parentTable.Rows)
            {
                foreach (Cell cell in row.Cells)
                {
                    System.Drawing.Point currentPos = new System.Drawing.Point(row.IndexOf(cell), parentTable.IndexOf(row));

                    // Check if the current cell is inside our merge range then merge it.
                    if (mergeRange.Contains(currentPos))
                    {
                        if (currentPos.X == mergeRange.X)
                            cell.CellFormat.HorizontalMerge = CellMerge.First;
                        else
                            cell.CellFormat.HorizontalMerge = CellMerge.Previous;

                        if (currentPos.Y == mergeRange.Y)
                            cell.CellFormat.VerticalMerge = CellMerge.First;
                        else
                            cell.CellFormat.VerticalMerge = CellMerge.Previous;
                    }
                }
            }
        }

        /// <summary>
        /// 垂直合并表中单元格
        /// </summary>
        //protected void VerticalMergeTable(int tableIndex, int startRowIndex, int endRowIndex, int columnIndex, CellMerge mergeSty = CellMerge.Previous)
        //{
        //    if (doc == null || builder == null)
        //    {
        //        return;
        //    }
        //    NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
        //    if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
        //    {
        //        return;
        //    }
        //    Table table = tables[tableIndex] as Table;
        //    if (table == null)
        //    {
        //        return;
        //    }
        //    try
        //    {
        //        if (startRowIndex >= table.Rows.Count || endRowIndex >= table.Rows.Count)
        //        {
        //            return;
        //        }
        //        if (endRowIndex <= startRowIndex)
        //        {
        //            return;
        //        }
        //        for (int index = startRowIndex; index <= endRowIndex; index++)
        //        {
        //            Row row = table.Rows[index];
        //            if (columnIndex > row.Cells.Count)
        //            {
        //                continue;
        //            }
        //            Cell cell = row.Cells[columnIndex];
        //            if (cell == null)
        //            {
        //                continue;
        //            }
        //            cell.CellFormat.VerticalMerge = mergeSty;
        //            cell = null;
        //            row = null;
        //        }
        //        table = null;
        //        tables = null;
        //        GC.Collect();
        //    }
        //    catch (SystemException ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex.ToString());
        //    }
        //}

        /// <summary>
        /// 垂直合并表中单元格样式
        /// </summary>
        //protected void VerticalMergeTableStyle(int tableIndex, int startRowIndex, int columnIndex, CellMerge mergeSty = CellMerge.None)
        //{
        //    if (doc == null || builder == null)
        //    {
        //        return;
        //    }
        //    NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
        //    if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
        //    {
        //        return;
        //    }
        //    Table table = tables[tableIndex] as Table;
        //    if (table == null)
        //    {
        //        return;
        //    }
        //    try
        //    {
        //        if (startRowIndex >= table.Rows.Count)
        //        {
        //            return;
        //        }

        //        Row row = table.Rows[startRowIndex];
        //        Cell cell = row.Cells[columnIndex];
        //        cell.CellFormat.VerticalMerge = mergeSty;
        //        cell = null;
        //        row = null;

        //        table = null;
        //        tables = null;
        //        GC.Collect();
        //    }
        //    catch (SystemException ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex.ToString());
        //    }
        //}

        #endregion Methods - Protected

        #region Methods - Override

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected virtual bool OnSetParamValue(object data)
        {
            return true;
        }

        #endregion Methods - Override

        #endregion Methods
    }
}