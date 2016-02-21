using Machine.Specifications;
using Machine.Specifications.AutoMocking.Rhino;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using System.Collections.Generic;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Tasks.Screenings;

namespace MSpecTests.Profiling2.Screening
{
    public abstract class when_a_request_is_opened_by_screening_entity_for_the_first_time : Specification<IScreeningTasks, ScreeningTasks>
    {
        protected static IScreeningTasks screeningTasks;
        protected static RequestPerson requestPerson;
        protected static Request request;
        protected static ScreeningEntity screeningEntity;
        protected static string username;

        Establish context = () => 
            {
                screeningTasks = DependencyOf<IScreeningTasks>();
                requestPerson = new RequestPerson();
                request = new Request() { Persons = new List<RequestPerson> { requestPerson } };
                screeningEntity = new ScreeningEntity() { ScreeningEntityName = ScreeningEntity.JHRO };
                username = "I-0001";
            };

        Because of = () => screeningTasks.CreateScreeningRequestPersonEntitiesForRequest(request, screeningEntity, username);
    }

    [Subject("Screening request entity responses")]
    public class where_individuals_have_no_previous_screening_history
        : when_a_request_is_opened_by_screening_entity_for_the_first_time
    {
        It should_create_new_responses;// = () => request.ScreeningRequestEntityResponses.ShouldNotBeEmpty();

        It should_return_blank_responses;
    }

    [Subject("Screening request entity responses")]
    public class where_individuals_have_been_screened_before
        : when_a_request_is_opened_by_screening_entity_for_the_first_time
    {
        It should_create_new_responses;// = () => request.ScreeningRequestEntityResponses.ShouldNotBeEmpty();

        It should_return_responses_populated_with_results_of_most_recent_response;
    }
}
