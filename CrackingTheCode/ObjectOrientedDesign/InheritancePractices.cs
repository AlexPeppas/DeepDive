using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.CrackingTheCode.ObjectOrientedDesign
{
    public static class InheritancePractices
    {
        
        public static void MainFunc() 
        {
            WorkItem item = new WorkItem("Art Of War", "Book", new TimeSpan());
            
            Console.WriteLine(item.ToString());
            item.Update("Art of War II", new TimeSpan());
            Console.WriteLine(item.ToString());

            ChangeRequest chrIt = new ChangeRequest("BigBang", "Cosmology Book", new TimeSpan(), 1);
            Console.WriteLine(chrIt.ToString());
            chrIt.Update("Big Bang", new TimeSpan());
            Console.WriteLine(chrIt.ToString());
        }
        
    }

    public class WorkItem
    {
        private static int currentId;

        protected int ID { get; set; }
        protected string Title { get; set; }
        protected string Desc { get; set; }
        protected TimeSpan JobLength { get; set; }

        public WorkItem()
        {
            Title = "Default";
            Desc = "Default";
            ID = 0;
            JobLength = new TimeSpan();
        }

        public WorkItem(string title, string desc, TimeSpan jobLength)
        {
            ID = GetNextId();
            this.Title = title;
            this.Desc = desc;
            this.JobLength = jobLength;
        }

        //static constructor to initialize the static member.
        //this constructor is called one time, automatically, before any
        //instance of WorkItem
        static WorkItem() => currentId = 0;
        protected int GetNextId() => ++currentId; 

        public void Update(string title, TimeSpan jobLen)
        {
            this.Title = title;
            this.JobLength = jobLen;
        }

        public override string ToString() => $"{this.ID} - {this.Title}";
    }

    public class ChangeRequest : WorkItem
    {
        protected int originalItemId { get; set; }

        public ChangeRequest() { }

        public ChangeRequest(string title, string desc, TimeSpan joblen, int originalId)
        {
            this.ID = GetNextId();
            this.Title = title;
            this.Desc = desc;
            this.JobLength = joblen;

            this.originalItemId = originalId;
        }
    }

}
