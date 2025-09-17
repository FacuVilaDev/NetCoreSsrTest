namespace NetCoreSsrTest.Domain
{
    public class Person
    {
        public int Id { get; set; }
        public string ExternalUid { get; set; } = default!;
        public string? Name { get; set; }
    }

}
