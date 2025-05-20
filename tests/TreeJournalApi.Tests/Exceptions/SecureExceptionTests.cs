using TreeJournalApi.Exceptions;

namespace TreeJournalApi.Tests.Exceptions
{
    public class SecureExceptionTests
    {
        [Fact]
        public void SecureException_CreatesCorrectMessage()
        {
            string expectedMessage = "Access denied";

            var exception = new SecureException(expectedMessage);

            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void SecureException_PreservesInnerException()
        {
            string expectedMessage = "Encryption error";
            var innerException = new InvalidOperationException("Invalid key");

            var exception = new SecureException(expectedMessage, innerException);

            Assert.Equal(expectedMessage, exception.Message);
            Assert.NotNull(exception.InnerException);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Equal("Invalid key", exception.InnerException.Message);
        }
    }
}
