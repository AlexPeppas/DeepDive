using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.CrackingTheCode.ObjectOrientedDesign
{
    class DummyObject 
    {
        public int MyProperty { get; set; }
    }

    public class Modifiers
    {
        public static void UseRef()
        {
            int x = 5;
            ChangeRef(ref x); //it may be changed insidie this Function
            Console.WriteLine($"Now x is {x}");
        }
        private static void ChangeRef(ref int x)
        {
            x--; //this will affect the calling func's x value.
            return;
        }

        public static void UseOut()
        {
            DummyObject x;
            ChangeOut(out x); //must be instantiated in this Function
            Console.WriteLine($"now x's MyProperty is {x.MyProperty}");
        }
        private static void ChangeOut(out DummyObject x)
        {
            //ERROR if you do not instantiate 
            x = new DummyObject();
            x.MyProperty = 1;
        }

        public static void UseIn()
        {
            DummyObject x = new DummyObject();
            ChangeIn(in x); //cannot instantiate inside this Function
        }
        private static void ChangeIn(in DummyObject x)
        {
            //x = new DummyObject(); Error x is a readonly now
            x.MyProperty = 1;
            return;
        }
    }

    sealed class TryToInherite 
    { }

    /*public class Trier : TryToInherite //trows an error
    {

    }*/

    public abstract class Abstract
    {
        public Abstract(int x)
        {

        }

        public virtual int GetItems(int x)=> x++;

        public abstract int GetAbstractItems(int x);
    }
    //Abstract can be used as a base class but if it has a constructor
    //then your derived class by Abstract should feed the base class' constructor
    public class TrierAbstract : Abstract
    {
        public TrierAbstract(int x) : base (x)
        {

        }
        public override int GetItems(int x)
        {
            //return base.GetItems(x);
            return x--;
        }

        public override int GetAbstractItems(int x)
        {
            return x++;
        }
    }
}
