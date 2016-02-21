using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IEmailTasks
    {
        void SendRequestSentForFinalDecisionEmail(string username, int requestId);

        void SendRequestSentForValidationEmail(string username, int requestId);

        void SendPersonsProposedEmail(string username, int requestId);

        void SendRespondedToEmail(string username, int requestId);

        void SendProfileRequestEmail(string username, Person person);

        void SendRequestCompletedEmail(string username, int requestId);

        void SendRequestForwardedToConditionalityParticipantsEmail(string username, int requestId);

        void SendRequestRejectedEmail(string username, int requestId, string reason);

        void SendFeedingSourceUploadedEmail(FeedingSource fs);

        void SendFeedingSourcesUploadedEmail(IList<FeedingSource> sources);

        void SendFeedingSourceApprovedEmail(FeedingSource fs);

        void SendFeedingSourceRejectedEmail(FeedingSource fs);
    }
}
