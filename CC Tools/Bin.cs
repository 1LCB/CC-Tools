using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CC_Tools
{

    public partial class Bin
    {

        public string Scheme { get; set; }
        public Number Number { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Prepaid { get; set; }
        public Country Country { get; set; }
        public Bank Bank { get; set; }
    }

    public partial class Bank
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
    }

    public partial class Country
    {
        public string Numeric { get; set; }
        public string Alpha2 { get; set; }
        public string Name { get; set; }
        public string Emoji { get; set; }
        public string Currency { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public partial class Number
    {
        public string Length { get; set; }
        public string Luhn { get; set; }
    }

}


