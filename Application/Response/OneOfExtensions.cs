using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public static class OneOfExtensions
    {
        public static bool IsSuccess<TResult>(this OneOf<TResult, ErrorInfo> obj) => obj.IsT0;
        public static TResult GetSuccessResult<TResult>(this OneOf<TResult, ErrorInfo> obj) => obj.AsT0;
        public static bool IsError<TResult>(this OneOf<TResult, ErrorInfo> obj) => obj.IsT1;
        public static ErrorInfo GetError<TResult>(this OneOf<TResult, ErrorInfo> obj) => obj.AsT1;
    }
}
