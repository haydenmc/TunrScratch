using System;
using System.IO;
using System.Threading.Tasks;

namespace Tunr.Services
{
    public interface IMusicFileStore
    {
        Task PutFileAsync(Guid id, Stream fileStream);
        Task<Stream> GetFileStreamAsync(Guid id);
        Task<Uri> GetFileLocationAsync(Guid id);
    }
}