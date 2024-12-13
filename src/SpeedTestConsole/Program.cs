using SpeedTestConsole.Commands;
using SpeedTestConsole.Lib.Client;
using SpeedTestConsole.Lib.Enums;



var app = new CommandApp<ListServersCommand>();

app.Configure(config =>
{
#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
});

var result = app.Run(args);

Console.ReadKey();

return result;



/*
AnsiConsole.MarkupLine("[bold][orange1]SpeedTestConsole[/][/]");

ISpeedTestClient speedTestClient = new SpeedTestClient();

speedTestClient.StageChanged += (_, stage) =>
{
    Console.WriteLine($"Changed stage to: {stage}");
};

speedTestClient.ProgressChanged += (_, info) =>
{
    switch (speedTestClient.CurrentStage)
    {
        case TestStage.Download:
            Console.Write("Downloaded ");
            break;
        case TestStage.Upload:
            Console.Write("Uploaded ");
            break;
    }
    Console.WriteLine($"{info.BytesProcessed} bytes @ {info.Speed} {speedTestClient.SpeedUnit}");
};

var result = await speedTestClient.TestSpeedAsync(SpeedUnit.Mbps, parallelTasks: 1, testLatency: true, testDownload: false, testUpload: false);

Console.WriteLine("============[Finished]============");
Console.WriteLine($"Latency: {result.Latency}ms");
Console.WriteLine($"Download: {result.DownloadSpeed} {result.SpeedUnit}");
Console.WriteLine($"Upload: {result.UploadSpeed} {result.SpeedUnit}");
Console.ReadKey();
*/