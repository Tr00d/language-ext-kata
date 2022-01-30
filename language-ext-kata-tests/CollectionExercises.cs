using System;
using System.Collections.Generic;
using System.Linq;
using language_ext.kata.Persons;
using LanguageExt;
using Xunit;
using static language_ext.kata.Persons.PetType;
using static LanguageExt.Prelude;

namespace language_ext.kata.tests
{
    public class CollectionExercises : PetDomainKata
    {
        [Fact]
        public void GetFirstNamesOfAllPeople()
        {
            // Replace it, with a transformation method on people.
            var firstNames = this.people.Map(person => person.FirstName);
            var expectedFirstNames = Seq("Mary", "Bob", "Ted", "Jake", "Barry", "Terry", "Harry", "John");
            Assert.Equal(expectedFirstNames, firstNames);
        }

        [Fact]
        public void GetNamesOfMarySmithsPets()
        {
            var person = GetPersonNamed("Mary Smith");

            // Replace it, with a transformation method on people.
            var names = person.Pets.Map(pet => pet.Name);
            Assert.Equal("Tabby", names.Single());
        }

        [Fact]
        public void GetPeopleWithCats()
        {
            // Replace it, with a positive filtering method on people.
            var peopleWithCats = this.people.Filter(person => person.HasPetType(CAT));
            Assert.Equal(2, peopleWithCats.Count);
        }

        [Fact]
        public void GetPeopleWithoutCats()
        {
            // Replace it, with a negative filtering method on Seq.
            var peopleWithoutCats = this.people.Filter(person => !person.HasPetType(CAT));
            Assert.Equal(6, peopleWithoutCats.Count);
        }

        [Fact]
        public void DoAnyPeopleHaveCats()
        {
            //replace null with a Predicate lambda which checks for PetType.CAT
            bool doAnyPeopleHaveCats = this.people.Any(person => person.HasPetType(CAT));
            Assert.True(doAnyPeopleHaveCats);
        }

        [Fact]
        public void DoAllPeopleHavePets()
        {
            Func<Person, bool> predicate = p => p.IsPetPerson();

            // OR use local functions -> static bool predicate(Person p) => p.IsPetPerson();
            // replace with a method call send to this.people that checks if all people have pets
            bool result = people.ForAll(predicate);
            Assert.False(result);
        }

        [Fact]
        public void HowManyPeopleHaveCats()
        {
            // replace 0 with the correct answer
            var count = this.people.Count(person => person.HasPetType(CAT));
            Assert.Equal(2, count);
        }

        [Fact]
        public void FindMarySmith()
        {
            Person result = this.people.Find(person => person.Named("Mary Smith"))
                .IfNone(() => throw new InvalidOperationException());
            Assert.Equal("Mary", result.FirstName);
            Assert.Equal("Smith", result.LastName);
        }

        [Fact]
        public void GetPeopleWithPets()
        {
            // replace with only the pets owners
            var petPeople = this.people.Filter(person => person.IsPetPerson());
            Assert.Equal(7, petPeople.Count);
        }

        [Fact]
        public void GetAllPetTypesOfAllPeople()
        {
            var petTypes = this.people.Bind(person => person.GetPetTypes().Keys).ToSeq().Distinct();
            Assert.Equal(
                Seq(CAT, DOG, SNAKE, BIRD, TURTLE, HAMSTER),
                petTypes);
        }

        [Fact]
        public void HowManyPersonHaveCats()
        {
            // use count
            var count = this.people.Count(person => person.HasPetType(CAT));
            Assert.Equal(2, count);
        }

        [Fact]
        public void TotalPetAge()
        {
            var totalAge = this.people.Bind(person => person.Pets).Distinct().Sum(pet => pet.Age);
            Assert.Equal(17L, totalAge);
        }

        [Fact]
        public void PetsNameSorted()
        {
            string sortedPetNames = this.people.Bind(person => person.Pets).Map(pet => pet.Name).Distinct()
                .OrderBy(name => name).ToSeq().ToFullString();
            Assert.Equal("Dolly, Fuzzy, Serpy, Speedy, Spike, Spot, Tabby, Tweety, Wuzzy", sortedPetNames);
        }

        [Fact]
        public void SortByAge()
        {
            // Create a Seq<int> with ascending ordered age values.
            var sortedAgeList = this.people.Bind(person => person.Pets).Map(pet => pet.Age).OrderBy(age => age)
                .Distinct().ToSeq();
            Assert.Equal(4, sortedAgeList.Count);
            Assert.Equal(Seq(1, 2, 3, 4), sortedAgeList);
        }

        [Fact]
        public void SortByDescAge()
        {
            // Create a Seq<int> with descending ordered age values.
            var sortedAgeList = this.people.Bind(person => person.Pets).Map(pet => pet.Age)
                .OrderByDescending(age => age).Distinct().ToSeq();
            Assert.Equal(4, sortedAgeList.Count);
            Assert.Equal(Seq(4, 3, 2, 1), sortedAgeList);
        }

        [Fact]
        public void Top3OlderPets()
        {
            // Create a Seq<string> with the 3 older pets.
            var top3OlderPets = this.people.Bind(person => person.Pets).OrderByDescending(pet => pet.Age).Take(3)
                .Map(pet => pet.Name).ToSeq();
            Assert.Equal(3, top3OlderPets.Count);
            Assert.Equal(Seq("Spike", "Dolly", "Tabby"), top3OlderPets);
        }

        [Fact]
        public void GetFirstPersonWithAtLeast2Pets()
        {
            // Find the first person who owns at least 2 pets
            var firstPersonWithAtLeast2Pets = this.people.Find(person => person.Pets.Count >= 2);
            Assert.True(firstPersonWithAtLeast2Pets.IsSome);
            Assert.Equal("Bob", firstPersonWithAtLeast2Pets.Map(p => p.FirstName));
        }

        [Fact]
        public void IsThereAnyPetOlderThan4()
        {
            // Check whether any exercises older than 4 exists or not
            bool isThereAnyPetOlderThan4 = this.people.Bind(person => person.Pets).Any(pet => pet.Age > 4);
            Assert.False(isThereAnyPetOlderThan4);
        }

        [Fact]
        public void IsEveryPetsOlderThan1()
        {
            // Check whether all pets are older than 1 or not
            bool allOlderThan1 = this.people.Bind(person => person.Pets).All(pet => pet.Age >= 1);
            Assert.True(allOlderThan1);
        }

        [Fact]
        public void GetListOfPossibleParksForAWalkPerPerson()
        {
            Func<Seq<PetType>, Seq<string>> getParksForPetType = petTypes => this.parks
                .Filter(park => !petTypes.Except(park.AuthorizedPetTypes).Any())
                .Map(park => park.Name);

            // For each person described as "firstName lastName" returns the list of names possible parks to go for a walk
            Dictionary<string, Seq<string>> possibleParksForAWalkPerPerson = this.people.ToDictionary(
                person => $"{person.FirstName} {person.LastName}",
                person => getParksForPetType(person.Pets.Map(pet => pet.Type)));
            Assert.Equal(Seq("Jurassic", "Central", "Hippy"), possibleParksForAWalkPerPerson["John Doe"]);
            Assert.Equal(Seq("Jurassic", "Hippy"), possibleParksForAWalkPerPerson["Jake Snake"]);
        }
    }
}