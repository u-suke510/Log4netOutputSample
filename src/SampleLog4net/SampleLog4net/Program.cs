using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleLog4net;
using SampleLog4net.Services;

// 処理対象
List<string> targets = null;
// ロガー
ILogger<Program> logger;
// サービスプロバイダー
IServiceProvider provider;

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

    // サービスの生成
    var services = new ServiceCollection();
    services.AddLogging(builder => {
        builder.AddConfiguration(configuration.GetSection("Logging"));
        builder.AddConsole();
        builder.AddLog4Net(new Log4NetProviderOptions("log4net.config"));
    });
    services.AddSingleton<IB00Service, B00Service>();
    provider = services.BuildServiceProvider();

    // ロガーの生成
    var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
    logger = loggerFactory.CreateLogger<Program>();
}

/// <summary>
/// 処理名から対象のサービスクラスを取得します。
/// </summary>
/// <param name="target">処理名</param>
/// <returns>サービスクラス</returns>
IServiceBase getService(string target)
{
    switch (target)
    {
        case "B00":
            return provider.GetService<IB00Service>();
        default:
            return null;
    }
}
