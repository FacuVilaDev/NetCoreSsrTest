namespace NetCoreSsrTest.Domain
{
    public class MovieSpecies
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;
        public int SpeciesId { get; set; }
        public Species Species { get; set; } = default!;
    }

}
