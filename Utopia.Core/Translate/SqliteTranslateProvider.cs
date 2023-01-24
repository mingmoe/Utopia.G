//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace Utopia.Core.Translate
{
    /// <summary>
    /// Sql翻译提供者。
    /// SQL数据库的结构要求如：
    /// 表名为TranslateIdentifence.ToString()的可能输出。
    /// 表中有一列ID和一列TRANSLATE。
    /// ID是翻译条目，TRANSLATE是翻译后的字符串。
    /// </summary>
    public class SqliteTranslateProvider : ITranslateProvider
    {
        readonly SQLiteConnection _connect;
        readonly Func<TranslateIdentifence, Guuid, string> _provider = (language, id) =>
        {
            return string.Format("SELECT * FROM @LANGUAGE WHERE ID == @ID;");
        };

        public SqliteTranslateProvider(SQLiteConnection connect)
        {
            ArgumentNullException.ThrowIfNull(connect);
            this._connect = connect;
        }

        public SqliteTranslateProvider(string dbFile)
        {
            dbFile = Path.GetFullPath(dbFile);
            _connect = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbFile));
            _connect.Open();
        }


        public bool TryGetItem(TranslateIdentifence language, Guuid id, out string? result)
        {
            var cmdStr = _provider.Invoke(language, id);
            var cmd = new SQLiteCommand(cmdStr, _connect);

            cmd.Parameters.AddWithValue("LANGUAGE", language.ToString());
            cmd.Parameters.AddWithValue("ID", id.ToString());

            var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);

            if (!reader.HasRows)
            {
                result = null;
                return false;
            }

            result = reader.GetString("TRANSLATE");
            reader.Close();

            return true;
        }
    }
}
