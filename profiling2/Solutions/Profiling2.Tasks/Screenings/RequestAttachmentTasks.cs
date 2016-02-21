using System;
using System.IO;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Attach;
using Profiling2.Infrastructure.Util;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks.Screenings
{
    public class RequestAttachmentTasks : IRequestAttachmentTasks
    {
        private readonly IUserTasks userTasks;
        private readonly INHibernateRepository<Request> requestRepository;

        private readonly INHibernateRepository<RequestAttachmentStatus> requestAttachmentStatusRepository;
        private readonly INHibernateRepository<RequestAttachment> requestAttachmentRepository;
        private readonly INHibernateRepository<RequestAttachmentHistory> requestAttachmentHistoryRepository;
        private readonly INHibernateRepository<Attachment> attachmentRepository;

        public RequestAttachmentTasks(IUserTasks userTasks,
            INHibernateRepository<Request> requestRepository,
            INHibernateRepository<RequestEntity> requestEntityRepository,
            INHibernateRepository<RequestType> requestTypeRepository,
            INHibernateRepository<RequestStatus> requestStatusRepository,
            INHibernateRepository<RequestHistory> requestHistoryRepository,
            INHibernateRepository<RequestAttachmentStatus> requestAttachmentStatusRepository,
            INHibernateRepository<RequestAttachment> requestAttachmentRepository,
            INHibernateRepository<RequestAttachmentHistory> requestAttachmentHistoryRepository,
            INHibernateRepository<Attachment> attachmentRepository)
        {
            this.userTasks = userTasks;
            this.requestRepository = requestRepository;
            this.requestAttachmentStatusRepository = requestAttachmentStatusRepository;
            this.requestAttachmentRepository = requestAttachmentRepository;
            this.requestAttachmentHistoryRepository = requestAttachmentHistoryRepository;
            this.attachmentRepository = attachmentRepository;
        }

        public Attachment GetAttachment(int attachmentId)
        {
            return this.attachmentRepository.Get(attachmentId);
        }

        public void ArchiveAttachment(int requestId, int attachmentId, string username)
        {
            Request request = this.requestRepository.Get(requestId);
            if (request != null)
            {
                foreach (RequestAttachment ra in request.RequestAttachments)
                {
                    if (attachmentId == ra.Id)
                    {
                        ra.Archive = true;
                        RequestAttachmentHistory rah = new RequestAttachmentHistory();
                        rah.AdminUser = this.userTasks.GetAdminUser(username);
                        rah.DateStatusReached = DateTime.Now;
                        rah.RequestAttachmentStatus = this.requestAttachmentStatusRepository.Get(RequestAttachmentStatus.REMOVED);
                        rah.RequestAttachment = ra;
                        this.requestAttachmentHistoryRepository.Save(rah);
                    }
                }
            }
        }

        public void RestoreAttachment(int requestId, int attachmentId, string username)
        {
            Request request = this.requestRepository.Get(requestId);
            if (request != null)
            {
                foreach (RequestAttachment ra in request.RequestAttachments)
                {
                    if (attachmentId == ra.Id)
                    {
                        ra.Archive = false;
                        RequestAttachmentHistory rah = new RequestAttachmentHistory();
                        rah.AdminUser = this.userTasks.GetAdminUser(username);
                        rah.DateStatusReached = DateTime.Now;
                        rah.RequestAttachmentStatus = this.requestAttachmentStatusRepository.Get(RequestAttachmentStatus.ADDED);
                        rah.RequestAttachment = ra;
                        this.requestAttachmentHistoryRepository.Save(rah);
                    }
                }
            }
        }

        public void AddAttachment(int requestId, string fileName, Stream stream, string username)
        {
            Request request = this.requestRepository.Get(requestId);
            if (request != null)
            {
                if (stream != null)
                {
                    AdminUser user = this.userTasks.GetAdminUser(username);

                    // create Attachment object
                    Attachment att = new Attachment();
                    MemoryStream ms = new MemoryStream((int)stream.Length);
                    stream.CopyTo(ms);
                    att.FileData = ms.ToArray();
                    att.FileName = fileName;
                    att.FileExtension = FileUtil.GetExtension(fileName);
                    att.UploadedDateTime = DateTime.Now;
                    att.UploadedByAdminUser = user;
                    att = this.attachmentRepository.Save(att);

                    // attach to Request
                    RequestAttachment ra = new RequestAttachment();
                    ra.Request = request;
                    ra.Attachment = att;
                    ra = this.requestAttachmentRepository.Save(ra);

                    // create History object
                    RequestAttachmentHistory rah = new RequestAttachmentHistory();
                    rah.AdminUser = user;
                    rah.DateStatusReached = DateTime.Now;
                    rah.RequestAttachmentStatus = this.requestAttachmentStatusRepository.Get(RequestAttachmentStatus.ADDED);
                    rah.RequestAttachment = ra;
                    this.requestAttachmentHistoryRepository.Save(rah);
                }
            }
        }
    }
}
