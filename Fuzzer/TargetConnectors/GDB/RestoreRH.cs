// RestoreRH.cs
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
using System.Text.RegularExpressions;
using System.Globalization;
namespace Fuzzer.TargetConnectors.GDB
{
	/// <summary>
	/// Handles breakpoint responses.
	/// </summary>
	public class RestoreRH : GDBResponseHandler
	{
		
		
		#region implemented abstract members of Fuzzer.TargetConnectors.GDB.GDBResponseHandler
		public override string LogIdentifier 
		{
			get { return "RH_restore"; }
		}
		
		
		public override GDBResponseHandler.HandleResponseEnum HandleResponse (GDBSubProcess connector, string[] responseLines, bool allowRequestLine)
		{
			return GDBResponseHandler.HandleResponseEnum.Handled;
		}
		
		#endregion
		public RestoreRH (GDBSubProcess gdbProc)
			:base(gdbProc)
		{
		}
	}
}