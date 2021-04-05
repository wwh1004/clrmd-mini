using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace Example {
	internal static class Program {
		private static void Main() {
			using var dataTarget = DataTarget.CreateSnapshotAndAttach(Process.GetCurrentProcess().Id);
			dataTarget.EnumerateModules();
			foreach (var clrModule in dataTarget.ClrVersions.Select(t => t.CreateRuntime()).SelectMany(t => t.AppDomains).SelectMany(t => t.Modules)) {
				if (clrModule.IsDynamic)
					continue;

				string name = clrModule.Name;
				bool inMemory;
				if (!string.IsNullOrEmpty(name)) {
					inMemory = name.Contains(",");
				}
				else {
					name = "<<EmptyName>>";
					inMemory = true;
				}
				string moduleName = !inMemory ? Path.GetFileName(name) : name.Split(',')[0];
				Console.WriteLine(moduleName);
				// Name
				Console.WriteLine(clrModule.AppDomain.Name);
				// Domain Name
				Console.WriteLine(clrModule.AppDomain.Runtime.ClrInfo.Version.ToString());
				// CLR Version
				Console.WriteLine("0x" + clrModule.ImageBase.ToString(IntPtr.Size == 8 ? "X16" : "X8"));
				// ImageBase
				Console.WriteLine("0x" + clrModule.Size.ToString("X8"));
				// Size
				Console.WriteLine("0x" + clrModule.MetadataAddress.ToString(IntPtr.Size == 8 ? "X16" : "X8"));
				// MetadataAddress
				Console.WriteLine("0x" + clrModule.MetadataLength.ToString(IntPtr.Size == 8 ? "X16" : "X8"));
				// MetadataLength
				Console.WriteLine(!inMemory ? name : "InMemory");
				// Path
				Console.WriteLine();
			}
			Console.ReadKey(true);
		}
	}
}
