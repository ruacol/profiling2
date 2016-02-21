
//using System;
//using System.Collections.Generic;
//using NHibernate;
//using Profiling2.Domain.Contracts.Queries;
//using Profiling2.Domain.Prf.Sources;
//using SharpArch.NHibernate;

//namespace Profiling2.Infrastructure.Queries
//{
//    public class SourceDataTablesQuery : NHibernateQuery<Source>, ISourceDataTablesQuery
//    {
//        public override IList<Source> ExecuteQuery()
//        {
//            throw new NotImplementedException();
//        }

//        protected IQueryOver<Source, Source> BaseSearch(string searchName, string searchExt, string searchText)
//        {
//            IQueryOver<Source, Source> qo = Session.QueryOver<Source>();
//            if (!string.IsNullOrEmpty(searchName))
//                qo.WhereRestrictionOn(x => x.SourceName).IsLike("%" + searchName + "%");
//            if (!string.IsNullOrEmpty(searchExt))
//                qo.WhereRestrictionOn(x => x.FileExtension).IsLike("%" + searchExt + "%");
//            //if (!string.IsNullOrEmpty(searchText))
//                //qo.Where(Restrictions.On<Source>(x => x.FileExtension).IsLike("%" + sSearch + "%") || Restrictions.On<Source>(x => x.SourceName).IsLike("%" + sSearch + "%"));
//            return qo;
//        }

//        public int GetSearchTotal(string searchName, string searchExt, string searchText)
//        {
//            var qo = BaseSearch(searchName, searchExt, searchText);
//            return qo.RowCount();
//        }

//        // TODO - filter IsRestricted
//        // TODO - incorporate ReviewedAdminSource.IsRelevant
//        public IList<Source> GetPaginatedResults(int iDisplayStart, int iDisplayLength, 
//            string searchName, string searchExt, string searchText, 
//            int iSortingCols, List<int> iSortCol, List<string> sSortDir)
//        {
//            var qo = BaseSearch(searchName, searchExt, searchText);

//            if (iDisplayLength > -1)
//                qo.Take(iDisplayLength);
//            qo.Skip(iDisplayStart);

//            if (sSortDir[0] == "desc")
//            {
//                if (iSortCol[0] == 0)
//                {
//                    qo = qo.OrderBy(s => s.Id).Desc;
//                }
//                else if (iSortCol[0] == 1)
//                {
//                    qo = qo.OrderBy(s => s.SourceName).Desc;
//                }
//            }
//            else if (sSortDir[0] == "asc")
//            {
//                if (iSortCol[0] == 0)
//                {
//                    qo = qo.OrderBy(s => s.Id).Asc;
//                }
//                else if (iSortCol[0] == 1)
//                {
//                    qo = qo.OrderBy(s => s.SourceName).Asc;
//                }
//            }

//            for (int k = 1; k < iSortingCols; k++)
//            {
//                if (sSortDir[k] == "desc")
//                {
//                    if (iSortCol[k] == 0)
//                    {
//                        qo = qo.ThenBy(s => s.Id).Desc;
//                    }
//                    else if (iSortCol[k] == 1)
//                    {
//                        qo = qo.ThenBy(s => s.SourceName).Desc;
//                    }
//                }
//                else if (sSortDir[k] == "asc")
//                {
//                    if (iSortCol[k] == 0)
//                    {
//                        qo = qo.ThenBy(s => s.Id).Asc;
//                    }
//                    else if (iSortCol[k] == 1)
//                    {
//                        qo = qo.ThenBy(s => s.SourceName).Asc;
//                    }
//                }
//            }
            
            
//            return qo.List<Source>();
//        }

//    }
//}
