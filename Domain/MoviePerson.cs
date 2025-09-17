namespace NetCoreSsrTest.Domain
{
    public class MoviePerson
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;
        public int PersonId { get; set; }
        public Person Person { get; set; } = default!;
    }

}
