using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Security;
using Profiling2.Infrastructure.Util;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class SourceQueries : NHibernateQuery, ISourceQueries
    {
        public IList<Source> GetSourcesByName(string term)
        {
            if (!string.IsNullOrEmpty(term))
                return Session.QueryOver<Source>()
                    .WhereRestrictionOn(x => x.SourceName)
                    .IsInsensitiveLike("%" + term + "%")
                    .And(x => !x.Archive)
                    .Take(50)
                    .List<Source>();
            else
                return new List<Source>();
        }

        public IList<SourceDTO> GetAllSourceDTOs(IStatelessSession session, bool excludeBinaryIndexedSources, bool excludeSourceLogged)
        {
            string sql = @"
                SELECT s.SourceID, s.SourceName, s.FullReference, s.SourcePath, s.SourceDate, s.FileExtension, s.IsRestricted, s.FileDateTimeStamp, s.Archive, 
                    s.IsReadOnly, s.Notes, s.IsPublic, DATALENGTH(s.FileData) AS FileSize, j.CaseNumber AS JhroCaseNumber, j.JhroCaseID,
                    (CASE WHEN s.OriginalFileData IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) AS HasOcrText,
                    au.UserID AS UploadedByUserID
                FROM PRF_Source s LEFT JOIN PRF_JhroCase j ON s.JhroCaseID = j.JhroCaseID
                LEFT JOIN PRF_FeedingSource fs ON s.SourceID = fs.SourceID
                LEFT JOIN PRF_AdminUser au ON fs.UploadedByID = au.AdminUserID
                WHERE s.Archive = 0
            ";
            if (excludeBinaryIndexedSources)
            {
                sql += " AND s.SourceID NOT IN (SELECT SourceID FROM PRF_SourceIndexLog)";
            }
            if (excludeSourceLogged)
            {
                sql += " AND s.SourceID NOT IN (SELECT SourceID FROM PRF_SourceLog)";
            }

            return (session == null ? Session.CreateSQLQuery(sql) : session.CreateSQLQuery(sql))
                .SetResultTransformer(Transformers.AliasToBean(typeof(SourceDTO)))
                .List<SourceDTO>();
        }

        public SourceDTO GetSourceDTO(int sourceId)
        {
            string sql = @"
                SELECT s.SourceID, s.SourceName, s.FullReference, s.SourcePath, s.SourceDate, s.FileExtension, s.IsRestricted, s.FileDateTimeStamp, s.Archive, 
                    s.IsReadOnly, s.Notes, s.IsPublic, DATALENGTH(s.FileData) AS FileSize, j.CaseNumber AS JhroCaseNumber, j.JhroCaseID,
                    (CASE WHEN s.OriginalFileData IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) AS HasOcrText,
                    au.UserID AS UploadedByUserID
                FROM PRF_Source s LEFT JOIN PRF_JhroCase j ON s.JhroCaseID = j.JhroCaseID
                LEFT JOIN PRF_FeedingSource fs ON s.SourceID = fs.SourceID
                LEFT JOIN PRF_AdminUser au ON fs.UploadedByID = au.AdminUserID
                WHERE s.SourceID = :sourceId
            ";
            IList<SourceDTO> results = Session.CreateSQLQuery(sql)
                .SetParameter("sourceId", sourceId, NHibernateUtil.Int32)
                .SetResultTransformer(Transformers.AliasToBean(typeof(SourceDTO)))
                .List<SourceDTO>();  // returning list, as the join with PRF_FeedingsSource can possibly result in more than one row returned

            if (results != null && results.Count > 0)
                return results[0];
            return null;
        }

        public IList<SourceDTO> GetSourceDTOsByHash(string hash)
        {
            string sql = @"
                SELECT s.SourceID, s.SourceName, s.FullReference, s.SourcePath, s.SourceDate, s.FileExtension, s.IsRestricted, s.FileDateTimeStamp, s.Archive, 
                    s.IsReadOnly, s.Notes, s.IsPublic, DATALENGTH(s.FileData) AS FileSize, j.CaseNumber AS JhroCaseNumber, j.JhroCaseID,
                    (CASE WHEN s.OriginalFileData IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) AS HasOcrText
                FROM PRF_Source s LEFT JOIN PRF_JhroCase j ON s.JhroCaseID = j.JhroCaseID
                WHERE s.Archive = 0
                AND s.Hash = :hash
            ";
            return Session.CreateSQLQuery(sql)
                .SetParameter("hash", HexUtil.GetHexToBytes(hash), NHibernateUtil.Binary)
                .SetResultTransformer(Transformers.AliasToBean(typeof(SourceDTO)))
                .List<SourceDTO>();
        }

        public IList<SourceDTO> GetScannableSourceDTOs(int days)
        {
            string sql = @"
                SELECT s.SourceID, s.SourceName, s.FullReference, s.SourcePath, s.SourceDate, s.FileExtension, s.IsRestricted, s.FileDateTimeStamp, s.Archive, 
                    s.IsReadOnly, s.Notes, s.IsPublic, DATALENGTH(s.FileData) AS FileSize, j.CaseNumber AS JhroCaseNumber, j.JhroCaseID,
                    (CASE WHEN s.OriginalFileData IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) AS HasOcrText
                FROM PRF_Source s LEFT JOIN PRF_JhroCase j ON s.JhroCaseID = j.JhroCaseID
                WHERE s.Archive = 0
                AND s.OriginalFileData IS NULL
                AND s.FileExtension IN ('jpg', 'jpeg', 'png', 'bmp', 'gif', 'tiff', 'pdf')
                AND s.SourceDate > DATEADD(DAY, -:days, GETDATE())
                ORDER BY s.FileDateTimeStamp DESC
            ";
            return Session.CreateSQLQuery(sql)
                .SetParameter("days", days, NHibernateUtil.Int32)
                .SetResultTransformer(Transformers.AliasToBean(typeof(SourceDTO)))
                .List<SourceDTO>();
        }

        public IList<SourceDTO> GetSourceDTOsWithPasswordErrors()
        {
            string sql = @"
                SELECT s.SourceID, s.SourceName, s.FullReference, s.SourcePath, s.SourceDate, s.FileExtension, s.IsRestricted, s.FileDateTimeStamp, s.Archive, 
                    s.IsReadOnly, s.Notes, s.IsPublic, DATALENGTH(s.FileData) AS FileSize, j.CaseNumber AS JhroCaseNumber, j.JhroCaseID,
                    (CASE WHEN s.OriginalFileData IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) AS HasOcrText,
                    au.UserID AS UploadedByUserID
                FROM PRF_Source s LEFT JOIN PRF_JhroCase j ON s.JhroCaseID = j.JhroCaseID
                LEFT JOIN PRF_FeedingSource fs ON s.SourceID = fs.SourceID
                LEFT JOIN PRF_AdminUser au ON fs.UploadedByID = au.AdminUserID
                WHERE s.SourceID IN (SELECT SourceID FROM PRF_SourceLog WHERE LogSummary LIKE '%assword%')
            ";
            return Session.CreateSQLQuery(sql)
                .SetResultTransformer(Transformers.AliasToBean(typeof(SourceDTO)))
                .List<SourceDTO>();  // returning list, as the join with PRF_FeedingsSource can possibly result in more than one row returned
        }

        public IList<object[]> GetDuplicatesByHash(int maxResults)
        {
            var query = Session.QueryOver<Source>()
                .Select(
                    Projections.GroupProperty(Projections.Property<Source>(x => x.Hash)),
                    Projections.Count<Source>(x => x.Id)
                )
                .Where(Restrictions.Gt(Projections.Count<Source>(x => x.Id), 1))
                .And(Restrictions.IsNotNull(Projections.Property<Source>(x => x.Hash)))
                .And(Restrictions.Eq(Projections.Property<Source>(x => x.Archive), false));

            if (maxResults > 0)
                return query.Take(maxResults).List<object[]>();
            else
                return query.List<object[]>();
        }

        public IList<object[]> DuplicatesByName(int threshold)
        {
            return Session.QueryOver<Source>()
                .Select(Projections.Group<Source>(x => x.SourceName), Projections.Count<Source>(x => x.SourceName))
                .Where(Restrictions.Gt(Projections.Count<Source>(x => x.SourceName), threshold))
                .Where(x => !x.Archive)
                .OrderBy(Projections.Count<Source>(x => x.SourceName)).Desc
                .List<object[]>();
        }

        public IList<SourceDTO> DuplicatesByIgnored(string[] ignoredFileExtensions)
        {
            string sql = @"
                SELECT SourceID, SourceName, FullReference, SourcePath, SourceDate, FileExtension, IsRestricted, FileDateTimeStamp, Archive, IsReadOnly, Notes, IsPublic, DATALENGTH(FileData) AS FileSize, (CASE WHEN OriginalFileData IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) AS HasOcrText
                FROM PRF_Source
                WHERE FileExtension IN (:ignored)
            ";
            return Session.CreateSQLQuery(sql)
                .SetParameterList("ignored", ignoredFileExtensions, NHibernateUtil.String)
                .SetResultTransformer(Transformers.AliasToBean(typeof(SourceDTO)))
                .List<SourceDTO>();
        }

        public IList<SourceDTO> DuplicatesByNameOf(string name)
        {
            string sql = @"
                SELECT SourceID, SourceName, FullReference, SourcePath, SourceDate, FileExtension, IsRestricted, FileDateTimeStamp, Archive, IsReadOnly, Notes, IsPublic, DATALENGTH(FileData) AS FileSize, (CASE WHEN OriginalFileData IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) AS HasOcrText
                FROM PRF_Source
                WHERE SourceName = :SourceName
                AND Archive = 0
                ORDER BY SourceID
            ";
            return Session.CreateSQLQuery(sql)
                .SetString("SourceName", name)
                .SetResultTransformer(Transformers.AliasToBean(typeof(SourceDTO)))
                .List<SourceDTO>();
        }

        public void DeleteSource(int sourceId)
        {
            string sql = @"
                DELETE
                FROM PRF_Source
                WHERE SourceID = :SourceID
                AND SourceID NOT IN (SELECT SourceID FROM PRF_AdminReviewedSource WHERE SourceID = :SourceID)
                AND SourceID NOT IN (SELECT SourceID FROM PRF_EventSource WHERE SourceID = :SourceID)
                AND SourceID NOT IN (SELECT SourceID FROM PRF_PersonSource WHERE SourceID = :SourceID)
                AND SourceID NOT IN (SELECT SourceID FROM PRF_UnitSource WHERE SourceID = :SourceID)
                AND SourceID NOT IN (SELECT SourceID FROM PRF_OrganizationSource WHERE SourceID = :SourceID)
                AND SourceID NOT IN (SELECT SourceID FROM PRF_OperationSource WHERE SourceID = :SourceID)
                AND SourceID NOT IN (SELECT ParentSourceID FROM PRF_SourceRelationship WHERE ParentSourceID = :SourceID)
                AND SourceID NOT IN (SELECT ChildSourceID FROM PRF_SourceRelationship WHERE ChildSourceID = :SourceID)
                AND SourceID NOT IN (SELECT SourceID FROM PRF_FeedingSource WHERE SourceID = :SourceID)
            ";
            Session.CreateSQLQuery(sql).SetInt32("SourceID", sourceId).SetTimeout(6000).ExecuteUpdate();
        }

        // run without transaction for speed - necessary when doing a bulk merge
        public void ChangeAttachmentsToAnotherSource(int fromSourceId, int toSourceId)
        {
            Session.CreateSQLQuery(@"UPDATE s
                SET s.SourceID = :toSourceId 
                FROM PRF_AdminReviewedSource s
                WHERE s.SourceID = :fromSourceId AND NOT EXISTS
                (SELECT ex.* FROM PRF_AdminReviewedSource ex WHERE ex.SourceID = :toSourceId AND ex.AdminSourceSearchID = s.AdminSourceSearchID)")
                    .SetInt32("toSourceId", toSourceId)
                    .SetInt32("fromSourceId", fromSourceId)
                    .ExecuteUpdate();

            Session.CreateSQLQuery(@"UPDATE s
                SET s.SourceID = :toSourceId 
                FROM PRF_PersonSource s
                WHERE s.SourceID = :fromSourceId AND NOT EXISTS 
                (SELECT ex.* FROM PRF_PersonSource ex WHERE ex.SourceID = :toSourceId AND ex.PersonID = s.PersonID)")
                    .SetInt32("toSourceId", toSourceId)
                    .SetInt32("fromSourceId", fromSourceId)
                    .ExecuteUpdate();

            Session.CreateSQLQuery(@"UPDATE s
                SET s.SourceID = :toSourceId 
                FROM PRF_EventSource s
                WHERE s.SourceID = :fromSourceId AND NOT EXISTS 
                (SELECT ex.* FROM PRF_EventSource ex WHERE ex.SourceID = :toSourceId AND ex.EventID = s.EventID)")
                    .SetInt32("toSourceId", toSourceId)
                    .SetInt32("fromSourceId", fromSourceId)
                    .ExecuteUpdate();

            Session.CreateSQLQuery(@"UPDATE s
                SET s.SourceID = :toSourceId 
                FROM PRF_UnitSource s
                WHERE s.SourceID = :fromSourceId AND NOT EXISTS 
                (SELECT ex.* FROM PRF_UnitSource ex WHERE ex.SourceID = :toSourceId AND ex.UnitID = s.UnitID)")
                    .SetInt32("toSourceId", toSourceId)
                    .SetInt32("fromSourceId", fromSourceId)
                    .ExecuteUpdate();

            Session.CreateSQLQuery(@"UPDATE s
                SET s.SourceID = :toSourceId
                FROM PRF_OrganizationSource s 
                WHERE s.SourceID = :fromSourceId AND NOT EXISTS 
                (SELECT ex.* FROM PRF_OrganizationSource ex WHERE ex.SourceID = :toSourceId AND ex.OrganizationID = s.OrganizationID)")
                    .SetInt32("toSourceId", toSourceId)
                    .SetInt32("fromSourceId", fromSourceId)
                    .ExecuteUpdate();

            Session.CreateSQLQuery(@"UPDATE s
                SET s.SourceID = :toSourceId
                FROM PRF_OperationSource s 
                WHERE s.SourceID = :fromSourceId AND NOT EXISTS 
                (SELECT ex.* FROM PRF_OperationSource ex WHERE ex.SourceID = :toSourceId AND ex.OperationID = s.OperationID)")
                    .SetInt32("toSourceId", toSourceId)
                    .SetInt32("fromSourceId", fromSourceId)
                    .ExecuteUpdate();

            Session.CreateSQLQuery(@"UPDATE s
                SET s.ParentSourceID = :toSourceId 
                FROM PRF_SourceRelationship s
                WHERE s.ParentSourceID = :fromSourceId AND NOT EXISTS 
                (SELECT ex.* FROM PRF_SourceRelationship ex WHERE ex.ParentSourceID = :toSourceId AND ex.ChildSourceID = s.ChildSourceID)")
                    .SetInt32("toSourceId", toSourceId)
                    .SetInt32("fromSourceId", fromSourceId)
                    .ExecuteUpdate();

            Session.CreateSQLQuery(@"UPDATE s
                SET s.ChildSourceID = :toSourceId 
                FROM PRF_SourceRelationship s
                WHERE s.ChildSourceID = :fromSourceId AND NOT EXISTS 
                (SELECT ex.* FROM PRF_SourceRelationship ex WHERE ex.ChildSourceID = :toSourceId AND ex.ParentSourceID = s.ParentSourceID)")
                    .SetInt32("toSourceId", toSourceId)
                    .SetInt32("fromSourceId", fromSourceId)
                    .ExecuteUpdate();

            Session.CreateSQLQuery(@"UPDATE s
                SET s.SourceID = :toSourceId 
                FROM PRF_FeedingSource s
                WHERE s.SourceID = :fromSourceId")
                    .SetInt32("toSourceId", toSourceId)
                    .SetInt32("fromSourceId", fromSourceId)
                    .ExecuteUpdate();


            // archive fromSource
            Session.CreateSQLQuery("UPDATE PRF_Source SET Archive = 1 WHERE SourceID = :fromSourceId")
                .SetInt32("fromSourceId", fromSourceId)
                .ExecuteUpdate();
        }

        public IList<string> GetSourcePaths(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return Session.QueryOver<Source>().Where(x => !x.Archive).Select(x => x.SourcePath).List<string>();
            else
                return Session.QueryOver<Source>()
                    .WhereRestrictionOn(x => x.SourcePath).IsInsensitiveLike(prefix + "%")
                    .And(x => !x.Archive)
                    .Select(x => x.SourcePath)
                    .List<string>();
        }

        public void ArchiveSourcePathPrefix(string prefix)
        {
            string sql = @"
                UPDATE PRF_Source SET Archive = 1 WHERE SourcePath LIKE :prefix;
            ";
            Session.CreateSQLQuery(sql).SetString("prefix", prefix + "%").ExecuteUpdate();
        }

        public Source GetSource(IStatelessSession session, int id)
        {
            return session.QueryOver<Source>()
                .Where(x => x.Id == id)
                .Take(1)
                .SingleOrDefault();
        }

        public IList<SourceDTO> GetExtensionlessSources()
        {
            Source sourceAlias = null;
            SourceDTO dto = null;

            return Session.QueryOver<Source>(() => sourceAlias)
                .Where(Restrictions.Disjunction()
                    .Add(Restrictions.On(() => sourceAlias.FileExtension).IsNull)
                    .Add(Restrictions.Eq(Projections.SqlFunction("LENGTH", NHibernateUtil.Int32, Projections.Property(() => sourceAlias.FileExtension)), 0))
                    // include sources with no ext in SourceName
                    //.Add(Restrictions.Eq(
                    //    Projections.SqlFunction(
                    //        new StandardSQLFunction("CHARINDEX", NHibernateUtil.Int32), 
                    //        NHibernateUtil.Int32, 
                    //        Projections.Constant(@"."), 
                    //        Projections.Property(() => sourceAlias.SourceName)
                    //    ), 0))
                )
                .AndRestrictionOn(() => sourceAlias.FileData).IsNotNull
                .And(() => !sourceAlias.Archive)
                .SelectList(list => list
                    .Select(Projections.Property(() => sourceAlias.Id)).WithAlias(() => dto.SourceID)
                    .Select(Projections.Property(() => sourceAlias.SourceName)).WithAlias(() => dto.SourceName)
                    .Select(Projections.Property(() => sourceAlias.SourcePath)).WithAlias(() => dto.SourcePath)
                    .Select(Projections.Property(() => sourceAlias.FullReference)).WithAlias(() => dto.FullReference)
                    .Select(Projections.Property(() => sourceAlias.SourceDate)).WithAlias(() => dto.SourceDate)
                    .Select(Projections.Property(() => sourceAlias.FileDateTimeStamp)).WithAlias(() => dto.FileDateTimeStamp)
                    .Select(Projections.Property(() => sourceAlias.FileExtension)).WithAlias(() => dto.FileExtension)
                    .Select(Projections.Property(() => sourceAlias.IsRestricted)).WithAlias(() => dto.IsRestricted)
                    .Select(Projections.Property(() => sourceAlias.IsReadOnly)).WithAlias(() => dto.IsReadOnly)
                    .Select(Projections.Property(() => sourceAlias.Archive)).WithAlias(() => dto.Archive)
                    .Select(Projections.Property(() => sourceAlias.Notes)).WithAlias(() => dto.Notes)
                    .Select(Projections.Property(() => sourceAlias.IsPublic)).WithAlias(() => dto.IsPublic)
                )
                .TransformUsing(Transformers.AliasToBean<SourceDTO>())
                .List<SourceDTO>();
        }

        public Stream GetSourceData(int sourceId, bool hasOcrText)
        {
            using (var command = Session.Connection.CreateCommand())
            {
                Session.Transaction.Enlist(command);

                string dataColumn = hasOcrText ? "OriginalFileData" : "FileData";
                command.CommandText = "select " + dataColumn + " from PRF_Source where SourceID = @SourceID";

                var param = command.CreateParameter();
                param.ParameterName = "SourceID";
                param.Value = sourceId;
                command.Parameters.Add(param);

                SqlDataReader reader = (SqlDataReader)command.ExecuteReader(CommandBehavior.SequentialAccess);
                if (false == reader.Read())
                {
                    reader.Dispose();
                    return null;
                }
                return new SqlReaderStream(reader, 0);
            }
        }

        protected string GET_SOURCE_AUTHORS = @"SELECT a.Author 
                    FROM PRF_SourceAuthor a, PRF_SourceAuthorSource sas 
                    WHERE sas.SourceID = :sourceId
                    AND a.SourceAuthorID = sas.SourceAuthorID
                ";

        public IList<string> GetSourceAuthors(IStatelessSession session, int sourceId)
        {
            return session.CreateSQLQuery(GET_SOURCE_AUTHORS).SetParameter("sourceId", sourceId).List<string>();
        }

        public IList<string> GetSourceAuthors(int sourceId)
        {
            return Session.CreateSQLQuery(GET_SOURCE_AUTHORS).SetParameter("sourceId", sourceId).List<string>();
        }

        protected string GET_SOURCE_OWNERS = @"SELECT e.Name 
                    FROM PRF_SourceOwningEntity e, PRF_SourceOwner so
                    WHERE so.SourceID = :sourceId
                    AND e.SourceOwningEntityID = so.SourceOwningEntityID
                ";

        public IList<string> GetSourceOwners(IStatelessSession session, int sourceId)
        {
            return session.CreateSQLQuery(GET_SOURCE_OWNERS).SetParameter("sourceId", sourceId).List<string>();
        }

        public IList<string> GetSourceOwners(int sourceId)
        {
            return Session.CreateSQLQuery(GET_SOURCE_OWNERS).SetParameter("sourceId", sourceId).List<string>();
        }

        public int InsertSourceOwner(int sourceId, int sourceOwningEntityId)
        {
            return Session.CreateSQLQuery("INSERT INTO PRF_SourceOwner (SourceID, SourceOwningEntityID) VALUES (:sourceId, :sourceOwningEntityId)")
             .SetParameter("sourceId", sourceId)
             .SetParameter("sourceOwningEntityId", sourceOwningEntityId)
             .ExecuteUpdate();
        }

        public IList<SourceOwningEntity> GetSourceOwningEntities(ISession session, string name)
        {
            ISession thisSession = session == null ? this.Session : session;

            return thisSession.QueryOver<SourceOwningEntity>()
                .Where(x => x.Name == name)
                .List<SourceOwningEntity>();
        }

        public void SaveSource(ISession session, Source source)
        {
            ISession thisSession = session == null ? this.Session : session;

            thisSession.SaveOrUpdate(source);
        }
    }
}
