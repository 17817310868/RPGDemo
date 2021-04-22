namespace Net.Server
{
    using System;
    using System.Data;

    /// <summary>
    /// SQL Server数据库工具
    /// 只能在服务器项目进行启用, 把注释去掉即可
    /// </summary>
    public sealed class SQL
    {
/*
        private static SqlConnection sqlConn = new SqlConnection("Server=AUTOBVT-797ECMJ;DataBase=Jewellery;uid=sa;pwd=123");
        public static SqlConnection SqlConn
        {
            get
            {
                if (sqlConn.State != ConnectionState.Open)
                {
                    sqlConn = new SqlConnection("Server=AUTOBVT-797ECMJ;DataBase=Jewellery;uid=sa;pwd=123");
                    sqlConn.Open();
                }
                return sqlConn;
            }
        }

        /// <summary>
        /// 修改数据库列表的数据
        /// </summary>
        /// <param name="cmdText"></param>
        public static void Modify(string cmdText, Action<DataTable> call)
        {
            //创建sql
            using (SqlCommand sc = new SqlCommand(cmdText, SqlConn))
            {
                //定义数据结果集
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sc);
                sqlDataAdapter.Fill(ds);
                DataTable dt = ds.Tables[0];
                call(dt);
                // 将DataSet的修改提交至“数据库”
                SqlCommandBuilder mySqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
                sqlDataAdapter.Update(ds);
                ds.Dispose();
                sqlDataAdapter.Dispose();
                dt.Dispose();
                mySqlCommandBuilder.Dispose();
            }
        }

        /// <summary>
        /// 加载数据库
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="call"></param>
        public static void LoadData(string cmdText, Action<DataTable> call)
        {
            GetData(cmdText, call);
        }

        /// <summary>
        /// 获取数据库表中数据 - 不进行更新和修改
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="call"></param>
        public static void GetData(string cmdText, Action<DataTable> call)
        {
            //创建sql
            using (SqlCommand sc = new SqlCommand(cmdText, SqlConn))
            {
                //定义数据结果集
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sc);
                sqlDataAdapter.Fill(ds);
                DataTable dt = ds.Tables[0];
                call(dt);

                ds.Dispose();
                sqlDataAdapter.Dispose();
                dt.Dispose();
            }
        }
*/
    }
}
