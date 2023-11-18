namespace Cross.CQRS.EF.Models;

public class SortParameter
{
    public string Property { get; set; }

    public SortDirectionEnum Direction { get; set; }
}
