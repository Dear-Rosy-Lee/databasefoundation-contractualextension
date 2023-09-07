using System.Collections.Generic;
using System.Linq;
using YuLinTu.Data;

namespace YuLinTu.Library.CompareResult
{
    public class ResultFolderContext : DataSource, IResultFolderContext
    {
        #region Properties

        public override string ProviderName { get { return DataSource.Name; } }
        public override string ConnectionString { get { return DataSource.ConnectionString; } }

        public IProviderResultFolder DataSource { get; private set; }

        public Dictionary<string, int> DKBMToFIDs { get; private set; }

        /// <summary>
        /// 每个shpfile对应的地块编码和FID，key是文件全名
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> ShpFileDKBMToFIDs { get; private set; }


        public ResultDataRead Reader
        {
            get { return DataSource.Reader; }
        }


        #endregion

        #region Ctor

        public ResultFolderContext(IProviderResultFolder provider)
            : base(provider.Name, provider.ConnectionString)
        {
            DataSource = provider;
        }

        public ResultFolderContext(string cntStringName)
            : this(Provider.Create(cntStringName) as IProviderResultFolder)
        {
        }

        public ResultFolderContext(string providerName, string cntString)
            : this(Provider.Create(providerName, cntString) as IProviderResultFolder)
        {
        }

        protected override void OnInitialize(string providerName, string cntString)
        {
        }

        #endregion

        #region Methods

        #region Methods - Override

        public override void Dispose()
        {
            base.Dispose();

            if (DKBMToFIDs != null)
            {
                DKBMToFIDs.Clear();
                DKBMToFIDs = null;
            }
        }

        public override object Clone()
        {
            var ds = DataSource.Clone() as IProviderResultFolder;
            var db = new ResultFolderContext(ds);

            return db;
        }

        public void InitializeDKBMToFIDsDictionary()
        {
            //var dkShape = Reader.CurrentPath.ShapeFileList.FirstOrDefault(c => c.Name == LandParcel.TableName);
            //var dic = GetFileDKBMToFIDsDictionary(dkShape);
            //DKBMToFIDs = dic;

            //if (Reader.CurrentPath.ShapeFileList.Count(c => c.Name == LandParcel.TableName) > 1)
            //{
            //    var dkShapes = Reader.CurrentPath.ShapeFileList.Where(c => c.Name == LandParcel.TableName).ToList();
            //    var dics = new Dictionary<string, Dictionary<string, int>>();
            //    foreach (var item in dkShapes)
            //    {
            //        var itemfullname = item.FullName;
            //        var itemdic = GetFileDKBMToFIDsDictionary(item);
            //        dics[itemfullname] = itemdic;
            //    }
            //    ShpFileDKBMToFIDs = dics;
            //}
        }

        private Dictionary<string, int> GetFileDKBMToFIDsDictionary(FileCondition dkShape)
        {
            //if (dkShape == null)
            //    throw new ElementNotFoundException(null, DK.TableName);

            //var ds = ProviderShapefile.CreateDataSourceByFileName(dkShape.FilePath, false);
            //var dq = new DynamicQuery(ds);

            //var schema = ds.CreateSchema();
            //var hasDKBM = schema.AnyElementProperty(null, dkShape.FullName, DK.CDKBM);
            //if (!hasDKBM)
            //    throw new KeyNotFoundException(DK.CDKBM);

            var dic = new Dictionary<string, int>();

            //dq.ForEach(null, dkShape.FullName, (i, cnt, obj) =>
            //{
            //    var temp = obj.GetPropertyValue<string>(DK.CDKBM);
            //    if (temp == null) return true;
            //    dic[temp] = i;
            //    return true;
            //}, PropertySection.Property(DK.CDKBM, DK.CDKBM));

            return dic;
        }

        #endregion

        #endregion
    }
}
