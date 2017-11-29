using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reporter.ViewModel
{
    public class ErrorCommentConn
    {
        [Key, Column(Order = 0)]
        public string Batch { get; set; }

        [Key, Column(Order = 1)]
        public string Task { get; set; }

        [Key, Column(Order = 2)]
        public string ErrorMessage { get; set; }

        [StringLength(1000)]
        public string Comments { get; set; }
    }
}
