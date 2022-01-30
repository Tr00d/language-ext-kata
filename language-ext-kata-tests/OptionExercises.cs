using System;
using System.Text;
using language_ext.kata.Persons;
using LanguageExt;
using Xunit;
using static LanguageExt.Prelude;

namespace language_ext.kata.tests
{
    public class OptionExercises : PetDomainKata
    {
        [Fact]
        public void FilterAListOfPerson()
        {
            // Filter this list with only defined persons
            var persons = Seq(
                None,
                Some(new Person("John", "Doe")),
                Some(new Person("Mary", "Smith")),
                None);
            Seq<Person> definedPersons = persons
                .Filter(person => person.IsSome)
                .Map(person => person.IfNone(() => throw new InvalidOperationException()));
            Assert.Equal(2, definedPersons.Count);
        }

        [Fact]
        public void WorkingWithNull()
        {
            // Instantiate a None Option of string
            // map it to an Upper case function
            // then it must return the string "Ich bin empty" if empty
            Func<Option<string>, string> upperCase = parameter =>
                parameter.Match(value => value.ToUpperInvariant(), "Ich bin empty");
            var iamAnOption = Option<string>.None;
            string optionValue = upperCase(iamAnOption);
            Assert.True(iamAnOption.IsNone);
            Assert.Equal("Ich bin empty", optionValue);
        }

        [Fact]
        public void FindKaradoc()
        {
            // Find Karadoc in the people List or returns Perceval
            var foundPersonLastName = this.people
                .Find(person => person.LastName.Equals("Karadok"))
                .Match(person => person.LastName, "Perceval");
            Assert.Equal("Perceval", foundPersonLastName);
        }

        [Fact]
        public void FindPersonOrDieTryin()
        {
            // Find a person matching firstName and lastName, throws an IllegalArgumentException if not found
            var firstName = "Rick";
            var lastName = "Sanchez";
            Assert.Throws<ArgumentException>(() =>
                this.people.Find(person => person.Named($"{firstName} {lastName}"))
                    .IfNone(() => throw new ArgumentException()));
        }

        [Fact]
        public void ChainCall()
        {
            // Chain calls to the half method 4 times with start in argument
            // For each half append the value to the resultBuilder (side effect)
            double start = 500d;
            StringBuilder resultBuilder = new StringBuilder();
            Option<double> result = this.Half(start)
                .Do(value => resultBuilder.Append(value))
                .Bind(this.Half)
                .Do(value => resultBuilder.Append(value))
                .Bind(this.Half)
                .Do(value => resultBuilder.Append(value))
                .Bind(this.Half)
                .Do(value => resultBuilder.Append(value));
            Assert.Equal(result, None);
            Assert.Equal("250125", resultBuilder.ToString());
        }

        private Option<double> Half(double x)
        {
            return x % 2 == 0 ? Some(x / 2) : None;
        }
    }
}