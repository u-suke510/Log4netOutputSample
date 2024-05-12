using Microsoft.Extensions.Logging;

namespace SampleLog4net.Services
{
    /// <summary>
    /// B02処理のServiceインターフェース
    /// </summary>
    public interface IB02Service : IServiceBase
    {
    }

    /// <summary>
    /// B02処理のServiceクラス
    /// </summary>
    public class B02Service : ServiceBase, IB02Service
    {
        /// <summary>
        /// 処理ID
        /// </summary>
        protected override string SrvId { get; }
        /// <summary>
        /// 処理名
        /// </summary>
        protected override string SrvNm { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="logger">ロガー</param>
        public B02Service(ILogger<B02Service> logger) : base(logger)
        {
            // 処理IDの設定
            SrvId = "B02";
            // 処理名の設定
            SrvNm = "サンプル02";
        }

        /// <summary>
        /// サンプル02処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        public override bool ExecuteService()
        {
            logger.LogDebug($"Test Debug Log.");
            logger.LogInformation($"Test Info Log.");
            logger.LogWarning($"Test Warn Log.");
            logger.LogError($"Test Err Log.");

            return true;
        }
    }
}
