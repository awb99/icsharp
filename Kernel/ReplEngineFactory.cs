using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


using Common.Logging;
using iCSharp.Kernel.ScriptEngine;
using ScriptCs;
//using ScriptCs.Argument;
using ScriptCs.Contracts;
using ScriptCs.Hosting;
using ScriptCs.Engine.Mono ;
using  ScriptCs.Engine;

using ScriptCs.Engine.Roslyn;
using ScriptCs;

namespace iCSharp.Kernel
{
    public class ReplEngineFactory
    {
        private string[] args;

        private IReplEngine _replEngine;
        private ScriptCs.Repl _repl;
        private MemoryBufferConsole _console;
        private Common.Logging.ILog _logger;

        public ReplEngineFactory(Common.Logging.ILog logger, string[] args)
        {
            this._logger = logger;
            this.args = args;
        }

        public IReplEngine ReplEngine
        {
            get
            {
                if (this._replEngine == null)
                {
                    this._replEngine = new ReplEngineWrapper(this.Logger, this.Repl, this.Console);
                }

                return this._replEngine;
            }
        }

        public MemoryBufferConsole Console
        {
            get { return this._console; }
        }

        private ScriptCs.Repl Repl
        {
            get
            {
                if (this._repl == null)
                {
                    this._repl = this.GetRepl(this.args, out this._console);
                }

                return this._repl;
            }
        }

        private Common.Logging.ILog Logger
        {
            get { return this._logger; }
        }

        private static ScriptCs.Repl CreateRepl (Common.Logging.ILog logger,IConsole console)
		{
			ObjectSerializer serializer = new ObjectSerializer ();
			
			var shf = new ScriptHostFactory ();
			var  scriptEngine =new MonoScriptEngine (shf, logger);
        	
			//RoslynScriptEngine scriptEngine =new RoslynScriptEngine (shf, logger);
			var initializationServices = new InitializationServices(logger);
			initializationServices.GetAppDomainAssemblyResolver().Initialize();
			
			var fileSystem = initializationServices.GetFileSystem();
			string[] ScriptArgs = new string[0];
			
			FilePreProcessor filePreProcessor = new FilePreProcessor (fileSystem,logger, new List<ILineProcessor>());
			var repl = new ScriptCs.Repl(ScriptArgs, fileSystem, scriptEngine, serializer, logger, console, filePreProcessor, new List<IReplCommand> ());

			var workingDirectory = fileSystem.CurrentDirectory;
			var assemblies = initializationServices.GetAssemblyResolver ();
			// var assemblies = _assemblyResolver.GetAssemblyPaths(workingDirectory);
			// initializationServices.GetModuleLoader();
			// var scriptPacks = _scriptPackResolver.GetPacks();
			string[] ass = new string[0];
			
			repl.Initialize(ass, new List<IScriptPack>(), ScriptArgs);
			return repl;
		}
        
        private ScriptCs.Repl GetRepl(string[] args, out MemoryBufferConsole console)
        {
            SetProfile();
           	console = new MemoryBufferConsole ();
           	return CreateRepl (_logger, console);
        }
        
        
  

        //private static ArgumentParseResult ParseArguments(string[] args)
        //{
        //    var console = new ScriptConsole();
          //  try
        //    {
          //      var parser = new ArgumentHandler(new ArgumentParser(console), new ConfigFileParser(console), new FileSystem());
            //    return parser.Parse(args);
        //    }
        //    finally
        //    {
          //      console.Exit();
        //    }
        //}

        private static void SetProfile()
        {
            var profileOptimizationType = Type.GetType("System.Runtime.ProfileOptimization");
            if (profileOptimizationType != null)
            {
                var setProfileRoot = profileOptimizationType.GetMethod("SetProfileRoot", BindingFlags.Public | BindingFlags.Static);
                setProfileRoot.Invoke(null, new object[] { typeof(Program).Assembly.Location });

                var startProfile = profileOptimizationType.GetMethod("StartProfile", BindingFlags.Public | BindingFlags.Static);
                startProfile.Invoke(null, new object[] { typeof(Program).Assembly.GetName().Name + ".profile" });
            }
        }
    }
}
