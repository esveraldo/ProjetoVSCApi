namespace ProjetoVSCApi.HATEOAS
{
    public class Link
    {

        public Link(string href, string rel, string method)
        {
            this.href = href;
            this.rel = rel;
            this.method = method;
        }

        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }

    
}