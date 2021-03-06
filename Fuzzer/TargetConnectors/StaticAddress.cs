// StaticAddress.cs
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
	/// <summary>
	/// Just returns the specified address
	/// </summary>
	public class StaticAddress : IAddressSpecifier, IAllocatedMemory
	{
		private UInt64? _address;
		private UInt64? _size = null;
		
		public StaticAddress (UInt64? address)
		{
			_address = address;
		}
		
		public StaticAddress (UInt64? address, UInt64? size)
			:this(address)
		{
			_size = size;
		}
	
		#region IAllocatedMemory implementation
		public ulong Address 
		{
			get { return _address.Value; }
		}
		#endregion

		#region IAllocatedMemory implementation
		public UInt64? Size
		{
			get { return _size; }
		}
		#endregion

		#region IAddressSpecifier implementation
		public ulong? ResolveAddress() 
		{
			return _address;
		}
		#endregion
}
}

