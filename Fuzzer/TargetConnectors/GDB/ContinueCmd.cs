// ContinueRH.cs
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
namespace Fuzzer.TargetConnectors.GDB
{
	/// <summary>
	/// Sends a continue command
	/// </summary>
	public class ContinueCmd:GDBCommand
	{
		private ContinueRH _continueRH;
		private bool _reverse = false;
		
		#region implemented abstract members of Fuzzer.TargetConnectors.GDB.GDBCommand
		public override GDBResponseHandler ResponseHandler 
		{
			get { return _continueRH; }
		}		
		
		public override string Command 
		{
			get 
			{
				if(_reverse)
					return "reverse-continue";
				else
					return "continue"; }
		}
		
		
		protected override string LogIdentifier 
		{
			get { return "CMD_continue" + (_reverse ? "_r": ""); }
		}
		
		#endregion
		public ContinueCmd (bool reverse, string runArgs, Action<bool> continueResult, GDBSubProcess gdbProc)
			:base(gdbProc)
		{
			_reverse = reverse;
			_continueRH = new ContinueRH(runArgs, continueResult, _gdbProc);
		}
	}
}

