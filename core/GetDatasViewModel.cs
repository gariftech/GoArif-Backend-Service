
using System.Text.Json;

namespace Goarif.Shared.ViewModels
{
    public class GetDatasViewModel<T>
    {
        public List<T>? Items { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public int? totalCountData { get; set; }
        public int? code { get; set; }
        public ErrorDtoVM? errorMessage { get; set; }
    }

    public class ErrorDtoVM
    {
        public string? error { get; set; }
    }
}