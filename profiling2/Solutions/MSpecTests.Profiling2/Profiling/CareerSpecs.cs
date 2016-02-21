using System;
using Machine.Specifications;
using Machine.Specifications.AutoMocking.Rhino;
using Profiling2.Domain.Prf.Careers;
using SharpArch.Domain.DomainModel;

namespace MSpecTests.Profiling2.Profiling
{
    [Tags("Career")]
    public abstract class given_date_2013_04_01 : Specification<Entity, Career>
    {
        protected static Career Career;
        protected static DateTime DateToCheck;
        protected static bool WasActiveOn;

        Establish context = () =>
        {
            DateToCheck = new DateTime(2013, 4, 1);
        };
    }

    [Subject("Given date 2013-04-01")]
    public class was_active_for_career_starting_2013_02_ending_2013_06
        : given_date_2013_04_01
    {
        Establish context = () =>
        {
            Career = new Career() { YearOfStart = 2013, MonthOfStart = 2, YearOfEnd = 2013, MonthOfEnd = 6 };
        };
        Because of = () => WasActiveOn = Career.WasActiveOn(DateToCheck);
        It should_return_true = () => WasActiveOn.ShouldBeTrue();
    }

    [Subject("Given date 2013-04-01")]
    public class was_active_for_career_starting_2013_02
        : given_date_2013_04_01
    {
        Establish context = () =>
        {
            Career = new Career() { YearOfStart = 2013, MonthOfStart = 2 };
        };
        Because of = () => WasActiveOn = Career.WasActiveOn(DateToCheck);
        It should_return_true = () => WasActiveOn.ShouldBeTrue();
    }

    [Subject("Given date 2013-04-01")]
    public class was_active_for_career_starting_2013_04_01
        : given_date_2013_04_01
    {
        Establish context = () =>
        {
            Career = new Career() { YearOfStart = 2013, MonthOfStart = 4, DayOfStart = 1 };
        };
        Because of = () => WasActiveOn = Career.WasActiveOn(DateToCheck);
        It should_return_true = () => WasActiveOn.ShouldBeTrue();
    }

    [Subject("Given date 2013-04-01")]
    public class was_active_for_career_ending_2013_02
        : given_date_2013_04_01
    {
        Establish context = () =>
        {
            Career = new Career() { YearOfEnd = 2013, MonthOfEnd = 2 };
        };
        Because of = () => WasActiveOn = Career.WasActiveOn(DateToCheck);
        It should_return_false = () => WasActiveOn.ShouldBeFalse();
    }

    [Subject("Given date 2013-04-01")]
    public class was_active_for_career_with_as_of_2013_02
        : given_date_2013_04_01
    {
        Establish context = () =>
        {
            Career = new Career() { YearAsOf = 2013, MonthAsOf = 2 };
        };
        Because of = () => WasActiveOn = Career.WasActiveOn(DateToCheck);
        It should_return_true = () => WasActiveOn.ShouldBeTrue();
    }

    [Subject("Given date 2013-04-01")]
    public class was_active_for_career_with_as_of_2013_06
        : given_date_2013_04_01
    {
        Establish context = () =>
        {
            Career = new Career() { YearAsOf = 2013, MonthAsOf = 6 };
        };
        Because of = () => WasActiveOn = Career.WasActiveOn(DateToCheck);
        It should_return_false = () => WasActiveOn.ShouldBeFalse();
    }

    [Subject("Given date 2013-04-01")]
    public class was_active_for_career_with_as_of_2013_05_starting_2012_11
        : given_date_2013_04_01
    {
        Establish context = () =>
        {
            Career = new Career() { YearAsOf = 2013, MonthAsOf = 5, YearOfStart = 2012, MonthOfStart = 11 };
        };
        Because of = () => WasActiveOn = Career.WasActiveOn(DateToCheck);
        It should_return_true = () => WasActiveOn.ShouldBeTrue();
    }

    [Subject("Given date 2013-04-01")]
    public class was_active_for_career_with_as_of_2013_05_ending_2013_05
        : given_date_2013_04_01
    {
        Establish context = () =>
        {
            Career = new Career() { YearAsOf = 2013, MonthAsOf = 5, YearOfEnd = 2013, MonthOfEnd = 5 };
        };
        Because of = () => WasActiveOn = Career.WasActiveOn(DateToCheck);
        It should_return_false = () => WasActiveOn.ShouldBeFalse();
    }

    [Subject("Given date 2013-04-01")]
    public class was_active_for_career_with_as_of_2013_05_starting_2013_01_01_ending_2013_05
        : given_date_2013_04_01
    {
        Establish context = () =>
        {
            Career = new Career() { YearOfStart = 2013, MonthOfStart = 1, DayOfStart = 1, YearAsOf = 2013, MonthAsOf = 5, YearOfEnd = 2013, MonthOfEnd = 5 };
        };
        Because of = () => WasActiveOn = Career.WasActiveOn(DateToCheck);
        It should_return_true = () => WasActiveOn.ShouldBeTrue();
    }

    [Subject("Given date 2013-04-01")]
    public class was_active_for_career_starting_2005_10_ending_2013
        : given_date_2013_04_01
    {
        Establish context = () =>
        {
            Career = new Career() { YearOfStart = 2005, MonthOfStart = 10, YearOfEnd = 2013 };
        };
        Because of = () => WasActiveOn = Career.WasActiveOn(DateToCheck);
        It should_return_true = () => WasActiveOn.ShouldBeTrue();
    }
}
