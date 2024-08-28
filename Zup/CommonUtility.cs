using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zup
{
    public static class CommonUtility
    {
        public static int GetMin(params int[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("At least one argument is required.");
            }

            return args.Min();
        }

        public static int GetMax(params int[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("At least one argument is required.");
            }

            return args.Max();
        }
    }
}
