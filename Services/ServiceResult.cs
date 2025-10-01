using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace App.Services
{
    public class ServiceResult<T>
    {
        public T Data { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; }

        [JsonIgnore]
        public string? UrlAsCreated { get; set; }
        [JsonIgnore]
        public bool IsSuccess => Errors == null || Errors.Count == 0;

        [JsonIgnore]
        public bool IsFail => !IsSuccess;

        [JsonIgnore]
        public HttpStatusCode Status { get; set; }

        public static ServiceResult<T> Success(T data, HttpStatusCode status = HttpStatusCode.OK)
            => new() { Data = data, Status = status };
        public static ServiceResult<T> Fail(List<string> errors, HttpStatusCode status = HttpStatusCode.BadRequest)
           => new() { Errors = errors, Status = status };

        public static ServiceResult<T> Fail(string errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
            => new() { Errors = new List<string> { errorMessage }, Status = status };

        public static ServiceResult<T> SuccessAsCreated(T data, string urlAsCreated)
            => new() { Data = data, Status = HttpStatusCode.Created, UrlAsCreated = urlAsCreated };

         }

    public class ServiceResult
    {
        [JsonPropertyName("errors")]
        public List<string>? Errors { get; set; }
        [JsonIgnore] public bool IsFail => !IsSuccess;

        [JsonIgnore] public HttpStatusCode Status { get; set; }
        [JsonIgnore]public bool IsSuccess => Errors == null || Errors.Count == 0;


        public static ServiceResult Fail(List<string> errors, HttpStatusCode status = HttpStatusCode.BadRequest)
          => new() { Errors = errors, Status = status };

        public static ServiceResult Success(HttpStatusCode status = HttpStatusCode.OK)
            => new() { Status = status };

      
        public static ServiceResult Fail(string errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
            => new() { Errors = new List<string> { errorMessage }, Status = status };
    }
}
