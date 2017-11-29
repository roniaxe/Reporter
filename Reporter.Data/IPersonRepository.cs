using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Reporter.Model.Entities;
using Reporter.Model.Model;

namespace Reporter.Data
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
        
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
    }
}
