namespace SlotMachineAPI.Application.Enums
{
    public enum ErrorMessages
    {
        Unauthorized = 401,
        BadRequest = 400,
        NotFound = 404,
        ServerError = 500
    }
    public static class ErrorMessagesExtensions
    {
        public static string GetMessage(this ErrorMessages errorCode)
        {
            return errorCode switch
            {
                ErrorMessages.Unauthorized => "Please log in.",
                ErrorMessages.BadRequest => "Invalid request.",
                ErrorMessages.NotFound => "The source could not be found.",
                _ => "Server error, please try again!"
            };
        }
    }
}
