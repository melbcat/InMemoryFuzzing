// GDBSymbolTable.cs
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
using Fuzzer.IO.ConsoleIO;
using System.Collections.Generic;
using Iaik.Utils;
using System.Threading;
using Iaik.Utils.CommonAttributes;
namespace Fuzzer.TargetConnectors.GDB
{
	/// <summary>
	/// Provides access to ELF-embedded debugging and linker symbol tables
	/// using a gdb sub process
	/// </summary>
	/// 
	[ClassIdentifier("symbol_table/gdb")]
	public class GDBSymbolTable : GDBSubProcess, ISymbolTable
	{
		/// <summary>
		/// File to read the symbol table from.
		/// </summary>
		protected string _file = null;
		 
		/// <summary>
		/// The cached methods
		/// </summary>
		private ISymbolTableMethod[] _cachedMethods = null;
		
		
		private GDBConnector _connector = null;
		
		public GDBConnector Connector
		{
			get{ return _connector; }
			set{ _connector = value; }
		}
		
		public GDBSymbolTable ()
		{
			RegisterPermanentResponseHandler(new UnhandledRH(this));
		}
		
		public override void Setup (IDictionary<string, string> config)
		{
			base.Setup (config);
			
			_file = DictionaryHelper.GetString("file", config, null);
			if(_file == null)
				throw new ArgumentException("No 'file' specified");
			
			StartProcess();
			
			ManualResetEvent evt = new ManualResetEvent(false);
			bool success = false;
			QueueCommand(new FileCmd(_file, 
			   delegate(bool s){ 
				success = s;
				evt.Set();}, this));
			
			evt.WaitOne();
			if(!success) throw new ArgumentException("Could not load file");
		}
		
		public ISymbolTableMethod[] ListMethods
		{
			get
			{
				CheckCachedMethods(false);
				
				return _cachedMethods;
			}
		}
		
		public ISymbolTableMethod FindMethod(string methodName)
		{
			CheckCachedMethods(false);
			
			foreach(ISymbolTableMethod m in _cachedMethods)
			{
				if(m.Name.Equals(methodName))
					return m;
			}
			
			return null;
		}
		
		public IAddressSpecifier ResolveSymbol(ISymbol symbol)
		{
			ManualResetEvent evt = new ManualResetEvent(false);
			IAddressSpecifier myAddress = null;
			
			QueueCommand(new InfoAddressCmd(symbol,
			     delegate(ISymbol s, IAddressSpecifier address){
				 	myAddress = address;
					evt.Set();
			     }, this));
			
			evt.WaitOne();
			
			return myAddress;
		}
		
		public IAddressSpecifier ResolveSymbolToBreakpointAddress(ISymbolTableMethod symbol)
		{
			IAddressSpecifier address = null;
			ManualResetEvent evt = new ManualResetEvent(false);
			
			QueueCommand(new SetBreakpointNameCmd(symbol,
			       delegate(int breakpointNum, UInt64 breakpointAddress)
			       {
					  address = new StaticAddress(breakpointAddress);
						
				      QueueCommand(new DeleteBreakpointCmd(breakpointNum, this));
					  evt.Set();
				   }, this));
			evt.WaitOne();
			return address;
		}
		
		public ISymbolTableVariable[] GetParametersForMethod(ISymbolTableMethod method)
		{
			List<ISymbolTableVariable> variables = new List<ISymbolTableVariable>();
			string[] myParameterTypes = null;
			ManualResetEvent evt = new ManualResetEvent(false);
			QueueCommand(
			  new WhatIsCmd(this, method, 
			    delegate(ISymbol symbol, string returnType, string[] parameterTypes)
                {
					myParameterTypes = parameterTypes;
					evt.Set();
				}));
			
			evt.WaitOne();
			
			if(myParameterTypes != null)
			{
				ISymbol[] myDiscoveredSymbols = null;
				evt.Reset();
				QueueCommand(new InfoScopeCmd(this, method.Name, 
				   delegate(ISymbol[] discoveredSymbols)
				   {
					  myDiscoveredSymbols = discoveredSymbols;
					  evt.Set();
				   }));
				
				evt.WaitOne();
				if(myDiscoveredSymbols.Length >= myParameterTypes.Length)
				{
					//Now we know the number of parameters and got all local variables valid in the
					//method-scope. GDB (hopefully) always outputs the parameters first, so we got
					//the parameter names.....is there a simpler method??
					for(int i = 0; i<myParameterTypes.Length; i++)
					{
						//TODO: Maybe include the type in ISymbolTableVariable?
						variables.Add(new GDBSymbolTableVariable(this, myDiscoveredSymbols[i].Symbol));
					}
				}
				return variables.ToArray();
			}
			else
				return null;
		}
		
		
		private void CheckCachedMethods(bool forced)
		{
			if(_cachedMethods == null || forced)
			{
				ManualResetEvent evt = new ManualResetEvent(false);
				ISymbolTableMethod[] myResolvedMethods = null;
				ISymbolTableMethod[] myUnresolvedMethods = null;
				
				QueueCommand(new InfoFunctionsCmd(this, 
				   delegate(ISymbolTableMethod[] resolvedMethods, ISymbolTableMethod[] unresolvedMethods){
					  myResolvedMethods = resolvedMethods;
					  myUnresolvedMethods = unresolvedMethods;
					  evt.Set();
				}, this));
				
				evt.WaitOne();
				
				List<ISymbolTableMethod> methods = new List<ISymbolTableMethod>();
				methods.AddRange(myResolvedMethods);
				
				foreach(ISymbolTableMethod m in myUnresolvedMethods)
					m.Resolve();
				
				methods.AddRange(myUnresolvedMethods);
				
				
				//Resolve method parameters, gdb cannot discover and resolve in a single step
				foreach(SymbolTableMethod m in methods)
					m.Parameters = GetParametersForMethod(m);
				
				_cachedMethods = methods.ToArray();
			}
			
		}
	}
}

