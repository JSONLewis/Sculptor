using System.Collections.Generic;

namespace Sculptor.Core
{
    public interface IReservedDirectories
    {
        /// <summary>
        /// Every directory reserved by Sculptor for internal use. Used to prevent users
        /// from creating directory structures that will clash with our implementation.
        /// </summary>
        IReadOnlyCollection<string> DirectoryNames { get; }

        /// <summary>
        /// Validates if the provided string exists in <see cref="DirectoryNames"/>.
        /// </summary>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        /// <exception cref="T:System.FormatException">
        /// Thrown if a null or empty value is provided in <paramref name="directoryName"/>.
        /// </exception>
        bool IsDirectoryNameReserved(string directoryName);
    }
}