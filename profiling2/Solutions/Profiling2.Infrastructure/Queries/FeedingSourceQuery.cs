using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class FeedingSourceQuery : NHibernateQuery, IFeedingSourceQuery
    {
        public IList<FeedingSourceDTO> GetFeedingSourceDTOs(bool canViewAndSearchAll, bool includeRestricted, string uploadedByUserId)
        {
            FeedingSourceDTO output = null;
            FeedingSource feedingSourceAlias = null;
            AdminUser uploadedByAlias = null;
            AdminUser approvedByAlias = null;
            AdminUser rejectedByAlias = null;
            Source sourceAlias = null;
            PersonSource personSourceAlias = null;
            EventSource eventSourceAlias = null;
            UnitSource unitSourceAlias = null;
            OperationSource operationSourceAlias = null;

            var q = Session.QueryOver<FeedingSource>(() => feedingSourceAlias)
                .JoinAlias(() => feedingSourceAlias.UploadedBy, () => uploadedByAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => feedingSourceAlias.ApprovedBy, () => approvedByAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => feedingSourceAlias.RejectedBy, () => rejectedByAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => feedingSourceAlias.Source, () => sourceAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => sourceAlias.PersonSources, () => personSourceAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => sourceAlias.EventSources, () => eventSourceAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => sourceAlias.UnitSources, () => unitSourceAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => sourceAlias.OperationSources, () => operationSourceAlias, JoinType.LeftOuterJoin)
                .SelectList(list => list
                    .Select(Projections.Group(() => feedingSourceAlias.Id)).WithAlias(() => output.Id)
                    .Select(Projections.Group(() => feedingSourceAlias.Name)).WithAlias(() => output.Name)
                    .Select(Projections.Group(() => feedingSourceAlias.Restricted)).WithAlias(() => output.Restricted)
                    .Select(Projections.Group(() => feedingSourceAlias.FileModifiedDateTime)).WithAlias(() => output.FileModifiedDateTime)
                    .Select(Projections.Group(() => uploadedByAlias.UserName)).WithAlias(() => output.UploadedBy)
                    .Select(Projections.Group(() => feedingSourceAlias.UploadDate)).WithAlias(() => output.UploadDate)
                    .Select(Projections.Group(() => approvedByAlias.UserName)).WithAlias(() => output.ApprovedBy)
                    .Select(Projections.Group(() => feedingSourceAlias.ApprovedDate)).WithAlias(() => output.ApproveDate)
                    .Select(Projections.Group(() => rejectedByAlias.UserName)).WithAlias(() => output.RejectedBy)
                    .Select(Projections.Group(() => feedingSourceAlias.RejectedDate)).WithAlias(() => output.RejectedDate)
                    .Select(Projections.Group(() => feedingSourceAlias.RejectedReason)).WithAlias(() => output.RejectedReason)
                    .Select(Projections.Group(() => feedingSourceAlias.UploadNotes)).WithAlias(() => output.UploadNotes)
                    .Select(Projections.Group(() => feedingSourceAlias.Source.Id)).WithAlias(() => output.SourceID)
                    .Select(Projections.Group(() => feedingSourceAlias.IsReadOnly)).WithAlias(() => output.IsReadOnly)
                    .Select(Projections.Group(() => feedingSourceAlias.IsPublic)).WithAlias(() => output.IsPublic)
                    .Select(Projections.Count(() => personSourceAlias.Person.Id)).WithAlias(() => output.PersonSourceCount)
                    .Select(Projections.Count(() => eventSourceAlias.Event.Id)).WithAlias(() => output.EventSourceCount)
                    .Select(Projections.Count(() => unitSourceAlias.Unit.Id)).WithAlias(() => output.UnitSourceCount)
                    .Select(Projections.Count(() => operationSourceAlias.Operation.Id)).WithAlias(() => output.OperationSourceCount)
                    );

            if (canViewAndSearchAll)
            {
                if (!includeRestricted)
                {
                    q = q.Where(() => feedingSourceAlias.Restricted == false);
                }
            }
            else
            {
                // user can access sources they uploaded as well as sources marked public
                q = q.Where(() => uploadedByAlias.UserID == uploadedByUserId || feedingSourceAlias.IsPublic);
            }
            
            return q.TransformUsing(Transformers.AliasToBean<FeedingSourceDTO>())
                .List<FeedingSourceDTO>();
        }

        // for statistics purposes, hence no security permissions
        public IList<FeedingSourceDTO> GetFeedingSourceDTOs(ISession session, DateTime start, DateTime end, bool includeRestricted)
        {
            FeedingSource fsAlias = null;
            FeedingSourceDTO output = null;
            AdminUser uploadedByAlias = null;
            AdminUser approvedByAlias = null;
            AdminUser rejectedByAlias = null;

            ISession thisSession = session == null ? this.Session : session;

            var q = thisSession.QueryOver<FeedingSource>(() => fsAlias)
                .JoinAlias(() => fsAlias.UploadedBy, () => uploadedByAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => fsAlias.ApprovedBy, () => approvedByAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => fsAlias.RejectedBy, () => rejectedByAlias, JoinType.LeftOuterJoin)
                .Where(Restrictions.Disjunction()
                    .Add(Restrictions.On(() => fsAlias.UploadDate).IsBetween(start).And(end))
                    .Add(Restrictions.On(() => fsAlias.ApprovedDate).IsBetween(start).And(end))
                    .Add(Restrictions.On(() => fsAlias.RejectedDate).IsBetween(start).And(end))
                    )
                .SelectList(list => list
                    .Select(() => fsAlias.Id).WithAlias(() => output.Id)
                    .Select(() => fsAlias.Name).WithAlias(() => output.Name)
                    .Select(() => fsAlias.Restricted).WithAlias(() => output.Restricted)
                    .Select(() => fsAlias.FileModifiedDateTime).WithAlias(() => output.FileModifiedDateTime)
                    .Select(() => uploadedByAlias.UserName).WithAlias(() => output.UploadedBy)
                    .Select(() => fsAlias.UploadDate).WithAlias(() => output.UploadDate)
                    .Select(() => approvedByAlias.UserName).WithAlias(() => output.ApprovedBy)
                    .Select(() => fsAlias.ApprovedDate).WithAlias(() => output.ApproveDate)
                    .Select(() => rejectedByAlias.UserName).WithAlias(() => output.RejectedBy)
                    .Select(() => fsAlias.RejectedDate).WithAlias(() => output.RejectedDate)
                    .Select(() => fsAlias.RejectedReason).WithAlias(() => output.RejectedReason)
                    .Select(() => fsAlias.UploadNotes).WithAlias(() => output.UploadNotes)
                    );

            if (!includeRestricted)
                q = q.Where(() => !fsAlias.Restricted);

            return q.TransformUsing(Transformers.AliasToBean<FeedingSourceDTO>())
                .List<FeedingSourceDTO>();
        }
    }
}
