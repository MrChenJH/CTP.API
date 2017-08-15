using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace CTP.Util
{
    public class MmsqlHelper
    {

        /// <summary>
        /// 批量操作每批次记录数
        /// </summary>
        public static int BatchSize = 2000;

        /// <summary>
        /// 超时时间
        /// </summary>
        public static int CommandTimeOut = 600;

    
        public MmsqlHelper()
        {
            this.ConnectionString = Profile.mySqlCon;
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        #region 实例方法

        #region ExecuteNonQuery

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(string commandText, params MySqlParameter[] parms)
        {
            return ExecuteNonQuery(ConnectionString, CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteNonQuery(ConnectionString, commandType, commandText, parms);
        }

        #endregion ExecuteNonQuery

        #region ExecuteScalar

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public T ExecuteScalar<T>(string commandText, params MySqlParameter[] parms)
        {
            return ExecuteScalar<T>(ConnectionString, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(string commandText, params MySqlParameter[] parms)
        {
            return ExecuteScalar(ConnectionString, CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteScalar(ConnectionString, commandType, commandText, parms);
        }

        #endregion ExecuteScalar

        #region ExecuteDataReader

        /// <summary>
        /// 执行SQL语句,返回只读数据集
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回只读数据集</returns>
        public MySqlDataReader ExecuteDataReader(string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataReader(ConnectionString, CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回只读数据集
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回只读数据集</returns>
        private MySqlDataReader ExecuteDataReader(CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataReader(ConnectionString, commandType, commandText, parms);
        }
        #endregion

        #endregion 实例方法

        #region 静态方法

        private static void PrepareCommand(MySqlCommand command, MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, MySqlParameter[] parms)
        {
            if (connection.State != ConnectionState.Open) connection.Open();

            command.Connection = connection;
            command.CommandTimeout = CommandTimeOut;
            // 设置命令文本(存储过程名或SQL语句)
            command.CommandText = commandText;
            // 分配事务
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            // 设置命令类型.
            command.CommandType = commandType;
            if (parms != null && parms.Length > 0)
            {
                //预处理MySqlParameter参数数组，将为NULL的参数赋值为DBNull.Value;
                foreach (MySqlParameter parameter in parms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
                command.Parameters.AddRange(parms);
            }
        }

        #region ExecuteNonQuery

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, string commandText, params MySqlParameter[] parms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                return ExecuteNonQuery(connection, CommandType.Text, commandText, parms);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                return ExecuteNonQuery(connection, commandType, commandText, parms);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public static int ExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteNonQuery(connection, null, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public static int ExecuteNonQuery(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteNonQuery(transaction.Connection, transaction, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        private static int ExecuteNonQuery(MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            MySqlCommand command = new MySqlCommand();
            PrepareCommand(command, connection, transaction, commandType, commandText, parms);
            int retval = command.ExecuteNonQuery();
            command.Parameters.Clear();
            return retval;
        }

        #endregion ExecuteNonQuery

        #region ExecuteScalar

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public static T ExecuteScalar<T>(string connectionString, string commandText, params MySqlParameter[] parms)
        {
            object result = ExecuteScalar(connectionString, commandText, parms);
            if (result != null)
            {
                return (T)Convert.ChangeType(result, typeof(T)); ;
            }
            return default(T);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public static object ExecuteScalar(string connectionString, string commandText, params MySqlParameter[] parms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                return ExecuteScalar(connection, CommandType.Text, commandText, parms);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                return ExecuteScalar(connection, commandType, commandText, parms);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public static object ExecuteScalar(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteScalar(connection, null, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public static object ExecuteScalar(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteScalar(transaction.Connection, transaction, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        private static object ExecuteScalar(MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            MySqlCommand command = new MySqlCommand();
            PrepareCommand(command, connection, transaction, commandType, commandText, parms);
            object retval = command.ExecuteScalar();
            command.Parameters.Clear();
            return retval;
        }

        #endregion ExecuteScalar

        #region ExecuteDataReader

 


        /// <summary>
        /// 执行SQL语句,返回只读数据集
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回只读数据集</returns>
        private static MySqlDataReader ExecuteDataReader(string connectionString, string commandText, params MySqlParameter[] parms)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            return ExecuteDataReader(connection, null, CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回只读数据集
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回只读数据集</returns>
        private static MySqlDataReader ExecuteDataReader(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            return ExecuteDataReader(connection, null, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回只读数据集
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回只读数据集</returns>
        private static MySqlDataReader ExecuteDataReader(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataReader(connection, null, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回只读数据集
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回只读数据集</returns>
        private static MySqlDataReader ExecuteDataReader(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataReader(transaction.Connection, transaction, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回只读数据集
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回只读数据集</returns>
        private static MySqlDataReader ExecuteDataReader(MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            MySqlCommand command = new MySqlCommand();
            PrepareCommand(command, connection, transaction, commandType, commandText, parms);
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        #endregion
        #endregion 静态方法
    }
}
