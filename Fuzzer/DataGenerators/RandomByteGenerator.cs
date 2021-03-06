// RandomByteGenerator.cs
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
using System.IO;
using Fuzzer.DataLoggers;
using System.Collections.Generic;
using Iaik.Utils;
using Iaik.Utils.CommonAttributes;
namespace Fuzzer.DataGenerators
{
	[ClassIdentifier("datagen/random_bytes")]
	public class RandomByteGenerator : IDataGenerator
	{
		public enum ByteType
		{
			/// <summary>
			/// All bytes are allowed
			/// </summary>
			All,
			
			/// <summary>
			/// Only printable ascii chars 
			/// </summary>
			PrintableASCII,
			
			/// <summary>
			/// Combination of PrintableASCII and AllNullTerminated
			/// </summary>
			PrintableASCIINullTerminated,
			
			/// <summary>
			/// All characters are valid, except the null char, it only appears as the last character
			/// </summary>
			AllNullTerminated
		}
		
		private int _minLen;
		private int _maxLen;
		private ByteType _byteType;
		private byte[] _buffer = null;
		private Random _r = new Random();
		private DataGeneratorLogger _logger;
		private IDataGeneratorLenType _lenType = null;
		
		public RandomByteGenerator (int minLen, int maxLen, ByteType byteType, IDataGeneratorLenType lenType)
		{
			Init(minLen, maxLen, byteType, lenType);
			
		}
		
		/// <summary>
		/// Call the setup method to initialize the generator
		/// </summary>
		public RandomByteGenerator()
		{
			
		}
	
		private void Init (int minLen, int maxLen, ByteType byteType, string lenType)
		{
			_minLen = minLen;
			_maxLen = maxLen;
			_byteType = byteType;
			if (_minLen == _maxLen)
				_buffer = new byte[_minLen];
			
			_lenType = DataGeneratorLenTypeFactory.Create (minLen, maxLen, lenType);
		}
		
		private void Init (int minLen, int maxLen, ByteType byteType, IDataGeneratorLenType lenType)
		{
			_minLen = minLen;
			_maxLen = maxLen;
			_byteType = byteType;
			if (_minLen == _maxLen)
				_buffer = new byte[_minLen];
			
			_lenType = lenType;
		}

		#region IDataGenerator implementation
		public void Setup (IDictionary<string, string> config)
		{
			Init (
			     DictionaryHelper.GetInt ("minlen", config, 1),
			     DictionaryHelper.GetInt ("maxlen", config, 100),
			     DictionaryHelper.GetEnum<ByteType>("type", config, ByteType.PrintableASCIINullTerminated),
				 DictionaryHelper.GetString("lentype", config, null)
			     );
			     
		}
		
		public void SetLogger(DataGeneratorLogger logger)
		{
			_logger = logger;
		}
		
		public byte[] GenerateData ()
		{
			if (_buffer != null)
			{
				GenerateBytes (_buffer);
				
				if (_logger != null)
					_logger.LogData (_buffer);
				
				return _buffer;
			}
			else
			{
				int byteLen = _lenType.NextLength ();
				byte[] buffer = new byte[byteLen];
				GenerateBytes (buffer);
				
				if(_logger != null)
					_logger.LogData (buffer);
				
				return buffer;
			}
				
		}
		#endregion
		
		private void GenerateBytes (byte[] data)
		{
			if (_byteType == ByteType.All)
				_r.NextBytes (data);
			else if (_byteType == ByteType.PrintableASCII || _byteType == RandomByteGenerator.ByteType.PrintableASCIINullTerminated)
			{
				for (int i = 0; i < data.Length; i++)
					data[i] = (byte)_r.Next (0x21, 0x7e);
				
				if (_byteType == RandomByteGenerator.ByteType.PrintableASCIINullTerminated)
					data[data.Length - 1] = 0;
			}
			else if (_byteType == ByteType.AllNullTerminated) 
			{
				for (int i = 0; i < data.Length - 1; i++)
					data[i] = (byte)_r.Next (0x01, 0xff);
				data[data.Length - 1] = 0;
			}
		}
	}
}

