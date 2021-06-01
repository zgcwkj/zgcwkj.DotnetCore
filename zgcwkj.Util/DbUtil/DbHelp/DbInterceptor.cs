using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using zgcwkj.Util.Extension;

namespace zgcwkj.Util.DbUtil.DbHelp
{
    /// <summary>
    /// SQL 执行拦截器
    /// </summary>
    public class DbInterceptor : DbCommandInterceptor
    {
        /// <summary>
        /// 输出 SQL 日志时间
        /// </summary>
        public const int DBSlowSqlLogTime = 10;

        /// <summary>
        /// 在 EF 打算调用 System.Data.Common.DbCommand.ExecuteNonQuery 之前调用  
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            var obj = base.NonQueryExecuting(command, eventData, result);
            return obj;
        }

        /// <summary>
        /// 在 EF 打算调用 System.Data.Common.DbCommand.ExecuteNonQueryAsync 之前调用 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var obj = await base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
            return obj;
        }

        /// <summary>
        /// EF 调用 System.Data.Common.DbCommand.ExecuteNonQuery 后立即调用
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            if (eventData.Duration.TotalMilliseconds >= DBSlowSqlLogTime * 1000)
            {
                //LogHelper.Warn("耗时的Sql：" + command.GetCommandText());
            }
            int val = base.NonQueryExecuted(command, eventData, result);
            return val;
        }

        /// <summary>
        /// EF 调用 System.Data.Common.DbCommand.ExecuteNonQueryAsync 后立即调用
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            if (eventData.Duration.TotalMilliseconds >= DBSlowSqlLogTime * 1000)
            {
                //LogHelper.Warn("耗时的Sql：" + command.GetCommandText());
            }
            int val = await base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
            return val;
        }

        /// <summary>
        /// 在 EF 打算调用 System.Data.Common.DbCommand.ExecuteScalar 之前调用
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            var obj = base.ScalarExecuting(command, eventData, result);
            return obj;
        }

        /// <summary>
        /// 在 EF 打算调用 System.Data.Common.DbCommand.ExecuteScalarAsync 之前调用
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            var obj = await base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
            return obj;
        }

        /// <summary>
        /// 在 EF 调用 System.Data.Common.DbCommand.ExecuteScalar 之后立即调用
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            if (eventData.Duration.TotalMilliseconds >= DBSlowSqlLogTime * 1000)
            {
                //LogHelper.Warn("耗时的Sql：" + command.GetCommandText());
            }
            var obj = base.ScalarExecutedAsync(command, eventData, result);
            return obj;
        }

        /// <summary>
        /// 在 EF 调用 System.Data.Common.DbCommand.ExecuteScalarAsync 之后立即调用
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
        {
            if (eventData.Duration.TotalMilliseconds >= DBSlowSqlLogTime * 1000)
            {
                //LogHelper.Warn("耗时的Sql：" + command.GetCommandText());
            }
            var obj = await base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
            return obj;
        }

        /// <summary>
        /// 在 EF 打算调用 System.Data.Common.DbCommand.ExecuteReader 之前调用
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            var obj = base.ReaderExecuting(command, eventData, result);
            return obj;
        }

        /// <summary>
        /// 在 EF 打算调用 System.Data.Common.DbCommand.ExecuteReaderAsync 之前调用
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            var obj = await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
            return obj;
        }

        /// <summary>
        /// EF 调用 System.Data.Common.DbCommand.ExecuteReader 后立即调用
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            if (eventData.Duration.TotalMilliseconds >= DBSlowSqlLogTime * 1000)
            {
                //LogHelper.Warn("耗时的Sql：" + command.GetCommandText());
            }
            var reader = base.ReaderExecuted(command, eventData, result);
            return reader;
        }

        /// <summary>
        /// EF 调用 System.Data.Common.DbCommand.ExecuteReaderAsync 后立即调用
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            if (eventData.Duration.TotalMilliseconds >= DBSlowSqlLogTime * 1000)
            {
                //LogHelper.Warn("耗时的Sql：" + command.GetCommandText());
            }
            var reader = await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
            return reader;
        }
    }
}
