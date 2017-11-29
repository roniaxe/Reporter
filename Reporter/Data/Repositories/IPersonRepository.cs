using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Reporter.Model;

namespace Reporter.Data.Repositories
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
        void Remove(Person person);
    }

    public class PersonRepository : IPersonRepository
    {
        public List<Person> GetAll()
        {
            using (var db = new ReporterCompactModel())
            {
                return db.Persons.ToList();
            }
        }

        public Person GetById(object emailAddress)
        {
            using (var db = new ReporterCompactModel())
            {
                return db.Persons.FirstOrDefault(person => person.EmailAddress.Equals((string)emailAddress));
            }
        }

        public Person Insert(Person person)
        {
            using (var db = new ReporterCompactModel())
            {
                db.Persons.Add(person);
                db.SaveChanges();
                return person;
            }
        }

        public void Update(Person person)
        {
            using (var db = new ReporterCompactModel())
            {
                db.Persons.Attach(person);
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        public void Remove(Person person)
        {
            using (var db = new ReporterCompactModel())
            {
                db.Persons.Attach(person);
                db.Persons.Remove(person);
                db.SaveChanges();
            }
        }
    }
}
