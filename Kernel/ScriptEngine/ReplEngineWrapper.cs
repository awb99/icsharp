using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using ScriptCs;
//using ScriptCs.Argument;
using ScriptCs.Command;
//using ScriptCs.Contracts;
using ScriptCs.Hosting;

namespace iCSharp.Kernel.ScriptEngine
{
    internal class ReplEngineWrapper : IReplEngine
    {
        private readonly ILog logger;
        private readonly ScriptCs.Repl repl;
        private readonly MemoryBufferConsole console;

        public ReplEngineWrapper(ILog logger, ScriptCs.Repl repl, MemoryBufferConsole console)
        {
            this.logger = logger;
            this.repl = repl;
            this.console = console;
        }

        public ExecutionResult Execute(string script)
        {
            this.logger.Debug(string.Format("Executing: {0}", script));
            this.console.ClearAllInBuffer();

            ScriptCs.Contracts.ScriptResult scriptResult = repl.Execute(script);

            ExecutionResult executionResult = new ExecutionResult()
            {
                OutputResultWithColorInformation = this.console.GetAllInBuffer()
            };

            return executionResult;
        }

        private bool IsCompleteResult( ScriptCs.Contracts.ScriptResult scriptResult)
        {
            return scriptResult.ReturnValue != null && !string.IsNullOrEmpty(scriptResult.ReturnValue.ToString());
        }

        

        
    }
}
