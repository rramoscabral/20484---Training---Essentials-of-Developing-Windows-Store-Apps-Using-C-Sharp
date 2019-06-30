using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ILoveNotes.Data;

namespace ILoveNotes.Common
{
    public static class Extension
    {
        public static string HttpEncode(this string value)
        {
            return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }

        public static string ToCleanString(this string value)
        {
            return value.Replace("&", " ").Replace("<", " ").Replace(">", " ").Replace("\"", " ").Replace("'", " ");
        }

        public static string FormatedDate(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy");
        }

        public static string ToShortString(this string txt)
        {
            if (txt.Length > 256)
                return txt.Substring(0, 256);
            else
                return txt;
        }

        public static ObservableCollection<string> ToObservable(this List<string> list)
        {
            var observList = new ObservableCollection<string>();
            foreach (var item in list)
                observList.Add(item);
            return observList;
        }

        public static ObservableCollection<NoteDataCommon> ToObservable(this List<NoteDataCommon> list)
        {
            var observList = new ObservableCollection<NoteDataCommon>();
            foreach (var item in list)
                observList.Add(item);
            return observList;
        }


        /// <summary>
        /// Convert path of full path
        /// </summary>
        /// <param name="value">object local path</param>
        /// <returns>full path of the object</returns>
        public static string ToBaseUrl(this string value)
        { 
            if (string.IsNullOrEmpty(value))
                return null;
            else if(value.StartsWith("ms-"))
                return value;
            else if (value.StartsWith("Assets"))
                return string.Format("ms-appx:///{0}", value);
            else if (value.StartsWith("http://") || value.StartsWith("https://"))
                return value;
            return string.Format("ms-appdata:///Local/{0}", value);
        }
    }
}
