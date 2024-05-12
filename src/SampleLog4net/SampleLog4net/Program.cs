using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleLog4net;
using SampleLog4net.Services;

// 処理対象
List<string> targets = null;
// ロガー
ILogger<Program> logger;
// サービスコレクション
IServiceCollection services;
// サービスプロバイダー
IServiceProvider provider;
// Logging情報設定
IConfigurationSection confLogging;

// 初期化
Setup();

// 各処理の実行
var isAbort = false;
foreach (var target in targets)
{
    // サービスクラスの取得
    var service = getService(target);
    if (service == null)
    {
        logger.LogError("処理名に誤りがあります。(TargetName:{0})", target);
        continue;
    }

    // サービスクラスの実行
    if (!service.Execute())
    {
        isAbort = true;
    }
    Thread.Sleep(1000);
}

/// <summary>
/// バッチ処理を初期化します。
/// </summary>
void Setup()
{
    // アプリケーション設定
    var confBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true);
    var configuration = confBuilder.Build();
    if (targets == null)
    {
        // 処理対象の取得
        targets = configuration.GetSection("Targets").Get<List<string>>();
    }
    confLogging = configuration.GetSection("Logging");

    // サービスの生成
    services = new ServiceCollection();
    services.AddSingleton<IB00Service, B00Service>();
    services.AddSingleton<IB01Service, B01Service>();
    services.AddSingleton<IB02Service, B02Service>();
}

/// <summary>
/// 処理名から対象のサービスクラスを取得します。
/// </summary>
/// <param name="target">処理名</param>
/// <returns>サービスクラス</returns>
IServiceBase getService(string target)
{
    setLogging(target);
    switch (target)
    {
        case "B00":
            return provider.GetService<IB00Service>();
        case "B01":
            return provider.GetService<IB01Service>();
        case "B02":
            return provider.GetService<IB02Service>();
        default:
            return null;
    }
}

/// <summary>
/// ロガー情報を設定します。
/// </summary>
/// <param name="target">処理名</param>
void setLogging(string target)
{
    services.AddLogging(builder => {
        builder.ClearProviders();
        builder.AddConfiguration(confLogging);
        builder.AddConsole();
        builder.AddLog4Net(createLogOptions(target));
    });
    provider = services.BuildServiceProvider();

    // ロガーの生成
    var loggerFactory = provider.GetService<ILoggerFactory>();
    logger = loggerFactory.CreateLogger<Program>();
}

/// <summary>
/// ロガーオプションを生成します。
/// </summary>
/// <param name="target">処理名</param>
/// <returns>Log4netオプション</returns>
Log4NetProviderOptions createLogOptions(string target)
{
    var result = new Log4NetProviderOptions();
    result.PropertyOverrides.Add(new Microsoft.Extensions.Logging.Log4Net.AspNetCore.Entities.NodeInfo {
        XPath = "log4net/appender/file",
        Attributes = new Dictionary<string, string> { { "value", $"log4net/DebugLog.{target}" } }
    });

    return result;
}
