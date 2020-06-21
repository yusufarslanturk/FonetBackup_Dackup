﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using McMaster.Extensions.CommandLineUtils;

using dackup.Configuration;

namespace dackup
{
    [Command(UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw)]
    class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name        = "dackup",
                Description = "A backup app for your server or database or desktop",
            };

            app.HelpOption(inherited: true);

            app.Command("new", newCmd =>
            {
                    newCmd.Description = "Generate a config file";
                var modelName          = newCmd.Argument("model", "Name of the model").IsRequired();

                newCmd.OnExecute(() =>
                {
                    var fileName = Path.Combine(Environment.CurrentDirectory, modelName.Value + ".config");
                    PerformConfigHelper.GenerateMockupConfig(fileName);
                });
            });

            app.Command("perform", performCmd =>
            {
                performCmd.Description = "Performing your backup by config";

                var configFile = performCmd.Option("--config-file  <FILE>", "Required. The File name of the config.", CommandOptionType.SingleValue)
                                            .IsRequired()
                                            .Accepts(v => v.ExistingFile());
                var logPathConfig = performCmd.Option("--log-path  <PATH>", "op. The File path of the log.", CommandOptionType.SingleValue);
                var tmpPathConfig = performCmd.Option("--tmp-path  <PATH>", "op. The tmp path.", CommandOptionType.SingleValue);

                performCmd.OnExecute(() =>
                {
                    var logPath        = logPathConfig.Value();
                    var tmpPath        = tmpPathConfig.Value();
                    var configFilePath = configFile.Value();

                    if (string.IsNullOrEmpty(logPath))
                    {
                        logPath = "log";
                    }
                    if (string.IsNullOrEmpty(tmpPath))
                    {
                        tmpPath = "tmp";
                    }

                    DackupContext.Create(Path.Join(logPath, "dackup.log"), tmpPath);

                    var serviceProvider = ServiceProviderFactory.ServiceProvider;

                    var logger = serviceProvider.GetService<ILogger<Program>>();
                    AppDomain.CurrentDomain.UnhandledException += (s, args) => logger.LogError(args.ExceptionObject as Exception, "*** Crash! ***", "UnhandledException");
                    TaskScheduler.UnobservedTaskException += (s, args) => logger.LogError(args.Exception, "*** Crash! ***", "UnobservedTaskException");

                    var app = serviceProvider.GetService<DackupApplication>();
                    app.Run(configFilePath).Wait();
                });
            });

            app.OnExecute(() =>
            {
                app.ShowHelp();
            });

            app.Execute(args);
        }
    }
}