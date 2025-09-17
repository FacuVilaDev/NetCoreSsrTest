namespace NetCoreSsrTest.Domain
{
    public class MovieStarship
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;
        public int StarshipId { get; set; }
        public Starship Starship { get; set; } = default!;
    }

}
