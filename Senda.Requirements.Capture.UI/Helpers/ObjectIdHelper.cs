using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Senda.Requirements.Capture.UI.Helpers
{
    public static class ObjectIdHelper
    {
        private class ObjectIdHelperInstance
        {
            public string Prefix { get; set; }
            public Type Type { get; set; }
        }

        private static List<string> ValidObjectPrefixes
        {
            get
            {
                return new List<string>() {
                    "ATT", "AWD", "AWL", "ALQ", "BID", "BDL", "BLQ", "CLL", "COM", "CEL", "CNT", "EML", "EMV", "EON", "EOP", "OPP", "OPL", "OLQ", "PRT", "PEL", "PFL", "PFH", "PRJ", "RFQ", "RFL", "RLQ", "QTE", "QTL", "QLQ", "USR", "WFQ", "WFA", "NOT", "PRL"
                    };
            }
        }


        /// <summary>
        /// Tests if the first three characters are among the acceptable prefixes,
        /// if its 11 characters in total, and if the final eight characters can
        /// be parsed as an integer.
        /// </summary>
        /// <param name="object_id"></param>
        /// <returns></returns>
        public static bool IsValid(string object_id)
        {
            bool valid = false;

            object_id = object_id.Trim().ToUpper();

            if(object_id.Length == 11)
            {
                string prefix = object_id.Substring(0, 3);
                string idNumber = object_id.Substring(3);

                if(ValidObjectPrefixes.Contains(prefix))
                {
                    if(int.TryParse(idNumber, out int id))
                    {
                        valid = true;
                    }
                }
            }

            return valid; 
        }

        private static int PreferredIdPadding { get { return 8; } }





        public static string GetPrefix(string object_id)
        {
            return string.IsNullOrEmpty(object_id) ? "" : AlphabeticOnly(object_id.ToUpper());
        }

        public static string NumericOnly(string str) { return str == null ? null : Regex.Replace(str, "[^0-9]", ""); }
        public static string AlphabeticOnly(string str) { return str == null ? null : Regex.Replace(str.ToUpper(), "[^A-Z]", ""); }

        public static int GetId(string object_id)
        {
            int id = 0;
            string id_string = string.IsNullOrEmpty(object_id) ? "" : NumericOnly(object_id);
            int.TryParse(id_string, out id);
            return id;
        }

        public static string Format(string object_id)
        {
            if (!IsValid(object_id)) throw new Exception("Valid ObjectId Is Required");

            string prefix = GetPrefix(object_id);
            int id = GetId(object_id);

            return Format(prefix, id);
        }

        private static string Format(string prefix, int id)
        {
            return string.Format("{0}{1}", prefix, id.ToString().PadLeft(PreferredIdPadding, "0"[0]));
        }

        //public static bool AreIdentical(string object_id_a, string object_id_b)
        //{
        //    if (IsValid(object_id_a) && IsValid(object_id_b))
        //    {
        //        return GetPrefix(object_id_a) == GetPrefix(object_id_b) && GetId(object_id_a) == GetId(object_id_b);
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}


    }
}
