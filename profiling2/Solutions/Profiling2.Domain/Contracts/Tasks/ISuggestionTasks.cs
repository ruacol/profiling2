using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Suggestions;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface ISuggestionTasks
    {
        int GetSuggestionTotal(int personId);

        IList<SuggestionEventForPersonDTO> GetSuggestionResults(int iDisplayStart, int iDisplayLength, int personId);

        AdminSuggestionPersonResponsibility SaveSuggestionPersonResponsibility(AdminSuggestionPersonResponsibility aspr);

        IList<FeatureProbabilityCalc> CountSuggestedFeatures();

        IList<SuggestionEventForPersonDTO> GetSuggestionsRefactored(Person p, IList<int> enabledIds);

        IDictionary<string, AdminSuggestionFeaturePersonResponsibility> GetAdminSuggestionFeaturePersonResponsibilities();
    }
}
