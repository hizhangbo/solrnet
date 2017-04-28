using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrHelperApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(SolrHelper.SOLR_HTTP.Select("shxh_cgmx").Q("*:*").Wt(SolrHelper.ResultType.Json).Rows(0).Indent(SolrHelper.IndentType.On).Stats(true).StatsField("my", SolrHelper.StatsMethodType.Sum, SolrHelper.StatsMethodType.Max).Facet(true).FacetField("gysid", "year").FacetPivot("gysid", "year").End());

            Dictionary<string, SolrHelper.FieldMethodType> facet_field = new Dictionary<string, SolrHelper.FieldMethodType>();
            facet_field.Add("my", SolrHelper.FieldMethodType.Sum);
            facet_field.Add("cnt", SolrHelper.FieldMethodType.Sum);
            facet_field.Add("spxxid", SolrHelper.FieldMethodType.Unique);

            Console.WriteLine(SolrHelper.SOLR_HTTP.Select("shxh_cgmx").Q("*:*").Wt(SolrHelper.ResultType.Json).Rows(0).Indent(SolrHelper.IndentType.On).JsonFacet("gysid", facet_field).Fq("sdhrq", "[2016-08-01T16:00:00Z TO 2016-09-01T16:00:00Z]"));


            Console.ReadKey();
        }
    }
}
