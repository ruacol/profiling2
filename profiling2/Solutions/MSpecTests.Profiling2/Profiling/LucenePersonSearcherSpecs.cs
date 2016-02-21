using Machine.Specifications;
using Machine.Specifications.AutoMocking.Rhino;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Infrastructure.Search;
using System.Collections.Generic;
using Profiling2.Infrastructure.Search.IndexWriters;
using Lucene.Net.Store;

namespace MSpecTests.Profiling2.Profiling
{
    public abstract class given_a_lucene_index_on_persons : Specification<IPersonSearcher, PersonSearcher>
    {
        protected static ILuceneIndexer<PersonIndexer> personIndexer;
        protected static IPersonSearcher personSearcher;
        protected static IList<LuceneSearchResult> results;

        Establish context = () =>
            {
                personIndexer = new PersonIndexer();
                personSearcher = new PersonSearcher();

                personIndexer.DeleteIndex();
                personIndexer.Add<Person>(new Person() { FirstName = "Rob", LastName = "Stark", MilitaryIDNumber = "123456" });
                personIndexer.Add<Person>(new Person() { FirstName = "Arya", LastName = "Stark", MilitaryIDNumber = "5432257" });
                personIndexer.Add<Person>(new Person() { FirstName = "Jon", LastName = "Snow", MilitaryIDNumber = "436765" });
                personIndexer.Add<Person>(new Person() { FirstName = "Tyrion", LastName = "Lannister", MilitaryIDNumber = "55321" });
                personIndexer.Add<Person>(new Person() { MilitaryIDNumber = "1197402803091" });
                personIndexer.Add<Person>(new Person() { MilitaryIDNumber = "1196202803091" });
            };
    }

    [Tags("Lucene")]
    [Subject("Lucene person search")]
    public class when_searching_exact_first_name : given_a_lucene_index_on_persons
    {
        Because of = () => results = personSearcher.Search("rob", 10, true);
        It should_return_one_result = () => results.Count.ShouldEqual(1);
    }

    [Tags("Lucene")]
    [Subject("Lucene person search")]
    public class when_searching_exact_last_name : given_a_lucene_index_on_persons
    {
        Because of = () => results = personSearcher.Search("snow", 10, true);
        It should_return_one_result = () => results.Count.ShouldEqual(1);
    }

    [Tags("Lucene")]
    [Subject("Lucene person search")]
    public class when_searching_exact_ID_number : given_a_lucene_index_on_persons
    {
        Because of = () => results = personSearcher.Search("123456", 10, true);
        It should_return_one_result = () => results.Count.ShouldEqual(1);
    }

    [Tags("Lucene")]
    [Subject("Lucene person search")]
    public class when_searching_formatted_ID_number : given_a_lucene_index_on_persons
    {
        Because of = () => results = personSearcher.Search("1-23-456", 10, true);
        It should_return_result_first = () => results[0].FieldValues["MilitaryIDNumber"][0].ShouldEqual("123456");
    }

    [Tags("Lucene")]
    [Subject("Lucene person search")]
    public class when_searching_exact_full_name : given_a_lucene_index_on_persons
    {
        Because of = () => results = personSearcher.Search("rob stark", 10, true);
        It should_return_result_first = () => results[0].FieldValues["MilitaryIDNumber"][0].ShouldEqual("123456");
    }

    [Tags("Lucene")]
    [Subject("Lucene person search")]
    public class when_searching_exact_reversed_full_name : given_a_lucene_index_on_persons
    {
        Because of = () => results = personSearcher.Search("stark rob", 10, true);
        It should_return_result_first = () => results[0].FieldValues["MilitaryIDNumber"][0].ShouldEqual("123456");
    }

    [Tags("Lucene")]
    [Subject("Lucene person search")]
    public class when_searching_incomplete_first_name : given_a_lucene_index_on_persons
    {
        Because of = () => results = personSearcher.Search("jo", 10, true);
        It should_return_one_result = () => results.Count.ShouldEqual(1);
    }

    [Tags("Lucene")]
    [Subject("Lucene person search")]
    public class when_searching_incomplete_last_name : given_a_lucene_index_on_persons
    {
        Because of = () => results = personSearcher.Search("sno", 10, true);
        It should_return_one_result = () => results.Count.ShouldEqual(1);
    }

    [Tags("Lucene")]
    [Subject("Lucene person search")]
    public class when_searching_incomplete_ID_number : given_a_lucene_index_on_persons
    {
        Because of = () => results = personSearcher.Search("11974028", 10, true);
        It should_return_one_result = () => results.Count.ShouldEqual(1);
    }

    // TODO - search rank, function, aliases
}
