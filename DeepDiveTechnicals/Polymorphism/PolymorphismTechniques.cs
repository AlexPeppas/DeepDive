using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.Polymorphism
{
    public class Vehicles
    {
        protected virtual int Prop { get; set; }
        protected virtual int DoSomething(int a, int b) => a + b;
    }

    public class Car : Vehicles
    {
        protected override int Prop { get => base.Prop%2; set => base.Prop = value; }
        protected override int DoSomething(int a, int b)
        {
            return base.DoSomething(a, b);

        }
    }

    public class AutoMobile : Vehicles
    {
        protected override int Prop { get => base.Prop; set => base.Prop = value; }

        protected sealed override int DoSomething(int a, int b)
        {
            return a - b;
        }
    }

    sealed public class Bicycle : AutoMobile
    {
        sealed protected override int Prop { get => base.Prop+1; set => base.Prop = value+1; }
    }

    /////////////////////////////////////////////////////////////////////////////
    public class Employee 
    {
        public string Name { get; set; }
        protected int _basepay;

        public Employee(string name , int basepay)
        {
            Name = name;
            basepay = _basepay;
        }

        public virtual decimal CalculatePay()
        {
            return _basepay;
        }
    }

    public class SalesEmployee : Employee
    {
        private int _salesBonus;

        public SalesEmployee(int salesBonus, string name, int basepay) : base (name,basepay)
        {
            _salesBonus = salesBonus;
        }
        //cannot be inherited, can be used only if the salesEmployee be instanciated
        sealed public override decimal CalculatePay()
        {
            return _basepay + _salesBonus;
        }
    }
}
