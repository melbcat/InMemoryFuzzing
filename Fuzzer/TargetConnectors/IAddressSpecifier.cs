// IAddressSpecifier.cs
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
namespace Fuzzer.TargetConnectors
{
	public interface IAddressSpecifier
	{
		/// <summary>
		/// Resolves the address.
		/// The value of the resolved address may change during the lifetime of the program
		/// </summary>
		/// <returns>A <see cref="UInt64"/></returns>
		//UInt64? Address{ get; }
		
		UInt64? ResolveAddress();
	}
}

