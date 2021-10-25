using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace swfd.core
{
    public class ScriptRunningService : TimedHostedService
    {
        private const string PowershellExtension = "*.ps1";
        private const string JsonExtension = "*.json";
        private FileSystemWatcher _inputFileWatcher;
        private FileSystemWatcher _scriptFileWatcher;
        public string Key { get; internal set; }
        public string ScriptFolder { get; internal set; }
        public string InputFolder { get; internal set; }
        public List<WorkflowPayload> Payloads { get; private set; }
        public List<IWorkflowStepCollection> Steps { get; internal set; }
        public Dictionary<string, string> Scripts {get; private set;}

        public ScriptRunningService(ILogger<ScriptRunningService> logger) : base(logger)
        {
            Payloads = new List<WorkflowPayload>();
            Scripts = new Dictionary<string, string>();
        }

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            if(!Directory.Exists(ScriptFolder))
                Directory.CreateDirectory(ScriptFolder);
            
            if(!Directory.Exists(InputFolder))
                Directory.CreateDirectory(InputFolder);

            var notifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            _scriptFileWatcher = new FileSystemWatcher(InputFolder);
            _scriptFileWatcher.NotifyFilter = notifyFilter;

            _scriptFileWatcher.Created += OnScriptFileChanged;
            _scriptFileWatcher.Changed += OnScriptFileChanged;
            _scriptFileWatcher.Renamed += OnScriptFileChanged;
            _scriptFileWatcher.Deleted += OnScriptFileDeleted;

            _scriptFileWatcher.Filter = PowershellExtension;
            _scriptFileWatcher.IncludeSubdirectories = false;

            ProcessScripts();

            _scriptFileWatcher.EnableRaisingEvents = true;

            _inputFileWatcher = new FileSystemWatcher(InputFolder);
            _inputFileWatcher.NotifyFilter = notifyFilter;

            _inputFileWatcher.Created += OnInputFileChanged;
            _inputFileWatcher.Changed += OnInputFileChanged;
            _inputFileWatcher.Renamed += OnInputFileChanged;
            _inputFileWatcher.Deleted += OnInputFileDeleted;

            _inputFileWatcher.Filter = JsonExtension;
            _inputFileWatcher.IncludeSubdirectories = false;

            ProcessInputs();

            _inputFileWatcher.EnableRaisingEvents = true;

            return base.StartAsync(stoppingToken);
        }

        private void ProcessInputs()
        {
            var folders = Directory.GetDirectories(InputFolder);
            foreach(var folder in folders)
            {
                var scriptKey = Path.GetDirectoryName(folder);
                
                var files = Directory.GetFiles(folder, JsonExtension);
                foreach(var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var payloadJson = File.ReadAllText(file);
                    var payload = JsonSerializer.Deserialize<WorkflowPayload>(payloadJson);
                    payload.PayloadKey = fileName;
                    
                    Payloads.Add(payload);
                }
            }
        }

        private void ProcessScripts()
        {
            var scriptKey = Path.GetDirectoryName(ScriptFolder);
            
            var files = Directory.GetFiles(ScriptFolder, JsonExtension);
            foreach(var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var payload = File.ReadAllText(file);
                
                Scripts.Add(fileName, payload);
            }
        }

        private void OnScriptFileDeleted(object sender, FileSystemEventArgs e)
        {
            var fileName = Path.GetFileNameWithoutExtension(e.FullPath);
            Scripts.Remove(fileName);
        }

        private void OnScriptFileChanged(object sender, FileSystemEventArgs e)
        {
            var fileName = Path.GetFileNameWithoutExtension(e.FullPath);
            var payload = File.ReadAllText(e.FullPath);
            Scripts[fileName] = payload;
        }

        private void OnInputFileDeleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnInputFileChanged(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(CancellationToken token)
        {
            foreach(var jobs in Payloads)
            {
                
            }
        }

        public override void Dispose(){
            _inputFileWatcher?.Dispose();
            _inputFileWatcher = null;

            _scriptFileWatcher?.Dispose();
            _scriptFileWatcher = null;

            foreach(var payload in Payloads)
            {
                var filePath = Path.Combine(InputFolder, $"{payload.PayloadKey}.json");
                var jsonPayload = JsonSerializer.Serialize(payload);
                File.WriteAllText(filePath, jsonPayload);
            }
        }
    }
}
