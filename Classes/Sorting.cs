using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Production_Planner.Classes
{
    public static class Sorting
    {
        public static void SortProductsAlpha(List<Product> prods)
        {
            prods.Sort((i1, i2) => GetLastWord(i1.Name).CompareTo(GetLastWord(i2.Name)));
        }

        public static void SortProductsAlpha(List<ProductQty> prods)
        {
            prods.Sort((i1, i2) => GetLastWord(i1.Name).CompareTo(GetLastWord(i2.Name)));
        }

        private static string GetLastWord(string str)
        {
            string[] arr = str.Split(' ');
            return arr.Last();
        }

        public static void SortPartsAlpha(List<Part> parts)
        {
            parts.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));
        }

        public static void SortPartsAlpha(List<PartQty> parts)
        {
            parts.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));
        }

        public static void SortPartTypesAlpha(List<PartType> pt)
        {
            pt.Sort((i1, i2) => i1.TypeName.CompareTo(i2.TypeName));
        }
    }
}
