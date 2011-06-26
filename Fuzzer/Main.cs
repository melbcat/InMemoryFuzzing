using System;

using Iaik.Utils.CommonFactories;
using Fuzzer.TargetConnectors;
using System.Collections.Generic;
using System.Diagnostics;
using Iaik.Utils.IO;
using System.Globalization;
using System.Reflection;
using Iaik.Utils;
using Fuzzer.FuzzDescriptions;
using Fuzzer.DataGenerators;
using Iaik.Utils.libbfd;
using System.Runtime.InteropServices;
using System.IO;
using Fuzzer.TargetConnectors.GDB.CoreDump;
using System.Net.Sockets;
using Fuzzer.RemoteControl;
using System.Threading;
using System.Text;

namespace Fuzzer
{
	class MainClass
	{
		
		public static void Main (string[] args)
		{
			
//			NetworkStream netStream = RemoteControl.RemoteControlConnectionBuilder.Connect ("127.0.0.1", 8899);
//			RemoteControl.RemoteControlProtocol p = new RemoteControl.RemoteControlProtocol ();
//			p.PipeOpened += HandlerPipeOpened;
//			p.PipeClosed += HandlerPipeClosed;
//			p.PipeData += HandlerPipeData;
//			p.RemoteProcessInfo += HandlerRemoteProcessInfo;
//			p.SetConnection (netStream);
			//p.RemoteEcho ("TEST");
			
//			p.RemoteExec ("ls", "/bin/ls", 
//				new List<string> (new string[] { 
//					"-l", "-a"
//			}), 
//				new List<string> (new string[] {
//					"LD_PRELOAD=/home/andi/Documents/Uni/master-thesis/src/log_memory_allocations/liblog_memory_allocations.so",
//					"LOG_MEM_PIPE=test_pipe"
//			}));
//			
//			p.RemoteRequestPipe ("test_pipe");
//			//p.RemoteProcesses ();
//			Thread.Sleep (-1);
			
//			GDBCoreDump coreDump = new GDBCoreDump("/home/andi/hacklet/prog0-x64.execution_log", null, Registers.CreateFromFile("/home/andi/x86-64.registers"));
//			GDBProcessRecordSection processRecord = coreDump.GetProcessRecordSection();

			SetupLogging();
			TestApamaLinux();
		}

		static void HandlerRemoteProcessInfo (RemoteProcessInfo[] processes)
		{
			Console.WriteLine ("aiaiai");
		}

		static void HandlerPipeData (int pipeId, string pipeName, Byte[] data, int index, int offset)
		{
			Console.WriteLine ("PIPEDATA [{0}-{1}]: {2}", pipeId, pipeName, Encoding.ASCII.GetString (data, index, offset));
		}

		static void HandlerPipeClosed (int pipeId, string pipeName)
		{
			Console.WriteLine ("Pipe '{0}' closed", pipeName);
		}

		static void HandlerPipeOpened (int pipeId, string pipeName)
		{
			Console.WriteLine ("Pipe '{0}' opened", pipeName);
		}
		
		private static void TestApamaLinux()
		{	
			IDictionary<string, string> config = new Dictionary<string, string>();
			config.Add("gdb_exec", "/opt/gdb-7.2/bin/gdb");
			config.Add("gdb_log", "stream:stderr");
			
			//config.Add("target", "extended-remote :1234");
			
			config.Add("target", "run_local");
			
			//config.Add("target", "attach_local");
			//config.Add("target-options", "14577");
			
			//config.Add("file", "/home/andi/Documents/Uni/master-thesis/src/test_sources/gdb_reverse_debugging_test/gdb_reverse_debugging_test");
			config.Add("file", "/home/andi/hacklet/prog0-x64");
			
			using(ITargetConnector connector = 
				GenericClassIdentifierFactory.CreateFromClassIdentifierOrType<ITargetConnector>("general/gdb"))
			{
				ISymbolTable symbolTable = (ISymbolTable)connector;
				
				connector.Setup(config);
				connector.Connect();
				
				ISymbolTableMethod main = symbolTable.FindMethod("main");
				IBreakpoint snapshotBreakpoint = connector.SetSoftwareBreakpoint(main, 0, "break_snapshot");
				IBreakpoint restoreBreakpoint = connector.SetSoftwareBreakpoint (0x4007b5, 0, "break_restore");
				
//				IFuzzDescription barVar1_Description = new SingleValueFuzzDescription(bar.Parameters[0], 
//					new RandomByteGenerator( 4, 4, RandomByteGenerator.ByteType.All));		
//				IFuzzDescription barVar1_readableChar = new PointerValueFuzzDescription(bar.Parameters[0],
//					new RandomByteGenerator(5, 1000, RandomByteGenerator.ByteType.PrintableASCIINullTerminated));
				
				connector.DebugContinue ();
//				Registers r = ((Fuzzer.TargetConnectors.GDB.GDBConnector)connector).GetRegisters();
//				using(FileStream fSink = new FileStream("/home/andi/x86-64.registers", FileMode.CreateNew, FileAccess.Write))
//				{
//					StreamHelper.WriteTypedStreamSerializable(r, fSink);
//				}
			 	ISymbolTableVariable argv = main.Parameters[1];
				ISymbolTableVariable dereferencedArgv = argv.Dereference();
				
				IFuzzDescription fuzzArgv = new PointerValueFuzzDescription(
					dereferencedArgv, new RandomByteGenerator(
				                          100, 10000, RandomByteGenerator.ByteType.PrintableASCIINullTerminated));
				IStackFrameInfo stackFrameInfo = connector.GetStackFrameInfo();
				FuzzController fuzzController = new FuzzController(
					connector,
					snapshotBreakpoint,
					restoreBreakpoint,
					fuzzArgv);
					
				fuzzController.Fuzz();
			}
			

			
			Console.ReadLine();
			
		}
		
		
		/// <summary>
		/// Initializes the logger
		/// </summary>
		private static void SetupLogging()
		{
			log4net.Appender.ConsoleAppender appender = new log4net.Appender.ConsoleAppender();	
			appender.Name = "ConsoleAppender";
			appender.Layout = new log4net.Layout.PatternLayout("[%date{dd.MM.yyyy HH:mm:ss,fff}]-%-5level-%t-[%c]: %message%newline");
			log4net.Config.BasicConfigurator.Configure(appender);
		
			
			//_logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}

