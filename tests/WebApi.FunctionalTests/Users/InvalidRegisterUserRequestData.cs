namespace WebApi.FunctionalTests.Users;

public class InvalidRegisterUserRequestData : TheoryData<string, string, string, string>
{
    public InvalidRegisterUserRequestData()
    {
        Add(string.Empty, "Test1234!", "Test", "Test");
        Add("invalid-email", "Test1234!", "Test", "Test");
        Add("test@test", "Test1234!", "Test", "Test");
        Add("test@test.com", string.Empty, "Test", "Test");
        Add("test@test.com", "123", "Test", "Test");
        Add("test@test.com", "password", "Test", "Test");
        Add("test@test.com", "Test1234", "Test", "Test");
        Add("test@test.com", "Test!@#$", "Test", "Test");
        Add("test@test.com", "Test1234!", string.Empty, "Test");
        Add("test@test.com", "Test1234!", "T", "Test");
        Add("test@test.com", "Test1234!", "123", "Test");
        Add("test@test.com", "Test1234!", "!@#", "Test");
        Add("test@test.com", "Test1234!", "Test", string.Empty);
        Add("test@test.com", "Test1234!", "Test", "T");
        Add("test@test.com", "Test1234!", "Test", "123");
        Add("test@test.com", "Test1234!", "Test", "!@#");
    }
}