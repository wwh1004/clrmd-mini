// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Diagnostics.Runtime {
	/// <summary>
	/// Provides information about loaded modules in a <see cref="DataTarget"/>.
	/// </summary>
	public sealed class ModuleInfo {
		private byte[] _buildId;
		private VersionInfo? _version;
		private readonly bool _isVirtual;
		private readonly IDataReader _dataReader;
		private DataTarget _dataTarget;

		/// <summary>
		/// The DataTarget which contains this module.
		/// </summary>
		[Obsolete("This will be completely removed in the next minor release.  Only DataReader is supported.")]
		public DataTarget DataTarget {
			get => _dataTarget;
			internal set => _dataTarget = value;
		}

		public IDataReader DataReader => _dataReader ?? _dataTarget.DataReader;

		/// <summary>
		/// Gets the base address of the object.
		/// </summary>
		public ulong ImageBase { get; }

		/// <summary>
		/// Gets the specific file size of the image used to index it on the symbol server.
		/// </summary>
		public int IndexFileSize { get; }

		/// <summary>
		/// Gets the timestamp of the image used to index it on the symbol server.
		/// </summary>
		public int IndexTimeStamp { get; }

		/// <summary>
		/// Gets the file name of the module on disk.
		/// </summary>
		public string? FileName { get; }

		/// <summary>
		/// Gets the Linux BuildId of this module.  This will be <see langword="null"/> if the module does not have a BuildId.
		/// </summary>
		public byte[] BuildId {
			get {
				if (_buildId is null) {
					return _buildId = DataReader.GetBuildId(ImageBase);
				}

				return _buildId;
			}
		}

		public override string? ToString() {
			return FileName;
		}

		/// <summary>
		/// Gets the version information for this file.
		/// </summary>
		public VersionInfo Version {
			get {
				if (_version.HasValue)
					return _version.Value;

				_version = DataReader.GetVersionInfo(ImageBase, out var version) ? version : default;
				return version;
			}
		}

		// DataTarget is one of the few "internal set" properties, and is initialized as soon as DataTarget asks
		// IDataReader to create ModuleInfo.  So even though we don't set it here, we will immediately set the
		// value to non-null and never change it.

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="imageBase">The base of the image as loaded into the virtual address space.</param>
		/// <param name="fileName">The full path of the file as loaded from disk (if possible), otherwise only the filename.</param>
		/// <param name="isVirtual">Whether this image is mapped into the virtual address space.  (This is as opposed to a memmap'ed file.)</param>
		/// <param name="indexFileSize">The index file size used by the symbol server to archive and request this binary.  Only for PEImages (not Elf or Mach-O binaries).</param>
		/// <param name="indexTimeStamp">The index timestamp used by the symbol server to archive and request this binary.  Only for PEImages (not Elf or Mach-O binaries).</param>
		/// <param name="buildId">The ELF buildid of this image.  Not valid for PEImages.</param>
		[Obsolete("Use the overload which specifies the DataReader")]
		public ModuleInfo(ulong imageBase, string? fileName, bool isVirtual, int indexFileSize, int indexTimeStamp, byte[] buildId) {
			ImageBase = imageBase;
			IndexFileSize = indexFileSize;
			IndexTimeStamp = indexTimeStamp;
			FileName = fileName;
			_isVirtual = isVirtual;
			_buildId = buildId;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reader">The <see cref="IDataReader"/> containing this module.</param>
		/// <param name="imageBase">The base of the image as loaded into the virtual address space.</param>
		/// <param name="fileName">The full path of the file as loaded from disk (if possible), otherwise only the filename.</param>
		/// <param name="isVirtual">Whether this image is mapped into the virtual address space.  (This is as opposed to a memmap'ed file.)</param>
		/// <param name="indexFileSize">The index file size used by the symbol server to archive and request this binary.  Only for PEImages (not Elf or Mach-O binaries).</param>
		/// <param name="indexTimeStamp">The index timestamp used by the symbol server to archive and request this binary.  Only for PEImages (not Elf or Mach-O binaries).</param>
		/// <param name="buildId">The ELF buildid of this image.  Not valid for PEImages.</param>
		public ModuleInfo(IDataReader reader, ulong imageBase, string? fileName, bool isVirtual, int indexFileSize, int indexTimeStamp, byte[] buildId) {
			_dataReader = reader;
			ImageBase = imageBase;
			IndexFileSize = indexFileSize;
			IndexTimeStamp = indexTimeStamp;
			FileName = fileName;
			_isVirtual = isVirtual;
			_buildId = buildId;
		}
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
	}
}
