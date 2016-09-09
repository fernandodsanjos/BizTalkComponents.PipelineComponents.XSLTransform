//---------------------------------------------------------------------
// File: VirtualStream.cs
// 
// Summary: A sample pipeline component which demonstrates how to promote message context
//          properties and write distinguished fields for XML messages using arbitrary
//          XPath expressions.
//
// Sample: Arbitrary XPath Property Handler Pipeline Component SDK 
//
//---------------------------------------------------------------------
// This file is part of the Microsoft BizTalk Server SDK
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// This source code is intended only as a supplement to Microsoft BizTalk
// Server 2009 release and/or on-line documentation. See these other
// materials for detailed information regarding Microsoft code samples.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Samples.BizTalk.Pipelines.CustomComponent
{
	/// <summary>
	/// Implements a virtual stream, i.e. the always seekable stream which
	/// uses configurable amount of memory to reduce a memory footprint and 
	/// temporarily stores remaining data in a temporary file on disk.
	/// </summary>
	public sealed class VirtualStream : Stream, IDisposable
	{
		/// <summary>
		/// Memory handling.
		/// </summary>
		public enum MemoryFlag
		{
			AutoOverFlowToDisk	= 0,
			OnlyInMemory		= 1,
			OnlyToDisk			= 2
		}

		// Constants
		private const int				MemoryThreshold		= 4*1024*1024;		// The maximum possible memory consumption (4Mb)
		private const int				DefaultMemorySize	= 4*1024;			// Default memory consumption (4Kb)

		private Stream					wrappedStream;
		private bool					isDisposed;
		private bool					isInMemory;
		private int						thresholdSize;
		private MemoryFlag				memoryStatus;

		/// <summary>
		/// Initializes a VirtualStream instance with default parameters (10K memory buffer,
		/// allow overflow to disk).
		/// </summary>
		public VirtualStream() 
			: this(DefaultMemorySize, MemoryFlag.AutoOverFlowToDisk, new MemoryStream())
		{
		}

		/// <summary>
		/// Initializes a VirtualStream instance with memory buffer size.
		/// </summary>
		/// <param name="bufferSize">Memory buffer size</param>
		public VirtualStream(int bufferSize) 
			: this(bufferSize, MemoryFlag.AutoOverFlowToDisk, new MemoryStream(bufferSize))
		{
		}

		/// <summary>
		/// Initializes a VirtualStream instance with a default memory size and memory flag specified.
		/// </summary>
		/// <param name="flag">Memory flag</param>
		public VirtualStream(MemoryFlag flag)
			: this(DefaultMemorySize, flag,
			(flag == MemoryFlag.OnlyToDisk) ? CreatePersistentStream() : new MemoryStream())
		{
		}

		/// <summary>
		/// Initializes a VirtualStream instance with a memory buffer size and memory flag specified.
		/// </summary>
		/// <param name="bufferSize">Memory buffer size</param>
		/// <param name="flag">Memory flag</param>
		public VirtualStream(int bufferSize, MemoryFlag flag)
			: this(bufferSize, flag,
			(flag == MemoryFlag.OnlyToDisk) ? CreatePersistentStream() : new MemoryStream(bufferSize))
		{
		}

		/// <summary>
		/// Initializes a VirtualStream instance with a memory buffer size, memory flag and underlying stream
		/// specified.
		/// </summary>
		/// <param name="bufferSize">Memory buffer size</param>
		/// <param name="flag">Memory flag</param>
		/// <param name="dataStream">Underlying stream</param>
		private VirtualStream(int bufferSize, MemoryFlag flag, Stream dataStream)
		{
			if (null == dataStream)
				throw new ArgumentNullException("dataStream");

			isInMemory = (flag != MemoryFlag.OnlyToDisk);
			memoryStatus = flag;
			bufferSize = Math.Min(bufferSize, MemoryThreshold);
			thresholdSize = bufferSize;

			if (isInMemory)
				wrappedStream = dataStream;  // Don't want to double wrap memory stream
			else
				wrappedStream = new BufferedStream(dataStream, bufferSize);
			isDisposed	= false;
		}

		#region Stream Methods and Properties

		/// <summary>
		/// Gets a flag indicating whether a stream can be read.
		/// </summary>
		override public bool CanRead
		{
			get {return wrappedStream.CanRead;}
		}
		/// <summary>
		/// Gets a flag indicating whether a stream can be written.
		/// </summary>
		override public bool CanWrite
		{
			get {return wrappedStream.CanWrite;}
		}
		/// <summary>
		/// Gets a flag indicating whether a stream can seek.
		/// </summary>
		override public bool CanSeek
		{
			get {return true;}
		}
		/// <summary>
		/// Returns the length of the source stream.
		/// <seealso cref="GetLength()"/>
		/// </summary>
		override public long Length
		{
			get {return wrappedStream.Length;}
		}

		/// <summary>
		/// Gets or sets a position in the stream.
		/// </summary>
		override public long Position
		{
			get {return wrappedStream.Position;}
			set {wrappedStream.Seek(value, SeekOrigin.Begin);}
		}

		/// <summary>
		/// <see cref="Stream.Close()"/>
		/// </summary>
		/// <remarks>
		/// Calling other methods after calling Close() may result in a ObjectDisposedException beeing throwed.
		/// </remarks>
		override public void Close()
		{
			if(!isDisposed)
			{
				GC.SuppressFinalize(this);
				Cleanup();
			}
		}

		/// <summary>
		/// <see cref="Stream.Flush()"/>
		/// </summary>
		/// <remarks>
		/// </remarks>
		override public void Flush()
		{
			ThrowIfDisposed();
			wrappedStream.Flush();
		}

		/// <summary>
		/// <see cref="Stream.Read()"/>
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns>
		/// The number of bytes read
		/// </returns>
		/// <remarks>
		/// May throw <see cref="ObjectDisposedException"/>.
		/// It will read from cached persistence stream
		/// </remarks>
		override public int Read(byte[] buffer, int offset, int count)
		{
			ThrowIfDisposed();
			return wrappedStream.Read(buffer, offset, count);
		}

		/// <summary>
		/// <see cref="Stream.Seek()"/>
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns>
		/// The current position
		/// </returns>
		/// <remarks>
		/// May throw <see cref="ObjectDisposedException"/>.
		/// It will cache any new data into the persistence stream
		/// </remarks>
		override public long Seek(long offset, SeekOrigin origin)
		{
			ThrowIfDisposed();
			return wrappedStream.Seek(offset, origin);
		}

		/// <summary>
		/// <see cref="Stream.SetLength()"/>
		/// </summary>
		/// <param name="length"></param>
		/// <remarks>
		/// May throw <see cref="ObjectDisposedException"/>.
		/// </remarks>
		override public void SetLength(long length)
		{
			ThrowIfDisposed();

			// Check if new position is greater than allowed by threshold
			if (memoryStatus == MemoryFlag.AutoOverFlowToDisk && 
				isInMemory &&
				length > thresholdSize)
			{
				// Currently in memory, and the new write will push it over the limit
				// Switching to Persist Stream
				BufferedStream persistStream = new BufferedStream(CreatePersistentStream(), thresholdSize);

				// Copy current wrapped memory stream to the persist stream
				CopyStreamContent((MemoryStream)wrappedStream, persistStream);

				// Close old wrapped stream
				if (wrappedStream != null)
					wrappedStream.Close();

				wrappedStream = persistStream;
				isInMemory = false;
			}

			// Set new length for the wrapped stream
			wrappedStream.SetLength(length);
		}

		/// <summary>
		/// <see cref="Stream.Write()"/>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// </summary>
		/// <remarks>
		/// Write to the underlying stream.
		/// </remarks>
		override public void Write(byte[] buffer, int offset, int count)
		{			
			ThrowIfDisposed();

			// Check if new position after write is greater than allowed by threshold
			if (memoryStatus == MemoryFlag.AutoOverFlowToDisk && 
				isInMemory &&
				(count + wrappedStream.Position) > thresholdSize)
			{
				// Currently in memory, and the new write will push it over the limit
				// Switching to Persist Stream
				BufferedStream persistStream = new BufferedStream(CreatePersistentStream(), thresholdSize);

				// Copy current wrapped memory stream to the persist stream
				CopyStreamContent((MemoryStream) wrappedStream, persistStream);

				// Close old wrapped stream
				if (wrappedStream != null)
					wrappedStream.Close();

				wrappedStream = persistStream;
				isInMemory = false;
			}

			wrappedStream.Write(buffer, offset, count);
		}

		#endregion

		#region IDisposable Interface

		/// <summary>
		/// <see cref="IDisposeable.Dispose()"/>
		/// </summary>
		/// <remarks>
		/// It will call <see cref="Close()"/>
		/// </remarks>
		public void Dispose()
		{
			Close();
		}

		#endregion

		#region Private Utility Functions

		/// <summary>
		/// Utility method called by the Finalize(), Close() or Dispose() to close and release
		/// both the source and the persistence stream.
		/// </summary>
		private void Cleanup()
		{
			if(!isDisposed)
			{
				isDisposed = true;
				if(null != wrappedStream)
				{
					wrappedStream.Close();
					wrappedStream = null;
				}
			}
		}

		/// <summary>
		/// Copies source memory stream to the target stream.
		/// </summary>
		/// <param name="source">Source memory stream</param>
		/// <param name="target">Target stream</param>
		private void CopyStreamContent(MemoryStream source, Stream target)
		{
			// Remember position for the source stream
			long currentPosition = source.Position;

			// Read and write in chunks each thresholdSize
			byte[] tempBuffer = new Byte[thresholdSize];
			int read = 0;
		
			source.Position = 0;
			while ((read = source.Read(tempBuffer, 0, tempBuffer.Length)) != 0)
				target.Write(tempBuffer, 0, read);

			// Set target's stream position to be the same as was in source stream. This is required because 
			// target stream is going substitute source stream.
			target.Position = currentPosition;

			// Restore source stream's position (just in case to preserve the source stream's state)
			source.Position = currentPosition;
		}

		/// <summary>
		/// Called by other methods to check the stream state.
		/// It will thorw <see cref="ObjectDisposedException"/> if the stream was closed or disposed.
		/// </summary>
		private void ThrowIfDisposed()
		{
			if(isDisposed || null == wrappedStream)
				throw new ObjectDisposedException("VirtualStream");
		}

		/// <summary>
		/// Utility method.
		/// Creates a FileStream with a unique name and the temporary and delete-on-close attributes.
		/// </summary>
		/// <returns>
		/// The temporary persistence stream
		/// </returns>
		public static Stream CreatePersistentStream()
		{
			StringBuilder name = new StringBuilder(256);
			
			IntPtr handle;
			if(0 == GetTempFileName(Path.GetTempPath(), "BTS", 0, name))
				throw new IOException("GetTempFileName Failed.", Marshal.GetHRForLastWin32Error());

			handle = CreateFile(name.ToString(), (UInt32) FileAccess.ReadWrite, 0, IntPtr.Zero, (UInt32) FileMode.Create, 0x04000100, IntPtr.Zero);
			
			// FileStream constructor will throw exception if handle is zero or -1.
			return new FileStream(new SafeFileHandle(handle, true), FileAccess.ReadWrite);
		}

		[DllImport("kernel32.dll")]
		private static extern UInt32 GetTempFileName
			(
			string	path,
			string	prefix,
			UInt32	unique,
			StringBuilder	name
			);

		[DllImport("kernel32.dll")]
		private static extern IntPtr CreateFile
			(
			string	name,
			UInt32	accessMode,
			UInt32	shareMode,
			IntPtr	security,
			UInt32	createMode,
			UInt32	flags,
			IntPtr	template
			);

		#endregion
	}
}
