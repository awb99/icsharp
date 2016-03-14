using System;
using System.Collections.Generic;

using Common.Logging;
using Common.Logging.Simple;

using ScriptCs.Argument;
using ScriptCs.Command;
using ScriptCs.Hosting;
using ScriptCs.Contracts;
using ScriptCs.Engine.Roslyn;
using ScriptCs.Engine.Mono ;
using ScriptCs;

using iCSharp.Kernel;
using iCSharp.Kernel.ScriptEngine;

namespace testEngine
{
	class Program
	{
		
		private static ScriptCs.Repl CreateRepl (Common.Logging.ILog logger,IConsole console)
		{
			ObjectSerializer serializer = new ObjectSerializer ();

			var shf = new ScriptHostFactory ();
			
			var  scriptEngine =new MonoScriptEngine (shf, logger);
        			
			//RoslynScriptEngine scriptEngine =new RoslynScriptEngine (, logger);
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
		
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			
			// TODO: Implement Functionality Here
			
			Common.Logging.ILog logger = new ConsoleOutLogger("kernel.log", Common.Logging.LogLevel.All, true, true, false, "yyyy/MM/dd HH:mm:ss:fff");
			MemoryBufferConsole console = new MemoryBufferConsole ();
			
			var repl =CreateRepl (logger, console);

			repl.Execute("int i=13;");
			repl.Execute("int j=i+7;");
			repl.Execute("j");
			
			foreach (var x in console.GetAllInBuffer())
			{
				Console.WriteLine(x.Item1);
			}
						
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		
		
	}
}