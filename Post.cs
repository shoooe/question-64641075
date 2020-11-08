namespace Question
{
    public class Post
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public int Score { get; set; }

        public virtual User User { get; set; } = null!;
    }    
}