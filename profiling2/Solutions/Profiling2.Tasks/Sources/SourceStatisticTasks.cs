using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Profiling2.Domain.Contracts.Queries.Stats;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.DTO;

namespace Profiling2.Tasks.Sources
{
    public class SourceStatisticTasks : ISourceStatisticTasks
    {
        protected readonly ISourceStatisticsQueries sourceStatisticsQueries;

        public SourceStatisticTasks(ISourceStatisticsQueries sourceStatisticsQueries)
        {
            this.sourceStatisticsQueries = sourceStatisticsQueries;
        }

        public DateTime GetLastAdminSourceImportDate()
        {
            return this.sourceStatisticsQueries.GetLastAdminSourceImportDate();
        }

        public int GetNumArchived()
        {
            return this.sourceStatisticsQueries.GetSourceCount(true);
        }

        public int GetSourceCount()
        {
            return this.sourceStatisticsQueries.GetSourceCount(false);
        }

        public Int64 GetTotalSourceSize()
        {
            return this.sourceStatisticsQueries.GetTotalSize();
        }

        public Int64 GetTotalArchivedSourceSize()
        {
            return this.sourceStatisticsQueries.GetTotalArchivedSize();
        }

        public IList<object[]> GetSourceImportsByDay()
        {
            IList<object[]> countsByDay = this.sourceStatisticsQueries.GetSourceImportsByDay();
            IList<object[]> countsIncreasing = new List<object[]>();

            DateTime earliestDay = new DateTime();
            if (countsByDay != null && countsByDay.Count > 2)
                earliestDay = new DateTime((int)countsByDay[1][0], (int)countsByDay[1][1], (int)countsByDay[1][2]).Subtract(new TimeSpan(TimeSpan.TicksPerDay));

            int totalCount = 0;
            foreach (object[] row in countsByDay)
            {
                object[] newRow = new object[4];
                totalCount += (int)row[3];

                // set null Created dates to just before date of first Created entry
                if (row[0] == null && row[1] == null && row[2] == null)
                {
                    newRow[0] = earliestDay.Year;
                    newRow[1] = earliestDay.Month;
                    newRow[2] = earliestDay.Day;
                }
                else
                {
                    newRow[0] = row[0];
                    newRow[1] = row[1];
                    newRow[2] = row[2];
                }
                newRow[3] = totalCount;

                countsIncreasing.Add(newRow);
            }

            return countsIncreasing;
        }

        protected int GetSourceCountByFolder(string folder, ISession session)
        {
            return this.sourceStatisticsQueries.GetSourceCountByFolder(folder, session);
        }

        protected DateTime GetLastDateByFolder(string folder, ISession session)
        {
            return this.sourceStatisticsQueries.GetLastDateByFolder(folder, session);
        }

        public IDictionary<string, SourceFolderDTO> GetSourceFolderCounts(IList<string> paths, ISession session)
        {
            IDictionary<string, SourceFolderDTO> pathCounts = new Dictionary<string, SourceFolderDTO>();
            foreach (string path in paths.Where(x => !x.StartsWith("#")))
            {
                //pathCounts.Add(path, this.luceneTasks.SourcePathPrefixSearchReturnCounts(path));
                SourceFolderDTO dto = new SourceFolderDTO(path);
                dto.NumFiles = this.GetSourceCountByFolder(path, session);
                dto.LatestFileDate = this.GetLastDateByFolder(path, session);
                pathCounts.Add(path, dto);
            }
            return pathCounts;
        }
    }
}
