using System.Collections.Generic;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks
{
    public class PhotoTasks : IPhotoTasks
    {
        protected readonly INHibernateRepository<Photo> photoRepo;
        protected readonly INHibernateRepository<FileType> fileTypeRepo;
        protected readonly INHibernateRepository<PersonPhoto> personPhotoRepo;
        protected readonly IPersonTasks personTasks;

        public PhotoTasks(INHibernateRepository<Photo> photoRepo, 
            INHibernateRepository<FileType> fileTypeRepo,
            INHibernateRepository<PersonPhoto> personPhotoRepo,
            IPersonTasks personTasks)
        {
            this.photoRepo = photoRepo;
            this.fileTypeRepo = fileTypeRepo;
            this.personPhotoRepo = personPhotoRepo;
            this.personTasks = personTasks;
        }

        public Photo GetPhoto(int id)
        {
            return this.photoRepo.Get(id);
        }

        public Photo GetPhoto(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("PhotoName", name);
            IList<Photo> photos = this.photoRepo.FindAll(criteria);
            if (photos != null && photos.Count > 0)
                return photos[0];
            return null;
        }

        public Photo SavePhoto(Photo photo)
        {
            return this.photoRepo.SaveOrUpdate(photo);
        }

        public Photo SavePersonPhoto(Person person, Photo photo)
        {
            person.AddPhoto(photo);
            if (!person.HasValidProfileStatus())
            {
                person.ProfileStatus = this.personTasks.GetProfileStatus(ProfileStatus.ROUGH_OUTLINE);
                this.personTasks.SavePerson(person);
            }
            return this.photoRepo.SaveOrUpdate(photo);
        }

        public void DeletePersonPhoto(Person person, Photo photo)
        {
            person.RemovePhoto(photo);
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Person", person);
            criteria.Add("Photo", photo);
            foreach (PersonPhoto pp in this.personPhotoRepo.FindAll(criteria))
                this.personPhotoRepo.Delete(pp);
            this.photoRepo.Delete(photo);
        }

        public IList<FileType> GetFileTypes()
        {
            return this.fileTypeRepo.GetAll();
        }

        public FileType GetFileType(int id)
        {
            return this.fileTypeRepo.Get(id);
        }
    }
}
