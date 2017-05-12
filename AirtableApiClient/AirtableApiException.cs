using System;
using System.Net;

namespace AirtableApiClient
{
    public abstract class AirtableApiException : Exception
    {
        protected AirtableApiException(HttpStatusCode errorCode, string errorName, string errorMessage) : base($"{errorName} - {errorCode}: {errorMessage}")
        {
            ErrorCode = errorCode;
            ErrorName = errorName;
            ErrorMessage = errorMessage;
        }

        public readonly HttpStatusCode ErrorCode;
        public readonly string ErrorName;
        public readonly string ErrorMessage;
    }


    public class AirtableUnrecognizedException : AirtableApiException
    {
        public AirtableUnrecognizedException(HttpStatusCode statusCode) : base(statusCode, "Unrecognized Error", $"Airtable returned HTTP status code {statusCode}")
        {
        }
    }


    public class AirtableBadRequestException : AirtableApiException
    {
        public AirtableBadRequestException() : base(HttpStatusCode.BadRequest, "Bad Request", "The request encoding is invalid; the request can't be parsed as a valid JSON.")
        {
        }
    }

    public class AirtableUnauthorizedException : AirtableApiException
    {
        public AirtableUnauthorizedException() : base(HttpStatusCode.Unauthorized, "Unauthorized", "Accessing a protected resource without authorization or with invalid credentials.")
        {
        }
    }


    public class AirtablePaymentRequiredException : AirtableApiException
    {
        public AirtablePaymentRequiredException() : base(
            HttpStatusCode.PaymentRequired, 
            "Payment Required", 
            "The account associated with the API key making requests hits a quota that can be increased by upgrading the Airtable account plan.")
        {
        }
    }


    public class AirtableForbiddenException : AirtableApiException
    {
        public AirtableForbiddenException() : base(
            HttpStatusCode.Forbidden, 
            "Forbidden", 
            "Accessing a protected resource with API credentials that don't have access to that resource.")
        {
        }
    }


    public class AirtableNotFoundException : AirtableApiException
    {
        public AirtableNotFoundException() : base(
            HttpStatusCode.NotFound, 
            "Not Found", 
            "Route or resource is not found. This error is returned when the request hits an undefined route, or if the resource doesn't exist (e.g. has been deleted).")
        {
        }
    }


    public class AirtableRequestEntityTooLargeException : AirtableApiException
    {
        public AirtableRequestEntityTooLargeException() : base(
            HttpStatusCode.RequestEntityTooLarge, 
            "Request Entity Too Large", 
            "The request exceeded the maximum allowed payload size. You shouldn't encounter this under normal use.")
        {
        }
    }


    public class AirtableInvalidRequestException : AirtableApiException
    {
        public AirtableInvalidRequestException() : base(
            (HttpStatusCode)422, 
            "Invalid Request", 
            "The request data is invalid. This includes most of the base-specific validations. You will receive a detailed error message and code pointing to the exact issue.")
        {
        }
    }
}
