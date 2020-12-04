using System;

namespace Core
{
    public class EnumUtils<T> where T : Enum
    {
        public static T Get(string name, bool ignoreCase = false)
        {
            return (T) Enum.Parse(typeof(T), name, ignoreCase);
        }
        
        public static int Count
        {
            get
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T must be an enumerated type");

                return Enum.GetNames(typeof(T)).Length;
            }
        }
    }
}