namespace Pokemon.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string str)
        {
            //First letter big
            str = char.ToUpper(str[0]) + str.Substring(1);
            //return the rest unmodified
            return str;
        }
    }
}
