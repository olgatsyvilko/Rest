using Rest.Enums;

namespace Rest.Helpers
{
    public static class RandomHelper
    {
        private static readonly Random Random = new();

        public static string[] CreateRandomArray(int arraySize)
        {
            var array = new string[arraySize];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Random.Next(0, 99999).ToString();
            }
            return array;
        }

        public static int CreateRandomInt() => Random.Next(1, 100);

        public static string CreateRandomString()
        {
            var stringLength = Random.Next(5, 20);
            var randomString = new char[stringLength];

            for (int i = 0; i < stringLength; i++)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                randomString[i] = chars[Random.Next(chars.Length)];
            }
            return new string(randomString);
        }

        public static Sex GetRandomSex()
        {
            return (Sex)Random.Next(Enum.GetNames(typeof(Sex)).Length);
        }
    }
}
