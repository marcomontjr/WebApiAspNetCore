using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Alura.WebAPI.Api.Modelos
{
    public class ErrorResponse
    {
        public int Codigo { get; set; }
        public string Mensagem { get; set; }
        public ErrorResponse InnerError { get; set; }
        public string[] Detalhes { get; set; }

        public static ErrorResponse From(Exception ex)
        {
            if (ex == null)
                return null;
            
            return new ErrorResponse
            {
                Codigo = ex.HResult,
                Mensagem = ex.Message,
                InnerError = From(ex.InnerException)
            };
        }

        public static ErrorResponse FromModelState(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(m => m.Errors);
            return new ErrorResponse
            {
                Codigo = 100,
                Mensagem = "Houve erro no envio da requisição",
                Detalhes = erros.Select(e => e.ErrorMessage).ToArray()
            };
        }
    }
}