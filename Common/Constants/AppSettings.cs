namespace Common.Constants
{
    public static class AppSettings
    {
        public static string ConnectionStrings { get; set; }
        public static int RefreshTokenTtl { get; set; }
        public static string[] CORS { get; set; }
        public static string Secret { get; set; }
    }
}