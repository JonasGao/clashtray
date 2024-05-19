﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    internal class ClashProcess
    {
        private readonly Process _process;
        private bool _clashRunning;

        internal event MessageReceivedEventHandler? MessageReceived;

        internal bool Running
        {
            get { return _clashRunning; }
        }

        private ClashProcess(Process process)
        {
            _process = process;
        }

        internal static ClashProcess Create()
        {
            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            ClashProcess clashProcess = new(process);
            return clashProcess;
        }

        internal string QueryProcess()
        {
            var pwd = Directory.GetCurrentDirectory();
            var builder = new StringBuilder()
                .Append("# Working Directory: ").AppendLine(pwd)
                .Append("# Core: ").AppendLine(_process.StartInfo.FileName)
                .Append("# Arguments: ").AppendLine(_process.StartInfo.Arguments)
                .AppendLine("------");
            if (!_clashRunning)
            {
                builder.AppendLine("# Running Flag: false");
            }
            else
            {
                try
                {
                    var pid = _process.Id;
                    builder.Append("# ID: ").AppendLine(pid.ToString())
                        .Append("# Running Flag: ").AppendLine(_clashRunning.ToString())
                        .Append("# HasExited: ").AppendLine(_process.HasExited.ToString());
                }
                catch (Exception)
                {
                    builder.AppendLine("# Running Flag: true. But Process not exists");
                }
            }

            var content = builder.ToString();
            return content;
        }

        internal void Startup(String clashFilepath, StringBuilder args)
        {
            if (args != null && args.Length > 0)
            {
                _process.StartInfo.Arguments = args.ToString();
            }
            else
            {
                _process.StartInfo.Arguments = null;
            }

            _process.StartInfo.FileName = clashFilepath;
            _process.Start();
            _clashRunning = true;
        }

        private void SetOutput(string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        internal void Kill()
        {
            _process.Kill();
            _process.WaitForExit();
            _clashRunning = false;
        }
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }

        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }
    }

    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
}
