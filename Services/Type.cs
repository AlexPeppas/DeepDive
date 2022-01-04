using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DeepDiveTechnicals.Services
{
   
        [DataContract]
        public class PayloadObject
        {
            [DataMember(Name = "types")]
            public List<string> Types { get; set; }

            [DataMember(Name = "businessUnits")]
            public List<string> BusinessUnits { get; set; }

            [DataMember(Name = "processes")]
            public List<string> Processes { get; set; }

            [DataMember(Name = "name")]
            public string Name { get; set; }
        }





        [DataContract]
        public class GetEnvironmentalVariables
        {
            [DataMember(Name = "payload")]
            public PayloadObject Payload { get; set; }
        }



        public class Types
        {
            public string value { get; set; }
        }



        public class Processes
        {
            public string value { get; set; }
        }



        public class BusinessUnits
        {
            public string value { get; set; }
        }
    
}
