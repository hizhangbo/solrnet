using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrHelperApp
{
    public static class SolrHelper
    {
        public static readonly string SOLR_HTTP = ConfigurationManager.AppSettings["solrhttp"];

        public static string Select(this string solr_http, string core)
        {
            return string.Format("{0}/{1}/select?", solr_http, core);
        }

        public static string Q(this string solr_http, string q)
        {
            //验证q的格式 *:*
            return string.Format("{0}q={1}", solr_http, q);
        }

        public static string Start(this string solr_http, int start)
        {
            //如果不选默认为10，如果有json.facet默认为0
            return string.Format("{0}&start={1}", solr_http, start);
        }

        public static string Rows(this string solr_http, int rowNum)
        {
            //如果不选默认为10，如果有json.facet默认为0
            return string.Format("{0}&rows={1}", solr_http, rowNum);
        }

        public static string Wt(this string solr_http, ResultType type)
        {
            string wt = null;
            switch (type)
            {
                case ResultType.Json:
                    wt = "wt=json";
                    break;
                case ResultType.Xml:
                    wt = "wt=xml";
                    break;
                case ResultType.Python:
                    wt = "wt=python";
                    break;
                case ResultType.Ruby:
                    wt = "wt=ruby";
                    break;
                case ResultType.PHP:
                    wt = "wt=php";
                    break;
                case ResultType.CSV:
                    wt = "wt=csv";
                    break;
            }
            return string.Format("{0}&{1}", solr_http, wt);
        }

        public static string Indent(this string solr_http, IndentType type)
        {
            string indent = null;
            switch (type)
            {
                case IndentType.Off:
                    indent = "indent=off";
                    break;
                case IndentType.On:
                    indent = "indent=on";
                    break;
                default:
                    indent = "";
                    break;
            }
            return string.Format("{0}&{1}", solr_http, indent);
        }

        public static string Stats(this string solr_http, bool isUse)
        {
            string stats = "";
            if (isUse)
            {
                stats = "stats=true";
            }
            else
            {
                stats = "stats=false";
            }
            return string.Format("{0}&{1}", solr_http, stats);
        }

        public static string StatsField(this string solr_http, string field, params StatsMethodType[] method)
        {
            string stats_field = field;
            List<string> re_method = new List<string>();
            foreach (var item in method)
            {
                switch (item)
                {
                    case StatsMethodType.Count:
                        re_method.Add("count=true");
                        break;
                    case StatsMethodType.Max:
                        re_method.Add("max=true");
                        break;
                    case StatsMethodType.Min:
                        re_method.Add("min=true");
                        break;
                    case StatsMethodType.Mean:
                        re_method.Add("mean=true");
                        break;
                    case StatsMethodType.Missing:
                        re_method.Add("missing=true");
                        break;
                    case StatsMethodType.Stddev:
                        re_method.Add("stddev=true");
                        break;
                    case StatsMethodType.Sum:
                        re_method.Add("sum=true");
                        break;
                    case StatsMethodType.SumOfSquares:
                        re_method.Add("sumOfSquares=true");
                        break;
                }
            }

            return string.Format("{0}&stats.field={{!{1}}}{2}", solr_http, string.Join("+", re_method), stats_field);
        }

        public static string Facet(this string solr_http, bool isUse)
        {
            string facet = "";
            if (isUse)
            {
                facet = "facet=true";
            }
            else
            {
                facet = "facet=false";
            }
            return string.Format("{0}&{1}", solr_http, facet);
        }

        public static string FacetField(this string solr_http, params string[] field)
        {
            return string.Format("{0}&facet.field={1}", solr_http, string.Join("&facet.field=", field));
        }

        public static string FacetPivot(this string solr_http, params string[] field)
        {
            return string.Format("{0}&facet.pivot={1}", solr_http, string.Join(",", field));
        }

        public static string JsonFacet(this string solr_http, string facet_field, Dictionary<string, FieldMethodType> field)
        {
            string re_facet_field = string.Format("type:terms,field:{0}", facet_field);
            List<string> field_lst = new List<string>();

            foreach (var item in field)
            {
                switch (item.Value)
                {
                    case FieldMethodType.Sum:
                        field_lst.Add(string.Format("{0}:\"sum({1})\"", item.Key, item.Key));
                        break;
                    case FieldMethodType.Avg:
                        field_lst.Add(string.Format("{0}:\"avg({1})\"", item.Key, item.Key));
                        break;
                    case FieldMethodType.Unique:
                        field_lst.Add(string.Format("{0}:\"unique({1})\"", item.Key, item.Key));
                        break;
                }
            }

            string facet = string.Format("fz:{{ {0},facet:{{ {1} }} }}", re_facet_field, string.Join(",", field_lst));

            return string.Format("{0}&json.facet={{{1}}}", solr_http, facet);
        }

        public static string Fq(this string solr_http, string key, string value)
        {
            return string.Format("{0}&fq={1}:{2}", solr_http, key, value);
        }

        public static string End(this string solr_http)
        {
            //语法自动检查，如果语法错误，直接抛出自定义异常
            try
            {
                return solr_http;
                throw new SolrException();
            }
            catch (SolrException ex)
            {
                return ex.Message;
            }
        }

        public enum ResultType
        {
            Json = 1,
            Xml = 2,
            Python = 3,
            Ruby = 4,
            PHP = 5,
            CSV = 6
        }

        public enum IndentType
        {
            Off = 0,
            On = 1
        }

        public enum StatsMethodType
        {
            Min = 0,           //最小值
            Max = 1,           //最大值
            Sum = 2,           //合计
            Count = 3,         //记录数
            Missing = 4,       //空值数
            SumOfSquares = 5,  //平方和
            Mean = 6,          //平均数
            Stddev = 7         //标准差
        }

        public enum FieldMethodType
        {
            Sum = 0,
            Avg = 1,
            Unique = 2
        }

        private static String EscapeQueryChars(String s)
        {
            StringBuilder sb = new StringBuilder();
            String regex = "[+\\-&|!(){}\\[\\]^\"~*?:(\\)]";
            for (int i = 0; i < s.Length; i++)
            {
                char c = Convert.ToChar(s[i]);
                if (regex.Contains(c))
                {
                    sb.Append(@"\");
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

    }

    public class SolrException : Exception
    {
        public override string Message
        {
            get
            {
                return "solr exception: solr 语法错误。";
            }
        }
    }
}
