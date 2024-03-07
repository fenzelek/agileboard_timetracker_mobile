using System;
using System.Text;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class Emoji
    {

        private readonly int[] codes;

        public Emoji(params int[] codes)
        {
            this.codes = codes;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(codes.Length);
            foreach (var code in codes)
            {
                builder.Append(char.ConvertFromUtf32(code));
            }

            return builder.ToString();
        }
        
    }
}