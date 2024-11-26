namespace WebApi.FunctionalTests.Users;

public class InvalidRegisterUserRequestData : TheoryData<string, string, string, string>
{
    public InvalidRegisterUserRequestData()
    {
        Add(string.Empty, "Test1234!", "Test", "Test");
        Add("invalid-email", "Test1234!", "Test", "Test");
        Add("test@test.com", string.Empty, "Test", "Test");
        Add("test@test.com", "short1", "Test", "Test");
        Add("test@test.com", "test1234", "Test", "Test");
        Add("test@test.com", "TEST1234", "Test", "Test");
        Add("test@test.com", "Testabcd", "Test", "Test");
        Add("test@test.com", "Test1234!", string.Empty, "Test");
        Add("test@test.com", "Test1234!", new string('a', 51), "Test");
        Add("test@test.com", "Test1234!", "John123", "Test");
        Add("test@test.com", "Test1234!", "Test", string.Empty);
        Add("test@test.com", "Test1234!", "Test", new string('a', 101));
        Add("test@test.com", "Test1234!", "Test", "Doe@2023");
    }
}
