// RemoteControlConnectionBuilder.cs
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
using System.Net.Sockets;
namespace Fuzzer.RemoteControl
{
	public static class RemoteControlConnectionBuilder
	{
		public static NetworkStream Connect (string address, int port)
		{
			TcpClient cli = new TcpClient ();
			cli.Connect (address, port);
			
			return cli.GetStream ();
		}
	}
}

