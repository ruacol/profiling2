namespace Profiling2.Domain.Prf.Sources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NHibernate.Envers.Configuration.Attributes;
    using Profiling2.Domain.Prf.Events;
    using Profiling2.Domain.Prf.Organizations;
    using Profiling2.Domain.Prf.Persons;
    using Profiling2.Domain.Prf.Units;
    using SharpArch.Domain.DomainModel;

    /// <summary>
    /// Principal entity for storing binary contents of documents.  There are two binary columns: FileData and OriginalFileData,
    /// for those sources which have undergone OCR.
    /// 
    /// Loading this entity can in some cases fail if the binary data is large enough (NHibernate is not meant to handle this use case).
    /// Rather than set the binary attributes as lazy, which caused problems with Envers auditing, there exists SourceDTO which in
    /// most cases satisifies what is needed without having to load the binary contents of a Source into memory.
    /// </summary>
    public class Source : Entity
    {
        /// <summary>
        /// This list is replicated in the console utility DocumentImportConsole's config file, used when importing sources from file shares.
        /// </summary>
        public static string[] IGNORED_FILE_EXTENSIONS = { "avi", "mpg", "m4a", "mov", "mp3", "wmv", "db", "sql", "dll", "exe", "lnk", "tmp", "ini", 
                "onet",  // this should be 'onetoc2', but table column stores it as 'onet' since it only stores 4 bytes
                "nsf" };

        public static string[] IMAGE_FILE_EXTENSIONS = { "jpg", "jpeg", "png", "bmp", "gif", "tiff" };

        /// <summary>
        /// The file name, including file extension.
        /// </summary>
        public virtual string SourceName { get; set; }

        /// <summary>
        /// Seems to be defunct.
        /// </summary>
        public virtual string FullReference { get; set; }

        /// <summary>
        /// The full path to the original location from which this Source was imported.  Includes the file name.
        /// </summary>
        public virtual string SourcePath { get; set; }

        /// <summary>
        /// The date when this source was imported.
        /// </summary>
        public virtual DateTime? SourceDate { get; set; }

        /// <summary>
        /// Once archived, a Source doesn't appear in searches.  It stays in the database as its SourcePath is still useful in preventing
        /// the source from being automatically re-imported.
        /// </summary>
        [Audited]
        public virtual Boolean Archive { get; set; }

        public virtual string Notes { get; set; }

        /// <summary>
        /// Used by the MSSQL full-text indexer to determine which text filter to use when indexing; for OCR'd documents for example, it's value should be txt.
        /// It's recommended to use FileUtil.GetExtension() or Path.GetExtension() on the SourceName instead to get an OCR'd file's true extension.
        /// </summary>
        public virtual string FileExtension { get; set; }

        /// <summary>
        /// MSSQL's full-text indexer can be customised based on language, but this has not been exploited: this field isn't really used.
        /// </summary>
        public virtual Language FileLanguage { get; set; }

        /// <summary>
        /// Binary contents of the file.  Target of full-text indexer (both MSSQL and Lucene).
        /// </summary>
        public virtual byte[] FileData { get; set; }

        /// <summary>
        /// A classification level above those sources for which this is not true.
        /// </summary>
        public virtual Boolean IsRestricted { get; set; }

        /// <summary>
        /// The Last Modified Date of the file at the time of import.
        /// </summary>
        public virtual DateTime? FileDateTimeStamp { get; set; }

        /// <summary>
        /// Binary contents of the file if it has undergone OCR.  Not indexed.
        /// </summary>
        public virtual byte[] OriginalFileData { get; set; }

        /// <summary>
        /// JHRO case number, if detected to be a JHRO case or one of its related attachments.
        /// </summary>
        public virtual JhroCase JhroCase { get; set; }

        /// <summary>
        /// Designates extra warning levels to the user in the interface, but is not a classification level:
        /// Sources marked 'read only' are generally a subset of those marked 'restricted'.
        /// </summary>
        public virtual bool IsReadOnly { get; set; }

        /// <summary>
        /// MD5 hash of the file's binary contents (according to the SQL Server function master.sys.fn_repl_hash_binary).
        /// Populated by insert trigger.  See Profiling2.Migrations.Migrations.PopulateSourceHash.cs for the initial populate SQL
        /// (large Sources were not hashed).
        /// 
        /// NOTE: Another algorithm may have been used that allowed for better collision detection (i.e. different files generating
        /// the same hash), but MD5 proved simpler and faster to implement based on the convenience of master.sys.fn_repl_hash_binary.
        /// </summary>
        public virtual byte[] Hash { get; set; }

        public virtual bool IsPublic { get; set; }

        /// <summary>
        /// Log of when this Source was originally imported, if exists.
        /// </summary>
        public virtual IList<AdminSourceImport> AdminSourceImports { get; set; }

        /// <summary>
        /// Log of every instance where this Source has appeared as a search result, and the actions by the user thereafter.
        /// </summary>
        public virtual IList<AdminReviewedSource> AdminReviewedSources { get; set; }

        public virtual IList<PersonSource> PersonSources { get; set; }

        public virtual IList<OrganizationSource> OrganizationSources { get; set; }

        public virtual IList<EventSource> EventSources { get; set; }

        public virtual IList<UnitSource> UnitSources { get; set; }

        public virtual IList<OperationSource> OperationSources { get; set; }

        public virtual IList<SourceRelationship> SourceRelationshipsAsParent { get; set; }

        public virtual IList<SourceRelationship> SourceRelationshipsAsChild { get; set; }

        /// <summary>
        /// Before a file becomes a Source, it may be uploaded for approval as a FeedingSource.  There should only be 
        /// one FeedingSource per Source.  The FeedingSource contains useful attributes like UploadedBy and ApprovedBy.
        /// </summary>
        public virtual IList<FeedingSource> FeedingSources { get; set; }

        public virtual IList<SourceAuthor> SourceAuthors { get; set; }

        public virtual IList<SourceOwningEntity> SourceOwningEntities { get; set; }

        public Source()
        {
            this.AdminSourceImports = new List<AdminSourceImport>();
            this.AdminReviewedSources = new List<AdminReviewedSource>();
            this.PersonSources = new List<PersonSource>();
            this.EventSources = new List<EventSource>();
            this.OrganizationSources = new List<OrganizationSource>();
            this.UnitSources = new List<UnitSource>();
            this.OperationSources = new List<OperationSource>();
            this.SourceRelationshipsAsParent = new List<SourceRelationship>();
            this.SourceRelationshipsAsChild = new List<SourceRelationship>();
            this.FeedingSources = new List<FeedingSource>();
            this.SourceAuthors = new List<SourceAuthor>();
            this.SourceOwningEntities = new List<SourceOwningEntity>();
        }

        public virtual bool HasOcrText()
        {
            return this.OriginalFileData != null && this.OriginalFileData.Length > 0;
        }

        public virtual Source GetParentSource()
        {
            if (this.SourceRelationshipsAsChild != null && this.SourceRelationshipsAsChild.Any())
                return this.SourceRelationshipsAsChild.First().ParentSource;
            return null;
        }

        public virtual IList<Source> GetChildSources()
        {
            if (this.SourceRelationshipsAsParent != null && this.SourceRelationshipsAsParent.Any())
                return this.SourceRelationshipsAsParent.Select(x => x.ChildSource).ToList<Source>();
            return null;
        }

        public virtual bool HasUploadedBy()
        {
            return this.FeedingSources != null && this.FeedingSources.Any() && this.FeedingSources.First().UploadedBy != null;
        }

        public virtual AdminUser GetUploadedBy()
        {
            if (this.HasUploadedBy())
                return this.FeedingSources.First().UploadedBy;
            return null;
        }

        /// <summary>
        /// Return SourcePath, but without the filename (not including directory separator).
        /// </summary>
        /// <returns></returns>
        public virtual string GetSourcePathOnly()
        {
            // SourcePath includes filename, we index only the folder.
            // Not using Path.GetDirectoryName since it has a char length limitation.
            if (!string.IsNullOrEmpty(this.SourcePath))
            {
                int i = this.SourcePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                if (i > -1)
                    return this.SourcePath.Substring(0, i);
            }
            return this.SourcePath;
        }

        /// <summary>
        /// Uploading user's User ID.  Only to be set when indexing, as the indexer uses a stateless session which cannot
        /// use traverse the attached collections referenced in HasUploadedBy() and GetUploadedBy().
        /// </summary>
        protected internal virtual string _uploadedByHorsSession { get; set; }
        public virtual string GetUploadedByHorsSession()
        {
            return this._uploadedByHorsSession;
        }
        public virtual void SetUploadedByHorsSession(string uploadedByUserId)
        {
            this._uploadedByHorsSession = uploadedByUserId;
        }

        protected internal virtual IList<string> _authorsHorsSession { get; set; }
        public virtual IList<string> GetAuthorsHorsSession()
        {
            return this._authorsHorsSession;
        }
        public virtual void SetAuthorsHorsSession(IList<string> authors)
        {
            this._authorsHorsSession = authors;
        }

        protected internal virtual IList<string> _ownersHorsSession { get; set; }
        public virtual IList<string> GetOwnersHorsSession()
        {
            return this._ownersHorsSession;
        }
        public virtual void SetOwnersHorsSession(IList<string> owners)
        {
            this._ownersHorsSession = owners;
        }

        protected internal virtual string _jhroCaseNumber { get; set; }
        public virtual string GetJhroCaseNnumberHorsSession()
        {
            return this._jhroCaseNumber;
        }
        public virtual void SetJhroCaseNumberHorsSession(string jhroCaseNumber)
        {
            this._jhroCaseNumber = jhroCaseNumber;
        }

        protected internal virtual long _fileSize { get; set; }
        public virtual long GetFileSizeHorsSession()
        {
            return this._fileSize;
        }
        public virtual void SetFileSizeHorsSession(long fileSize)
        {
            this._fileSize = fileSize;
        }

        public virtual PersonSource GetPersonSource(Person person)
        {
            IList<PersonSource> personSources = (from ps in this.PersonSources
                                                 where ps.Person.Id == person.Id && !ps.Archive
                                                 select ps).ToList<PersonSource>();
            return (personSources != null && personSources.Count > 0 ? personSources[0] : null);
        }

        public virtual void AddPersonSource(PersonSource ps)
        {
            if (this.PersonSources.Contains(ps))
                return;

            this.PersonSources.Add(ps);
        }

        public virtual void RemovePersonSource(PersonSource ps)
        {
            this.PersonSources.Remove(ps);
        }

        public virtual void AddEventSource(EventSource es)
        {
            if (this.EventSources.Contains(es))
                return;

            this.EventSources.Add(es);
        }

        public virtual EventSource GetEventSource(Event ev)
        {
            IList<EventSource> eventSources = (from es in this.EventSources
                                               where es.Event.Id == ev.Id && !es.Archive
                                               select es).ToList<EventSource>();
            return (eventSources != null && eventSources.Count > 0 ? eventSources[0] : null);
        }

        public virtual void RemoveEventSource(EventSource es)
        {
            this.EventSources.Remove(es);
        }

        public virtual void AddOrganizationSource(OrganizationSource os)
        {
            if (this.OrganizationSources.Contains(os))
                return;

            this.OrganizationSources.Add(os);
        }

        public virtual void RemoveOrganizationSource(OrganizationSource os)
        {
            this.OrganizationSources.Remove(os);
        }

        public virtual UnitSource GetUnitSource(Unit unit)
        {
            IList<UnitSource> unitSources = (from us in this.UnitSources
                                                 where us.Unit.Id == unit.Id && !us.Archive
                                             select us).ToList<UnitSource>();
            return (unitSources != null && unitSources.Count > 0 ? unitSources[0] : null);
        }

        public virtual void AddUnitSource(UnitSource us)
        {
            if (this.UnitSources.Contains(us))
                return;

            this.UnitSources.Add(us);
        }

        public virtual void RemoveUnitSource(UnitSource us)
        {
            this.UnitSources.Remove(us);
        }

        public virtual OperationSource GetOperationSource(Operation operation)
        {
            IList<OperationSource> operationSources = (from os in this.OperationSources
                                                       where os.Operation.Id == operation.Id && !os.Archive
                                                       select os).ToList<OperationSource>();
            return (operationSources != null && operationSources.Count > 0 ? operationSources[0] : null);
        }

        public virtual void AddOperationSource(OperationSource os)
        {
            if (this.OperationSources.Contains(os))
                return;

            this.OperationSources.Add(os);
        }

        public virtual void RemoveOperationSource(OperationSource os)
        {
            this.OperationSources.Remove(os);
        }

        public virtual void AddSourceAuthor(SourceAuthor sa)
        {
            if (this.SourceAuthors.Contains(sa))
                return;

            this.SourceAuthors.Add(sa);
        }

        public virtual void AddSourceOwningEntity(SourceOwningEntity e)
        {
            if (this.SourceOwningEntities.Contains(e))
                return;

            this.SourceOwningEntities.Add(e);
        }

        public virtual void AddFeedingSource(FeedingSource fs)
        {
            if (this.FeedingSources.Contains(fs))
                return;

            this.FeedingSources.Add(fs);
        }

        public override string ToString()
        {
            return this.SourceName + " (ID=" + this.Id + ")";
        }
    }
}