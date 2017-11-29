using System.ComponentModel.DataAnnotations;

namespace Reporter.Model.Entities
{
    public class Person
    {
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        [Key]
        public string EmailAddress { get; set; }

        public bool Active { get; set; }
    }
}
