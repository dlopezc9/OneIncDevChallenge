namespace Users.Api;

public static class ApiEndpoints
{
    private const string Base = "api";
    public static class Users
    {
        private const string UsersBase = $"{Base}/users";

        public const string Create = UsersBase;
        public const string Get = $"{UsersBase}/{{id:int}}";
        public const string GetAll = UsersBase;
        public const string Update = $"{UsersBase}/{{id:int}}";
        public const string Delete = $"{UsersBase}/{{id:int}}";

    }
}
