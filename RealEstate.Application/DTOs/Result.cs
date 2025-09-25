using System.Collections.Generic;
namespace RealEstate.Application.DTOs
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T Value { get; set; }
        public List<string> Errors { get; set; }
    }
}
