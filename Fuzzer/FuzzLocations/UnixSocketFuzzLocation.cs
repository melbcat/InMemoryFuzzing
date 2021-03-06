// UnixSocketFuzzLocation.cs
//  
//  Author:
//       Andreas Reiter <andreas.reiter@student.tugraz.at>
// 
//  Copyright 2011  Andreas Reiter
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.Xml;
using Fuzzer.TargetConnectors;
using Iaik.Utils.Net;
using Iaik.Utils;
using Iaik.Utils.CommonAttributes;
using System.Collections.Generic;
using Fuzzer.Scripting.Environments;
using Fuzzer.Scripting;
namespace Fuzzer.FuzzLocations
{
	[ClassIdentifier("fuzzer/unix_socket")]
	public class UnixSocketFuzzLocation : BaseFuzzLocation
	{
		/// <summary>
		/// Evaluates the provided hook scripts
		/// </summary>
		private ScriptEvaluator<UnixSocketEnvironment> _scriptEvaluator;
		
		private UnixSocketConnection _socket;
			
		public UnixSocketConnection Connection
		{
			get { return _socket;}
		}
		
		#region implemented abstract members of Fuzzer.FuzzLocations.BaseFuzzLocation
		protected override bool SupportsStopCondition 
		{
			get { return true; }
		}
		
		
		protected override bool SupportsDataGen 
		{
			get { return true; }
		}
		
		
		protected override bool SupportsTrigger 
		{
			get { return true; }
		}
		
		#endregion
		
		public override void Init (XmlElement fuzzLocationRoot, ITargetConnector connector, Dictionary<string, IFuzzLocation> predefinedFuzzers)
		{
			base.Init (fuzzLocationRoot, connector, predefinedFuzzers);
			
			IDictionary<string, string> config = DictionaryHelper.ReadDictionaryXml (fuzzLocationRoot, "FuzzerArg");
			
			_scriptEvaluator = new ScriptEvaluator<UnixSocketEnvironment> (config, this);
			
			
			
			_socket = new UnixSocketConnection (config);
			
			//Attach to the UnixSocketConnection hooks and call the associated user script
			_socket.Hook_BeforeSocketCreation += delegate(UnixSocketConnection conn) {
				_scriptEvaluator.Environment.HookType = UnixSocketHookType.BeforeSocketCreation;
				_scriptEvaluator.Run ();
			};
			
			_socket.Hook_AfterSocketCreation += delegate(UnixSocketConnection conn) {
				_scriptEvaluator.Environment.HookType = UnixSocketHookType.AfterSocketCreation;
				_scriptEvaluator.Run ();
			};
			
			_socket.Hook_AfterSocketConnect += delegate(UnixSocketConnection conn) {
				_scriptEvaluator.Environment.HookType = UnixSocketHookType.AfterSocketConnect;
				_scriptEvaluator.Run ();
			};
			
			_socket.Hook_BeforeSocketClose += delegate(UnixSocketConnection conn) {
				_scriptEvaluator.Environment.HookType = UnixSocketHookType.BeforeSocketClose;
				_scriptEvaluator.Run ();
			};
			
			_socket.Hook_AfterSocketClose += delegate(UnixSocketConnection conn) {
				_scriptEvaluator.Environment.HookType = UnixSocketHookType.AfterSocketClose;
				_scriptEvaluator.Run ();
			};
		}
		
		public override void Run (FuzzController ctrl)
		{
			base.Run (ctrl);
			
			if (!_socket.Connected)
				_socket.Connect ();
			
			byte[] data = _dataGenerator.GenerateData ();
			_socket.Write (data, 0, data.Length);
		}
		
		protected override void Disposing ()
		{ 
			base.Dispose ();
			
			if (_socket != null && _socket.Connected)
			{
				_socket.Close ();
				_socket.Dispose ();
				_socket = null;
			}
		}
		
	}
}

