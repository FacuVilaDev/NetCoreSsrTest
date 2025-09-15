namespace NetCoreSsrTest.Infrastructure;

public class SwapiDtos
{
	public class SwapiListResponse<T>
	{
		public string Message { get; set; } = default!;
		public int? Total_records { get; set; }
		public int? Total_pages { get; set; }
		public string? Previous { get; set; }
		public string? Next { get; set; }
		public List<SwapiListItem>? Results { get; set; }
	}
	public class SwapiListItem
	{
		public string Uid { get; set; } = default!;
		public string? Title { get; set; }
		public string Url { get; set; } = default!;
	}
	public class SwapiFilmDetailResponse
	{
		public string Message { get; set; } = default!;
		public SwapiFilmDetail Result { get; set; } = default!;
	}
	public class SwapiFilmDetail
	{
		public string Uid { get; set; } = default!;
		public string? Description { get; set; }
		public SwapiFilmProps Properties { get; set; } = default!;
		public string? Url { get; set; }
	}
	public class SwapiFilmProps
	{
		public string Title { get; set; } = default!;
		public int Episode_id { get; set; }
		public string Opening_crawl { get; set; } = default!;
		public string Director { get; set; } = default!;
		public string Producer { get; set; } = default!;
		public string Release_date { get; set; } = default!;
		public string Url { get; set; } = default!;
	}

}
