using Machine.Specifications;
using Machine.Specifications.AutoMocking.Rhino;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using SharpArch.Domain.DomainModel;

namespace MSpecTests.Profiling2.Profiling
{
    [Tags("PersonRelationship")]
    public abstract class person_relationship_from_2013_02_to_2013_06 : Specification<Entity, PersonRelationship>
    {
        protected static PersonRelationship Relationship;
        protected static Event Event;
        protected static bool HasIntersectingDate;

        Establish context = () =>
        {
            Relationship = new PersonRelationship() { YearOfStart = 2013, MonthOfStart = 2, YearOfEnd = 2013, MonthOfEnd = 6 };
        };
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_from_2013_03_01_to_2013_03_02
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfStart = 2013, MonthOfStart = 3, DayOfStart = 1, YearOfEnd = 2013, MonthOfEnd = 3, DayOfEnd = 2 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_true = () => HasIntersectingDate.ShouldBeTrue();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_from_2013_03_01
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfStart = 2013, MonthOfStart = 3, DayOfStart = 1 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_true = () => HasIntersectingDate.ShouldBeTrue();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_from_2013_03
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfStart = 2013, MonthOfStart = 3 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_true = () => HasIntersectingDate.ShouldBeTrue();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_from_2013
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfStart = 2013 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_false = () => HasIntersectingDate.ShouldBeFalse();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_ends_2013_03_01
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfEnd = 2013, MonthOfEnd = 3, DayOfEnd = 1 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_true = () => HasIntersectingDate.ShouldBeTrue();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_ends_2013_03
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfEnd = 2013, MonthOfEnd = 3 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_true = () => HasIntersectingDate.ShouldBeTrue();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_ends_2013
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfEnd = 2013 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_false = () => HasIntersectingDate.ShouldBeFalse();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_no_event_date
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event();
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_false = () => HasIntersectingDate.ShouldBeFalse();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_from_2013_07_01_to_2013_07_02
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfStart = 2013, MonthOfStart = 7, DayOfStart = 1, YearOfEnd = 2013, MonthOfEnd = 7, DayOfEnd = 2 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_false = () => HasIntersectingDate.ShouldBeFalse();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_from_2013_07_01
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfStart = 2013, MonthOfStart = 7, DayOfStart = 1 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_false = () => HasIntersectingDate.ShouldBeFalse();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_from_2013_07
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfStart = 2013, MonthOfStart = 7 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_false = () => HasIntersectingDate.ShouldBeFalse();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_ends_2013_07_01
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfEnd = 2013, MonthOfEnd = 7, DayOfEnd = 1 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_false = () => HasIntersectingDate.ShouldBeFalse();
    }

    [Subject("Checking date intersection for a person relationship from 2013-2 to 2013-6")]
    public class when_event_date_ends_2013_07
        : person_relationship_from_2013_02_to_2013_06
    {
        Establish context = () =>
        {
            Event = new Event() { YearOfEnd = 2013, MonthOfEnd = 7 };
        };
        Because of = () => HasIntersectingDate = Relationship.HasIntersectingDateWith(Event);
        It should_return_false = () => HasIntersectingDate.ShouldBeFalse();
    }
}
