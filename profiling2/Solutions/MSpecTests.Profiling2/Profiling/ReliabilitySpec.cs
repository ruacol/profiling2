using System.Collections.Generic;
using Machine.Specifications;
using Machine.Specifications.AutoMocking.Rhino;
using Profiling2.Domain.Prf.Sources;
using SharpArch.Domain.DomainModel;

namespace MSpecTests.Profiling2.Profiling
{
    [Tags("Reliability")]
    public abstract class given_a_list_of_reliabilities : Specification<Entity, Reliability>
    {
        protected static List<Reliability> Reliabilities;

        Establish context = () =>
        {
            Reliabilities = new List<Reliability>();
            Reliabilities.Add(new Reliability() { ReliabilityName = "Doesn't exist" });
            Reliabilities.Add(new Reliability() { ReliabilityName = "High" });
            Reliabilities.Add(new Reliability() { ReliabilityName = "Moderate" });
            Reliabilities.Add(null);
            Reliabilities.Add(new Reliability() { ReliabilityName = "Low" });
        };
    }

    [Subject("Reliability implementation of IComparable")]
    public class checking_unsorted_reliabilities
        : given_a_list_of_reliabilities
    {
        It should_have_unrecognised_first = () => string.Equals(Reliabilities[0].ReliabilityName, "Doesn't exist");
        It should_have_high_second = () => string.Equals(Reliabilities[1].ReliabilityName, "High");
        It should_have_moderate_third = () => string.Equals(Reliabilities[2].ReliabilityName, "Moderate");
        It should_have_null_fourth = () => Reliabilities[3].ShouldBeNull();
        It should_have_low_last = () => string.Equals(Reliabilities[4].ReliabilityName, "Low");
    }

    [Subject("Reliability implementation of IComparable")]
    public class checking_sorted_reliabilities
        : given_a_list_of_reliabilities
    {
        Because of = () => Reliabilities.Sort();
        It should_sort_null_first = () => Reliabilities[0].ShouldBeNull();
        It should_sort_unrecognised_second = () => string.Equals(Reliabilities[1].ReliabilityName, "Doesn't exist");
        It should_sort_low_third = () => string.Equals(Reliabilities[2].ReliabilityName, "Low");
        It should_sort_moderate_fourth = () => string.Equals(Reliabilities[3].ReliabilityName, "Moderate");
        It should_sort_high_last = () => string.Equals(Reliabilities[4].ReliabilityName, "High");
    }
}
