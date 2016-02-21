using System;
using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf
{
    public class Photo : Entity
    {
        public virtual string PhotoName { get; set; }
        public virtual string FileName { get; set; }
        public virtual byte[] FileData { get; set; }
        public virtual DateTime? FileTimeStamp { get; set; }
        public virtual FileType FileType { get; set; }
        public virtual string FileURL { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        //public virtual IList<PersonPhoto> PersonPhotos { get; set; }
        //public virtual IList<Organization> Organizations { get; set; }

        public Photo()
        {
            //this.PersonPhotos = new List<PersonPhoto>();
        }

        public override string ToString()
        {
            return this.PhotoName;
        }
    }
}
