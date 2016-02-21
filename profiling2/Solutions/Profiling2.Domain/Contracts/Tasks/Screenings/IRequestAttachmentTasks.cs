using System.IO;
using Profiling2.Domain.Scr.Attach;

namespace Profiling2.Domain.Contracts.Tasks.Screenings
{
    public interface IRequestAttachmentTasks
    {
        Attachment GetAttachment(int attachmentId);

        void ArchiveAttachment(int requestId, int attachmentId, string username);

        void RestoreAttachment(int requestId, int attachmentId, string username);

        void AddAttachment(int requestId, string fileName, Stream stream, string username);
    }
}
