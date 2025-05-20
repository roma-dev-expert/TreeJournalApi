using System.Net;
using Newtonsoft.Json;
using TreeJournalApi.Data;
using TreeJournalApi.Exceptions;
using TreeJournalApi.Models;

namespace TreeJournalApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                long eventId = DateTime.UtcNow.Ticks;

                var journalEntry = new ExceptionJournal
                {
                    EventId = eventId,
                    CreatedAt = DateTime.UtcNow,
                    RequestParameters = GetRequestParameters(context),
                    StackTrace = ex.StackTrace
                };

                var db = context.RequestServices.GetService<AppDbContext>();
                db.ExceptionJournals.Add(journalEntry);
                await db.SaveChangesAsync();

                object responseObj;
                if (ex is SecureException)
                {
                    responseObj = new
                    {
                        type = "Secure",
                        id = eventId,
                        data = new { message = ex.Message }
                    };
                }
                else
                {
                    responseObj = new
                    {
                        type = "Exception",
                        id = eventId,
                        data = new { message = $"Internal server error ID = {eventId}" }
                    };
                }

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var json = JsonConvert.SerializeObject(responseObj);
                await context.Response.WriteAsync(json);
                await context.Response.Body.FlushAsync();
            }
        }

        private string GetRequestParameters(HttpContext context)
        {
            return context.Request.QueryString.Value;
        }
    }
}
