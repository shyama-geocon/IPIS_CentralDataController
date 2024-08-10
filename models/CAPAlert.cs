using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models
{
    public class CAPParameter
    {
        public string ValueName { get; set; }
        public string Value { get; set; }
    }

    public class CAPResource
    {
        public string ResourceDesc { get; set; }
        public string MimeType { get; set; }
        public string Size { get; set; }
        public string Uri { get; set; }
        public string DerefUri { get; set; }
        public string Digest { get; set; }
    }

    public class CAPArea
    {
        public string AreaDesc { get; set; }
        public string Polygon { get; set; }
        public string Altitude { get; set; }
        public string Ceiling { get; set; }
    }

    public class CAPInfo
    {
        public string Language { get; set; }
        public string Category { get; set; }
        public string Event { get; set; }
        public string ResponseType { get; set; }
        public string Urgency { get; set; }
        public string Severity { get; set; }
        public string Certainty { get; set; }
        public string Audience { get; set; }
        public DateTime Effective { get; set; }
        public DateTime Expires { get; set; }
        public string Headline { get; set; }
        public string Description { get; set; }
        public string Instruction { get; set; }
        public string Web { get; set; }
        public string Contact { get; set; }
        public List<CAPParameter> Parameter { get; set; }
        public List<CAPResource> Resource { get; set; }
        public List<CAPArea> Area { get; set; }
    }

    public class CAPAlert
    {
        public string Identifier { get; set; }
        public string Sender { get; set; }
        public DateTime Sent { get; set; }
        public string Status { get; set; }
        public string MsgType { get; set; }
        public string Source { get; set; }
        public string Scope { get; set; }
        public string Restriction { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string References { get; set; }
        public string Incidents { get; set; }
        public CAPInfo Info { get; set; }
    }
}
