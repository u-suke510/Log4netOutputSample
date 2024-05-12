using Microsoft.Extensions.Logging;

namespace SampleLog4net
{
    /// <summary>
    /// バッチ処理のServiceクラスのベースインターフェース
    /// </summary>
    public interface IServiceBase
    {
        /// <summary>
        /// メイン処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        bool Execute();

        /// <summary>
        /// 各種処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        bool ExecuteService();
    }

    /// <summary>
    /// バッチ処理のServiceクラスのベースクラス
    /// </summary>
    public abstract class ServiceBase : IServiceBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        /// 処理開始日時
        /// </summary>
        protected DateTime ExecDtm
        {
            get;
            private set;
        }
        /// <summary>
        /// 処理ID
        /// </summary>
        protected abstract string SrvId
        {
            get;
        }
        /// <summary>
        /// 処理名
        /// </summary>
        protected abstract string SrvNm
        {
            get;
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="provider">サービスプロバイダー</param>
        /// <param name="logger">ロガー</param>
        public ServiceBase(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// メイン処理を実装します。
        /// </summary>
        /// <param name="disabledJobMgmt">ジョブ管理情報の更新無効フラグ</param>
        /// <returns>処理結果</returns>
        public bool Execute()
        {
            try
            {
                ExecDtm = DateTime.Now;

                // 開始ログ
                logger.LogInformation("【{0}】{1}処理を開始します。", SrvId, SrvNm);

                // 各種処理の実施
                var result = ExecuteService();

                // 終了ログ
                endLog(result);
                return result;
            }
            catch (Exception ex)
            {
                // 終了ログ
                endLog(false, ex);
                return false;
            }
            throw new NotImplementedException();

            /// <summary>
            /// 終了ログの出力処理を実施します。
            /// </summary>
            /// <param name="result">処理結果</param>
            /// <param name="ex">例外</param>
            void endLog(bool result, Exception ex = null)
            {
                // 終了ログの出力
                if (result)
                {
                    logger.LogInformation("【{0}】{1}処理が正常に終了しました。", SrvId, SrvNm);
                }
                else
                {
                    logger.LogError("【{0}】{1}処理が異常終了しました。\r\n{2}", SrvId, SrvNm, ex == null ? string.Empty : ex);
                }
            }
        }

        /// <summary>
        /// 各種処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        public abstract bool ExecuteService();
    }
}
