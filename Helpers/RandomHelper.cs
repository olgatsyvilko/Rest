namespace Rest.Helpers
{
    public static class RandomHelper
    {
        public static string[] CreateRandomArray(int arraySize)
        {
            var array = new string[arraySize];
            Random random = new();

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random.Next(0, 99999).ToString();
            }
            return array;
        }
    }
}
