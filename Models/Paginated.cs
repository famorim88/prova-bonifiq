namespace ProvaPub.Models
{
    public class Paginated<T>
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
        public bool HasNext { get; set; }
    }
}
