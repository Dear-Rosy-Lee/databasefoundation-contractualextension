using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;

namespace YuLinTu.Component.StockRightShuShan.Helper
{
    public class UpdateDatabaseHelper
    {

        /// <summary>
        /// 添加确股合同表和确股权证表
        /// </summary>
        /// <param name="dbContext"></param>
        public void AddTable(IDbContext dbContext)
        {
            var createConcord = @"
CREATE TABLE QGCBJYQ_HT ( 
    ID TEXT              PRIMARY KEY
                                  NOT NULL,
    CBHTBM      TEXT NOT NULL,
    FBFBS TEXT              NOT NULL,
    FBFMC       TEXT NOT NULL,
    DYBM TEXT              NOT NULL,
    CBFS        TEXT NOT NULL,
    CBQXQ DATETIME,
    CBQXZ       DATETIME,
    SHDCBFS TEXT,
    SHDCBQXQ    DATETIME,
    SHDCBQXZ DATETIME,
    CBYT        TEXT NOT NULL,
    SHDCBYT TEXT,
    ELCBTZMJ    DECIMAL(13, 8),
    JYQX TEXT,
    FBFQDSJ     DATETIME,
    CBFQDSJ DATETIME,
    JZJGRQ      DATETIME,
    CBHTFJ TEXT,
    ZT          INTEGER,
    CBJYQZH TEXT,
    CBFBS       TEXT,
    CBFMC TEXT              NOT NULL,
    DECBFMC     TEXT,
    DECBFZZ TEXT,
    RJMJ        DECIMAL(13, 8),
    CBFZJHM TEXT,
    CBFFRDBXM   TEXT,
    CBFFRDBZJLX TEXT,
    CBFFRDBZJHM TEXT,
    CBFFRDBDHHM TEXT,
    DLRXM       TEXT,
    DLRZJLX TEXT,
    DLRZJHM     TEXT,
    DLRDHHM TEXT,
    CBFLX       TEXT,
    CJBZ BOOLEAN,
    ZLDMJ       DECIMAL(13, 8),
    CBJE DECIMAL(13, 8),
    QZSQSBZ TEXT,
    CJZ         TEXT,
    CJSJ DATETIME,
    ZHXGZ       TEXT,
    ZHXGSJ DATETIME,
    BZXX        TEXT,
    SCZMJ DECIMAL(13, 8),
    QQZMJ DECIMAL(13, 8),
    JDDZMJ DECIMAL(13, 8),
    HTSFKY BOOLEAN,
    YLA         TEXT,
    YLB TEXT,
    YLC         TEXT,
    GSJS TEXT,
    GSJSR       TEXT,
    GSRQ DATETIME,
    GSJGYJ      TEXT,
    CBFDB TEXT,
    GSJGRQ      DATETIME,
    GSSHYJ TEXT,
    GSSHR       TEXT,
    GSSHRQ DATETIME
); ";
            var createWarrant = @"
CREATE TABLE QGCBJYQ_QZ ( 
    ID        TEXT     PRIMARY KEY
                         NOT NULL,
    QZNH      TEXT,
    QZLSH     TEXT,
    QZDBR     TEXT,
    QZDJRQ    DATETIME,
    QZFJ      TEXT,
    QZBM      TEXT,
    CBJYQZBM  TEXT,
    FZJG      TEXT,
    FZRQ      DATETIME,
    TZJG      TEXT,
    TZRQ      DATETIME,
    DZRQ      DATETIME,
    DZCS      INTEGER,
    CJZ       TEXT,
    CJSJ      DATETIME,
    XGZ       TEXT,
    XGSJ      DATETIME,
    DYDM      TEXT,
    BZ        TEXT,
    ZT        INTEGER,
    QZSFLY    TEXT,
    QZLQRQ    DATETIME,
    QZLQRXM   TEXT,
    QZLQRZJLX TEXT,
    QZLQRZJHM TEXT
);";
            CreateTable(dbContext, "QGCBJYQ_HT", createConcord);
            CreateTable(dbContext, "QGCBJYQ_QZ", createWarrant);

        }


        private void CreateTable(IDbContext dbContext,string tableName,string createTableSql)
        {
            var elements = dbContext.DataSource.CreateSchema().GetElements();
            if (elements.Any(c => !string.IsNullOrEmpty(c.TableName) && c.TableName == tableName))
                return;
            var query = dbContext.CreateQuery();
            query.CommandContext.CommandText.Append(createTableSql);
            query.Execute();
            query.CommandContext.CommandText.Clear();
        }




    }
}
