using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;

namespace Sculptor.Core.Domain.Create
{
    public class ContentComposer : IContentComposer
    {
        private readonly IFileSystem _fileSystem;

        public ContentComposer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Compose(in string projectRootPath)
        {
            string contentPath = _fileSystem.Path.Combine(
                projectRootPath,
                ReservedDirectories.ContentDirectoryName);

            _fileSystem.Directory.CreateDirectory(contentPath);

            // TODO: change the extension to `.md`
            string indexPagePath = _fileSystem.Path.Combine(contentPath, "index.html");

            // TODO: replace this with markdown and have a conversion layer that
            // translates it and shoves the result in the output directory.
            byte[] indexPageContent = Encoding
                .UTF8
                .GetBytes($"<html><body><h1>Site Created on: {DateTime.UtcNow}</h1></body></html>");

            using (var stream = _fileSystem.FileStream.Create(indexPagePath, FileMode.Create))
            using (var memoryStream = new MemoryStream(indexPageContent))
            {
                memoryStream.CopyTo(stream);
                stream.Flush();
                stream.Close();
            }
        }
    }
}