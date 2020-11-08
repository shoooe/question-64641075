using System.Collections.Generic;

namespace Question
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public virtual IEnumerable<Post> Posts { get; set; } 
            = new HashSet<Post>();
    }
}