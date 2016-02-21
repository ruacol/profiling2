using System;

namespace Profiling2.Domain.DTO
{
    public class SourceFolderDTO
    {
        public string Folder { get; set; }
        public string ParentFolder { get; set; }
        public string OwnerCode { get; set; }
        public int NumFiles { get; set; }
        public DateTime? LatestFileDate { get; set; }

        public SourceFolderDTO() { }

        public SourceFolderDTO(string folder)
        {
            if (!string.IsNullOrEmpty(folder))
            {
                this.Folder = folder;

                int i = folder.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                if (i > -1)
                    this.ParentFolder = folder.Substring(0, i);
            }
        }
    }
}
