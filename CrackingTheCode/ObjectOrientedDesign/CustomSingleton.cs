using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.ObjectOrientedDesign
{

    public class SingletonUser : CustomSingleton
    {
        //return instance
        private CustomSingleton instance = getInstance();
        
    }

    public class CustomSingleton
    {
        private static CustomSingleton _instance = null;

        protected CustomSingleton()
        {

        }

        public static CustomSingleton getInstance()
        {
            if (_instance == null)
                _instance = new CustomSingleton();
            return _instance;
        }
    }

}
