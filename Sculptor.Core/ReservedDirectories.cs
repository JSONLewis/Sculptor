using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sculptor.Core
{
    public class ReservedDirectories : IReservedDirectories
    {
        public const string OutputDirectoryName = "public";
        public const string ContentDirectoryName = "content";

        public ReservedDirectories()
        {
            // TODO: update this so that it verifies actual relative paths - not just the
            // directory names as we may want to check nested directories.
            DirectoryNames = new ReadOnlyCollection<string>(
                new string[]
                {
                    OutputDirectoryName,
                    ContentDirectoryName
                });
        }

        public IReadOnlyCollection<string> DirectoryNames { get; }

        public bool IsDirectoryNameReserved(string directoryName)
        {
            if (string.IsNullOrEmpty(directoryName))
                throw new FormatException($"The value of {nameof(directoryName)} cannot be null or empty.");

            return DirectoryNames.Contains(directoryName, StringComparer.OrdinalIgnoreCase);
        }
    }
}