// IDataLogger.cs
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
namespace Fuzzer.DataLoggers
{
	/// <summary>
	/// Implemented by target connection specific classes.
	/// They log all relevant data generated by a target under test.
	/// The logger can also contain sub-loggers for reusable logging tasks
	/// (e.g. logging memory allocations for a gdb logger)
	/// </summary>
	public interface IDataLogger
	{
		/// <summary>
		/// Gets or sets a prefix for the generated log files.
		/// The prefix is changed by the fuzz controller on every fuzzing run
		/// </summary>
		string Prefix { get; set; }
		
		/// <summary>
		/// A single fuzz run (single set of input data) has been finished.
		/// The state of the connector has not been modified
		/// </summary>
		void FinishedFuzzRun();
		
		/// <summary>
		/// A fuzz run is about to be started
		/// </summary>
		void StartingFuzzRun();		
	}
}

