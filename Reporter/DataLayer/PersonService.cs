using System.Collections.Generic;
using Reporter.Model;

namespace Reporter.DataLayer
{
    public static class PersonService
    {
        private static readonly IPersonRepository PersonRepository;

        static PersonService()
        {
            PersonRepository = new PersonRepository();
        }

        public static List<Person> GetAll() => PersonRepository.GetAll();

        public static Person GetById(string emailAddress) => PersonRepository.GetById(emailAddress);

        public static Person Insert(Person person) => PersonRepository.Insert(person);

        public static void Update(Person person) => PersonRepository.Update(person);
    }
}
