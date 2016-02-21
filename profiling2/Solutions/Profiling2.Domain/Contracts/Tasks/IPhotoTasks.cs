using System.Collections.Generic;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IPhotoTasks
    {
        Photo GetPhoto(int id);

        Photo GetPhoto(string name);

        Photo SavePhoto(Photo photo);

        Photo SavePersonPhoto(Person person, Photo photo);

        void DeletePersonPhoto(Person person, Photo photo);

        IList<FileType> GetFileTypes();

        FileType GetFileType(int id);
    }
}
