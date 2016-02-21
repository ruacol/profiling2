using System;
using System.Collections.Generic;
using Machine.Specifications;
using Machine.Specifications.AutoMocking.Rhino;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;
using SharpArch.Domain.DomainModel;

namespace MSpecTests.Profiling2.Profiling
{
    [Tags("Career")]
    public abstract class given_person_with_careers : Specification<Entity, Person>
    {
        protected static Person Person;
        protected static DateTime DateToCheck;
        protected static IList<Career> CareersAtTimeOf;

        protected static Career Career1 = new Career() { YearOfStart = 2005, MonthOfStart = 8, DayOfStart = 17, YearAsOf = 2005, MonthAsOf = 8, DayAsOf = 17 };
        protected static Career Career2 = new Career() { YearOfStart = 2007, MonthOfStart = 6, DayOfStart = 14, YearAsOf = 2012, MonthAsOf = 10 };
        protected static Career Career3 = new Career() { YearAsOf = 2013, MonthAsOf = 7, DayAsOf = 7 };

        Establish context = () =>
        {
            Person = new Person();
            Person.AddCareer(Career1);
            Person.AddCareer(Career2);
            Person.AddCareer(Career3);
        };
    }

    [Subject("Given person with careers")]
    public class careers_at_time_of_2012_10_01
        : given_person_with_careers
    {
        Establish context = () =>
        {
            DateToCheck = new DateTime(2012, 10, 1);
        };
        Because of = () => CareersAtTimeOf = Person.GetCareersAtTimeOf(DateToCheck);
        It should_have_two_careers = () => CareersAtTimeOf.Count.ShouldEqual(2);
        It should_contain_career1 = () => CareersAtTimeOf.ShouldContain(Career1);
        It should_contain_career2 = () => CareersAtTimeOf.ShouldContain(Career2);
    }

    [Subject("Given person with careers")]
    public class careers_at_time_of_2012_09_01
        : given_person_with_careers
    {
        Establish context = () =>
        {
            DateToCheck = new DateTime(2012, 9, 1);
        };
        Because of = () => CareersAtTimeOf = Person.GetCareersAtTimeOf(DateToCheck);
        It should_have_two_careers = () => CareersAtTimeOf.Count.ShouldEqual(2);
        It should_contain_career1 = () => CareersAtTimeOf.ShouldContain(Career1);
        It should_contain_career2 = () => CareersAtTimeOf.ShouldContain(Career2);
    }
}
