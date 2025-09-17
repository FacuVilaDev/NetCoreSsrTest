namespace NetCoreSsrTest.Domain
{
    public class MovieVehicle
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = default!;
    }

}
