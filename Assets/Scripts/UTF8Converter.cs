using System.Text;

public class UTF8Converter
{
    public static string GetUTF8String(string str)
    {
        byte[] bytes = Encoding.Default.GetBytes(str);
        return Encoding.UTF8.GetString(bytes);
    }
}
